namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ChessPiece : MonoBehaviour
    {
        public ChessPieceType PieceType;
        public bool IsWhite;
        public Sprite Sprite;
        public ChessTile CurrentTile;
        public List<MoveRule> MoveRules = new List<MoveRule>();
        public int Lives = 1; // Number of hits before piece is destroyed

        private SpriteRenderer spriteRenderer;
        private ChessBoard board;

        public void Initialize(
            ChessPieceType type,
            bool white,
            Sprite sprite,
            ChessBoard chessBoard,
            List<Material> materials,
            List<MoveRule> customRules = null
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
        }

        public void SetDefaultMoveRules(List<MoveRule> customRules = null)
        {
            this.MoveRules.Clear();

            switch (this.PieceType)
            {
                case ChessPieceType.Pawn:
                    // Forward movement (will need special handling for first move and capture)
                    this.MoveRules = MoveRule.Pawn(this.IsWhite);
                    break;

                case ChessPieceType.Rook:
                    // Horizontal and vertical movement
                    this.MoveRules = MoveRule.Rook();
                    break;

                case ChessPieceType.Knight:
                    // L-shaped movement
                    this.MoveRules = MoveRule.Knight();
                    break;

                case ChessPieceType.Bishop:
                    // Diagonal movement
                    this.MoveRules = MoveRule.Bishop();
                    break;

                case ChessPieceType.Queen:
                    // Combination of Rook and Bishop movement
                    this.MoveRules = MoveRule.Queen();
                    break;

                case ChessPieceType.King:
                    // One square in any direction
                    this.MoveRules = MoveRule.King();
                    break;

                case ChessPieceType.Custom:
                    this.MoveRules = customRules ?? MoveRule.Pawn(this.IsWhite);
                    break;
            }
        }

        public void CustomizeMoveRules(List<MoveRule> newRules)
        {
            this.MoveRules = newRules;
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

            foreach (MoveRule rule in this.MoveRules)
            {
                for (int distance = 1; distance <= rule.MaxDistance; distance++)
                {
                    int targetX = currentX + (rule.XDirection * distance);
                    int targetY = currentY + (rule.YDirection * distance);

                    // Check if target is within board bounds
                    if (
                        targetX < 0
                        || targetX >= this.board.Width
                        || targetY < 0
                        || targetY >= this.board.Height
                    )
                    {
                        break;
                    }

                    string targetCoord = ChessBoard.GetCoordinateFromPosition(targetX, targetY);
                    ChessTile targetTile = this.board.GetTile(targetCoord);

                    if (targetTile == null)
                    {
                        break;
                    }

                    // If there's a piece on this tile
                    if (targetTile.CurrentPiece != null)
                    {
                        // If it's an enemy piece, we can capture it
                        if (targetTile.CurrentPiece.IsWhite != this.IsWhite)
                        {
                            validMoves.Add(targetTile);
                        }

                        // If we can't jump over pieces, stop checking this direction
                        if (!rule.CanJumpOverPieces)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // Empty tile, valid move
                        validMoves.Add(targetTile);
                    }
                }
            }

            // Special handling for pawns (capturing diagonally)
            // TODO: fix only diagonal capture for pawns not straight
            if (this.PieceType != ChessPieceType.Pawn)
            {
                return validMoves;
            }

            // Diagonal captures
            int[] captureDirections = { -1, 1 };
            int forwardDirection = this.IsWhite ? 1 : -1;

            foreach (int dir in captureDirections)
            {
                int targetX = currentX + dir;
                int targetY = currentY + forwardDirection;

                if (
                    targetX < 0
                    || targetX >= this.board.Width
                    || targetY < 0
                    || targetY >= this.board.Height
                )
                {
                    continue;
                }

                string targetCoord = ChessBoard.GetCoordinateFromPosition(targetX, targetY);
                ChessTile targetTile = this.board.GetTile(targetCoord);

                if (
                    targetTile != null
                    && targetTile.CurrentPiece != null
                    && targetTile.CurrentPiece.IsWhite != this.IsWhite
                )
                {
                    validMoves.Add(targetTile);
                }
            }

            // First move can be 2 squares forward
            if (
                (!this.IsWhite || currentY != 1)
                && (this.IsWhite || currentY != this.board.Height - 2)
            )
            {
                return validMoves;
            }

            int twoForward = currentY + (forwardDirection * 2);
            if (twoForward < 0 || twoForward >= this.board.Height)
            {
                return validMoves;
            }

            string oneStepCoord = ChessBoard.GetCoordinateFromPosition(
                currentX,
                currentY + forwardDirection
            );
            string twoStepCoord = ChessBoard.GetCoordinateFromPosition(currentX, twoForward);

            ChessTile oneStepTile = this.board.GetTile(oneStepCoord);
            ChessTile twoStepTile = this.board.GetTile(twoStepCoord);

            if (
                oneStepTile != null
                && oneStepTile.CurrentPiece == null
                && twoStepTile != null
                && twoStepTile.CurrentPiece == null
            )
            {
                validMoves.Add(twoStepTile);
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
