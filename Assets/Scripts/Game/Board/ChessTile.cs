// Represents a single chess tile on the board
namespace Assets.Scripts.Game.Board
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    // Represents a single chess tile on the board
    public class ChessTile : MonoBehaviour, IChessObject
    {
        [SerializeField]
        private ChessTileData tileData;

        public List<BuffBase> Buffs { get; } = new List<BuffBase>();
        public SpriteRenderer SpriteRenderer { get; set; }
        public Vector2Int Position;
        public Color OriginalColor;
        public bool IsWhite;
        public ChessPiece CurrentPiece;

        private Color highlightColor = new(0.0f, 0.4f, 0.0f); // green
        private Color checkColor = new(0.4f, 0.0f, 0.0f); // red

        // Initialize from ScriptableObject data
        public void Initialize(ChessTileData data, Vector2Int pos, bool isWhiteTile)
        {
            this.tileData = data;
            this.Position = pos;
            this.IsWhite = isWhiteTile;

            // Make sure we have a SpriteRenderer at this point
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
                if (this.SpriteRenderer == null)
                {
                    this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
                }
            }

            // Apply data from ScriptableObject
            if (data != null)
            {
                this.SpriteRenderer.sprite = data.Sprite;
                this.highlightColor = data.HighlightColor;
                this.SpriteRenderer.sortingOrder = data.SortingOrder;

                // Set color based on tile type
                this.SpriteRenderer.color = isWhiteTile ? data.WhiteColor : data.BlackColor;

                // Add initial buffs
                if (data.InitialBuffs != null)
                {
                    this.Buffs.AddRange(data.InitialBuffs);
                }

                // Spawn starting piece if configured
                if (data.SpawnPieceOnInitialize && data.StartingPiece != null)
                {
                    ChessBoard.Instance.SpawnPiece(
                        data.StartingPiece.PieceType,
                        data.StartingPiece.IsWhite,
                        CoordinateHelper.VectorToString(this.Position),
                        this,
                        data.StartingPiece.Sprite,
                        data.StartingPiece.Materials,
                        data.StartingPiece.CustomMoveRules
                    );
                }
            }
            else
            {
                // Fallback to default colors if no data provided
                this.SpriteRenderer.color = isWhiteTile ? Color.white : Color.black;
                this.SpriteRenderer.sortingOrder = 2;
            }

            this.OriginalColor = this.SpriteRenderer.color;
        }

        // Keep your existing Initialize method for backward compatibility
        public void Initialize(Vector2Int pos, bool isWhiteTile)
        {
            this.Position = pos;
            this.IsWhite = isWhiteTile;

            // Make sure we have a SpriteRenderer at this point
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
                if (this.SpriteRenderer == null)
                {
                    this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
                }
            }

            this.OriginalColor = this.SpriteRenderer.color;

            // Render tile behind chess pieces
            this.SpriteRenderer.sortingOrder = 2;
        }

        public void Highlight(bool highlight)
        {
            this.SpriteRenderer.color = highlight ? this.highlightColor : this.OriginalColor;
        }

        public ChessMoveHistory UpdatePiece(
            ChessPiece newPiece,
            bool ignoreMoveHistory,
            bool ignoreFight
        )
        {
            // Fight current piece with new piece and set winner as this tiles current piece
            if (
                !ignoreFight
                && !ChessPiece.FightPiece(this.CurrentPiece, newPiece, !ignoreMoveHistory)
            )
            {
                // TODO: add move history for fighting and loosing hp
                return null;
            }

            ChessMoveHistory move = null;

            if (newPiece != null)
            {
                Vector2Int? enemyPos = null;
                if (newPiece.CurrentTile != null)
                {
                    enemyPos = newPiece.CurrentTile.Position;
                    newPiece.CurrentTile.CurrentPiece = null;
                }

                // Ensure Z position is in front of tiles
                newPiece.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    -2 // render in front of tiles
                );

                // Update tile of new piece
                newPiece.CurrentTile = this;

                // process buffs at end to get updated piece position and tile
                var additionalCapturedPieces = this.ApplyBuffs(newPiece);

                Vector2Int? enemyTargetPos;
                if (this.CurrentPiece == null || this.CurrentPiece.Lives <= 0)
                {
                    enemyTargetPos = this.Position;
                }
                else
                {
                    enemyTargetPos = enemyPos;
                }

                move = new ChessMoveHistory(
                    newPiece,
                    enemyPos,
                    enemyTargetPos,
                    this.CurrentPiece,
                    additionalCapturedPieces,
                    ChessBoard.Instance.IsWhiteTurn
                );

                if (!ignoreMoveHistory)
                {
                    ChessBoard.Instance.AddMove(move);
                }
            }
            else if (this.CurrentPiece != null && !ignoreMoveHistory)
            {
                move = new ChessMoveHistory(
                    null,
                    null,
                    this.Position,
                    this.CurrentPiece,
                    null,
                    ChessBoard.Instance.IsWhiteTurn
                );
                ChessBoard.Instance.AddMove(move);
            }

            this.CurrentPiece = newPiece;
            return move;
        }

        public void Destroy()
        {
            if (Application.isEditor)
            {
                DestroyImmediate(this.CurrentPiece.gameObject);
                DestroyImmediate(this.gameObject);
            }
            else
            {
                Destroy(this.CurrentPiece.gameObject);
                Destroy(this.gameObject);
            }
        }

        public void AddBuff(BuffBase buff)
        {
            this.Buffs.Add(buff);
        }

        private List<ChessPiece> ApplyBuffs(ChessPiece piece)
        {
            List<ChessPiece> destroyedPieces = new();
            foreach (var buff in this.Buffs)
            {
                if (buff == null || !buff.IsActive)
                {
                    continue;
                }

                if (buff is MoveBuff)
                {
                    if (
                        buff.ApplyBuff(piece) is List<MoveRule> additionalMoves
                        && additionalMoves.Count > 0
                    )
                    {
                        piece.MoveRules.AddRange(additionalMoves);
                    }
                }
                else if (buff is UpdateBuff)
                {
                    if (buff.ApplyBuff(piece) is ChessPiece updatedPiece && updatedPiece != null)
                    {
                        if (updatedPiece.IsWhite == piece.IsWhite)
                        {
                            piece.UpdateFromBuff(updatedPiece);
                        }
                        else
                        {
                            destroyedPieces.Add(updatedPiece);
                        }
                    }
                }
            }

            return destroyedPieces;
        }

        // Property to access tile data description for UI or debugging
        public string GetDescription()
        {
            return this.tileData != null ? this.tileData.Description : null ?? string.Empty;
        }

        // Check if this tile has special properties
        public bool HasSpecialProperties()
        {
            return this.tileData != null && this.tileData.HasSpecialProperties;
        }

        // Get the starting piece data for this tile
        public ChessPieceData GetStartingPiece()
        {
            return this.tileData != null ? this.tileData.StartingPiece : null;
        }

        // Check if this tile should spawn a piece on initialize
        public bool ShouldSpawnPiece()
        {
            return this.tileData != null && this.tileData.SpawnPieceOnInitialize;
        }

        public void InCheck(bool inCheck)
        {
            if (inCheck)
            {
                this.SpriteRenderer.color = this.checkColor;
            }
            else
            {
                this.SpriteRenderer.color = this.OriginalColor;
            }
        }
    }
}
