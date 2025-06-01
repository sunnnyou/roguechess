namespace Assets.Scripts.Game.Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Buffs.Pieces;
    using Assets.Scripts.Game.Buffs.Pieces.Move;
    using Assets.Scripts.Game.Buffs.Pieces.Update;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    public class ChessPiece : MonoBehaviour, IChessObject
    {
        public List<IBuff> Buffs { get; } = new();

        public SpriteRenderer SpriteRenderer { get; set; }

        public ChessPieceType PieceType { get; private set; }

        public bool IsWhite { get; set; }

        public ChessTile CurrentTile { get; set; }

        public List<MoveRule> MoveRules { get; private set; } = new();

        public int Strength { get; private set; } = 1; // Strength of the piece, used for reducing lives in fights

        public int Lives { get; private set; } = 1; // Number of hits before piece is destroyed

        internal ChessBoard Board { get; private set; }

        public void Initialize(
            ChessPieceType type,
            bool white,
            Sprite sprite,
            ChessBoard chessBoard,
            List<Material> materials,
            List<MoveRule> customRules = null,
            List<IBuff> buffs = null
        )
        {
            this.PieceType = type;
            this.IsWhite = white;
            this.Board = chessBoard;

            // Check if we already have a SpriteRenderer
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            }

            if (materials != null)
            {
                int baseRenderQueue = 3000; // default render queue value for sprites
                for (int i = 0; i < materials.Count; i++)
                {
                    var pieceMaterial = new Material(materials[i])
                    {
                        renderQueue = baseRenderQueue - (i + 1), // lower value so that its rendered behind sprite
                    };
                    this.SpriteRenderer.materials = this
                        .SpriteRenderer.materials.Append(pieceMaterial)
                        .ToArray();
                }
            }

            this.SpriteRenderer.sortingOrder = 10; // render above tiles
            this.SpriteRenderer.sprite = sprite;

            // Set default move rules based on piece type
            this.SetDefaultMoveRules(customRules);

            // Add buffs if provided
            if (buffs != null)
            {
                this.Buffs.AddRange(buffs);
            }

            if (type == ChessPieceType.Pawn)
            {
                this.Buffs.Add(new EnPassantPieceBuff());
                this.Buffs.Add(new ExtraReachPieceBuff());
                this.Buffs.Add(new PromoteAtEndPieceBuff(null, white ? this.Board.Height - 1 : 0));
            }
        }

        public void SetDefaultMoveRules(List<MoveRule> customRules = null)
        {
            this.MoveRules.Clear();

            switch (this.PieceType)
            {
                case ChessPieceType.Pawn:
                    this.MoveRules = PawnMove.GetMoveRules(this.IsWhite);
                    break;
                case ChessPieceType.Rook:
                    this.MoveRules = RookMove.GetMoveRules();
                    break;
                case ChessPieceType.Knight:
                    this.MoveRules = KnightMove.GetMoveRules();
                    break;
                case ChessPieceType.Bishop:
                    this.MoveRules = BishopMove.GetMoveRules();
                    break;
                case ChessPieceType.Queen:
                    this.MoveRules = QueenMove.GetMoveRules();
                    break;
                case ChessPieceType.King:
                    this.MoveRules = KingMove.GetMoveRules();
                    break;
                case ChessPieceType.Custom:
                    this.MoveRules = customRules ?? PawnMove.GetMoveRules(this.IsWhite);
                    break;
            }
        }

        public List<ChessTile> GetValidTiles()
        {
            var validMoves = new List<ChessTile>();

            if (this.CurrentTile == null || this.Board == null)
            {
                return validMoves;
            }

            validMoves = MoveRule.GetValidTiles(
                this.MoveRules,
                this.CurrentTile.Position,
                this.Board,
                this.IsWhite
            );

            // Get valid tiles from buffs
            foreach (var buff in this.Buffs)
            {
                if (buff == null || !buff.IsActive || buff is not MoveBuff)
                {
                    continue;
                }

                if (
                    buff.ApplyBuff(this, this.Board) is List<ChessTile> buffedTiles
                    && buffedTiles.Count > 0
                )
                {
                    validMoves.AddRange(buffedTiles);
                }
            }

            return validMoves;
        }

        public List<ChessMove> GetValidMoves()
        {
            var moves = new List<ChessMove>();
            var validTiles = this.GetValidTiles();

            foreach (var tile in validTiles)
            {
                moves.Add(
                    new ChessMove(
                        this.CurrentTile.Position,
                        tile.Position,
                        tile.CurrentPiece != null
                    )
                );
            }

            return moves;
        }

        public static bool FightPiece(ChessPiece currentPiece, ChessPiece enemyPiece)
        {
            if (currentPiece == null && enemyPiece != null)
            {
                return true;
            }
            else if (currentPiece != null && enemyPiece == null)
            {
                currentPiece.DestroyPiece();
                return true;
            }
            else if (currentPiece != null && enemyPiece != null)
            {
                // TODO: fight function (use buffs)

                currentPiece.Lives -= enemyPiece.Strength;
                if (currentPiece.Lives <= 0)
                {
                    // Enemy won and killed this piece
                    currentPiece.DestroyPiece();
                    return true;
                }
            }

            return false;
        }

        private void DestroyPiece()
        {
            // TODO: add capture animation and add to captured pieces list for ui
            this.gameObject.SetActive(false);
        }

        private void RevivePiece(int lives = 1)
        {
            this.gameObject.SetActive(true);
            this.Lives = lives;
        }

        public void UseUpdateBuffs()
        {
            foreach (var buff in this.Buffs)
            {
                if (
                    buff != null
                    && buff.IsActive
                    && buff is UpdateBuff
                    && buff.ApplyBuff(this, this.Board) is ChessPiece updatedPiece
                    && updatedPiece != null
                )
                {
                    this.UpdateFromBuff(updatedPiece);
                }
            }
        }

        public void UpdateFromBuff(ChessPiece updatedPiece)
        {
            if (updatedPiece == null)
            {
                Debug.LogError("Updated piece is null, cannot apply buff.");
                return;
            }

            // Update properties from the buffed piece
            this.PieceType = updatedPiece.PieceType;
            this.IsWhite = updatedPiece.IsWhite;
            this.Lives = updatedPiece.Lives;

            // Update move rules
            this.MoveRules = new List<MoveRule>(updatedPiece.MoveRules);

            // Update buffs
            this.Buffs.Clear();
            this.Buffs.AddRange(updatedPiece.Buffs);

            // Update sprite renderer
            // TODO: Maybe add function to check if white or black
            if (this.SpriteRenderer != null)
            {
                this.SpriteRenderer.sprite = updatedPiece.SpriteRenderer.sprite;
            }

            // TODO: maybe add function for updating tile if position has not changed
        }
    }
}
