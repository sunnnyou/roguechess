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
        public List<IBuff> Buffs { get; } = new List<IBuff>();

        public SpriteRenderer SpriteRenderer { get; set; }

        public Vector2Int Position;
        public Color OriginalColor;
        public bool IsWhite;
        public ChessPiece CurrentPiece;

        private Color highlightColor = new(0.0f, 0.4f, 0.0f); // green

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

        public void UpdatePiece(ChessPiece newPiece, bool ignoreMoveHistory, bool ignoreFight)
        {
            // Fight current piece with new piece and set winner as this tiles current piece
            if (!ignoreFight && !ChessPiece.FightPiece(this.CurrentPiece, newPiece))
            {
                return;
            }

            if (newPiece != null)
            {
                Vector2Int? enemyPos =
                    newPiece.CurrentTile != null ? newPiece.CurrentTile.Position : null;

                // Ensure Z position is in front of tiles
                newPiece.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    -2 // render in front of tiles
                );
                newPiece.CurrentTile = this;

                // process buffs at end to get updated piece position and tile
                var additionalCapturedPieces = this.ApplyBuffs(newPiece);

                if (!ignoreMoveHistory)
                {
                    Vector2Int? enemyTargetPos;
                    if (this.CurrentPiece == null || this.CurrentPiece.Lives <= 0)
                    {
                        enemyTargetPos = this.Position;
                    }
                    else
                    {
                        enemyTargetPos = enemyPos;
                    }

                    newPiece.Board.AddMove(
                        newPiece,
                        enemyPos,
                        enemyTargetPos,
                        this.CurrentPiece,
                        additionalCapturedPieces
                    );
                }
            }
            else if (this.CurrentPiece != null && !ignoreMoveHistory)
            {
                this.CurrentPiece.Board.AddMove(null, null, this.Position, this.CurrentPiece, null);
            }

            this.CurrentPiece = newPiece;
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

        public void AddBuff(IBuff buff)
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
                        buff.ApplyBuff(piece, piece.Board) is List<MoveRule> additionalMoves
                        && additionalMoves.Count > 0
                    )
                    {
                        piece.MoveRules.AddRange(additionalMoves);
                    }
                }
                else if (buff is UpdateBuff)
                {
                    if (
                        buff.ApplyBuff(piece, piece.Board) is ChessPiece updatedPiece
                        && updatedPiece != null
                    )
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
    }
}
