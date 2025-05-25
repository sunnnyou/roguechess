namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    public class ChessPiece : MonoBehaviour
    {
        public ChessPieceType PieceType;
        public bool IsWhite;
        public Sprite Sprite;
        public ChessTile CurrentTile;
        public List<MoveRule> MoveRules = new List<MoveRule>();
        public int Lives = 1; // Number of hits before piece is destroyed
        public List<IBuff> Buffs = new List<IBuff>();

        private SpriteRenderer spriteRenderer;
        private ChessBoard board;

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
            this.Sprite = sprite;
            this.board = chessBoard;

            // Check if we already have a SpriteRenderer
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.spriteRenderer == null)
            {
                this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            }

            if (materials != null)
            {
                int baseRenderQueue = 3000; // default render queue value for sprites
                for (int i = 0; i < materials.Count; i++)
                {
                    var pieceMaterial = new Material(materials[i])
                    {
                        renderQueue = --baseRenderQueue, // lower value so that its rendered behind sprite
                    };
                    this.spriteRenderer.materials = this
                        .spriteRenderer.materials.Append(pieceMaterial)
                        .ToArray();
                }
            }

            this.spriteRenderer.sortingOrder = 10; // render above tiles
            this.spriteRenderer.sprite = sprite;

            // Set default move rules based on piece type
            this.SetDefaultMoveRules(customRules);

            // Add buffs if provided
            if (buffs != null)
            {
                this.Buffs.AddRange(buffs);
            }

            if (type == ChessPieceType.Pawn)
            {
                this.Buffs.Add(new PawnBuff());
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

            if (this.CurrentTile == null || this.board == null)
            {
                return validMoves;
            }

            // Parse current position
            ChessBoard.ParseCoordinate(
                this.CurrentTile.Coordinate,
                out int currentX,
                out int currentY
            );

            validMoves = MoveRule.GetValidTiles(
                this.MoveRules,
                currentX,
                currentY,
                this.board,
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
                    buff.ApplyBuff(currentX, currentY, this.board, this.IsWhite)
                        is List<ChessTile> buffedTiles
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
            List<ChessMove> moves = new List<ChessMove>();
            List<ChessTile> validTiles = this.GetValidTiles();

            foreach (var tile in validTiles)
            {
                moves.Add(
                    new ChessMove(
                        ChessBoard.GetPositionFromCoordinate(this.CurrentTile.Coordinate),
                        ChessBoard.GetPositionFromCoordinate(tile.Coordinate),
                        tile.CurrentPiece != null
                    )
                );
            }

            return moves;
        }

        public bool FightPiece(ChessPiece enemyPiece)
        {
            if (enemyPiece == null)
            {
                this.DestroyPiece();
                return true;
            }

            // TODO: fight function

            this.Lives--;
            if (this.Lives <= 0)
            {
                this.DestroyPiece();
                return true;
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
    }
}
