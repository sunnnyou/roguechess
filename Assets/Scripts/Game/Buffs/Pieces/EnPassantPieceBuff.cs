namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class EnPassantPieceBuff : MoveBuff
    {
        public override string BuffName { get; set; } = "En Passant";

        public override string Description { get; set; } =
            "Gives a piece the ability to use en passant, while also making itself susceptible to it.";

        public PawnBuff()
        {
            // TODO add function for reaching the end of the board
            // and turning into a queen or other piece
            this.MoveFunctions = new List<Func<object[], object>> { DoubleMove, GetEnPassantTiles };
        }

        public static List<ChessTile> GetEnPassantTiles(params object[] args)
        {
            if (
                args == null
                || args.Length != 4
                || args[0] is not int currentX
                || args[1] is not int currentY
                || args[2] is not ChessBoard board
                || args[3] is not bool isWhite
            )
            {
                Debug.LogError(
                    "Invalid arguments for GetEnPassantTiles. Expected int, int, ChessBoard, bool."
                );
                return null;
            }

            var validTiles = new List<ChessTile>();

            // Check if there are any moves in history
            if (board == null || board.MoveHistory == null || board.MoveHistory.Count == 0)
            {
                return validTiles;
            }

            // Get last move
            var lastMove = board.MoveHistory[board.MoveHistory.Count - 1];

            // Check if last move was a pawn move
            if (lastMove.MovedPiece.PieceType != ChessPieceType.Pawn)
            {
                return validTiles;
            }

            // Check if it was opponent's move
            if (lastMove.WasWhiteTurn == isWhite)
            {
                return validTiles;
            }

            // Calculate ranks for the last move
            int fromY = ChessBoard.GetPositionFromCoordinate(lastMove.FromCoordinate).y;
            int toY = ChessBoard.GetPositionFromCoordinate(lastMove.ToCoordinate).y;

            // Check if it was a double pawn push
            if (Math.Abs(toY - fromY) != 2)
            {
                return validTiles;
            }

            // Calculate correct rank for en passant capture
            int correctRank = isWhite ? board.Height - 4 : 3;

            // Check if capturing pawn is in correct position
            if (currentY != correctRank)
            {
                return validTiles;
            }

            // Get last moved pawn's position
            int lastMovedPawnX = ChessBoard.GetPositionFromCoordinate(lastMove.ToCoordinate).x;

            // Check if pawns are adjacent
            if (Math.Abs(currentX - lastMovedPawnX) == 1)
            {
                int targetY = isWhite ? correctRank + 1 : correctRank - 1;
                string targetCoord = ChessBoard.GetCoordinateFromPosition(lastMovedPawnX, targetY);
                ChessTile targetTile = board.GetTile(targetCoord);

                if (targetTile != null)
                {
                    validTiles.Add(targetTile);
                    targetTile.Buffs.Add(
                        new TileEnPassantBuff
                        {
                            TargetX = currentX,
                            TargetY = currentY,
                            IsWhite = isWhite,
                        }
                    );
                }
            }

            return validTiles;
        }

        public static List<ChessTile> DoubleMove(params object[] args)
        {
            if (
                args == null
                || args.Length != 4
                || args[0] is not int currentX
                || args[1] is not int currentY
                || args[2] is not ChessBoard board
                || args[3] is not bool isWhite
            )
            {
                Debug.LogError(
                    "Invalid arguments for DoubleMove. Expected int, int, ChessBoard, bool."
                );
                return null;
            }

            var validMoves = new List<ChessTile>();
            if (
                board == null
                || ((!isWhite || currentY != 1) && (isWhite || currentY != board.Height - 2))
            )
            {
                return validMoves;
            }

            int forwardDirection = isWhite ? 1 : -1;
            int twoForward = currentY + (forwardDirection * 2);
            if (twoForward < 0 || twoForward >= board.Height)
            {
                return validMoves;
            }

            string oneStepCoord = ChessBoard.GetCoordinateFromPosition(
                currentX,
                currentY + forwardDirection
            );
            string twoStepCoord = ChessBoard.GetCoordinateFromPosition(currentX, twoForward);

            ChessTile oneStepTile = board.GetTile(oneStepCoord);
            ChessTile twoStepTile = board.GetTile(twoStepCoord);

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
    }
}
