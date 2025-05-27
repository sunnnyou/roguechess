namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class EnPassantPieceBuff : PieceMoveBuff
    {
        public override string BuffName { get; set; } = "En Passant";

        public override string Description { get; set; } =
            "Gives a piece the ability to use en passant, while also making itself susceptible to it.";

        public override Sprite Icon { get; set; }

        public override int Cost { get; set; }

        public override bool WasUsed { get; set; }

        public EnPassantPieceBuff()
        {
            this.MoveFunction = GetEnPassantTiles;
        }

        public static List<ChessTile> GetEnPassantTiles(
            Vector2Int currentPos,
            ChessBoard board,
            bool isWhite
        )
        {
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

            // Check if it was a double pawn push
            if (Math.Abs(lastMove.ToPosition.y - lastMove.FromPosition.y) != 2)
            {
                return validTiles;
            }

            // Calculate correct rank for en passant capture
            int correctRank = isWhite ? board.Height - 4 : 3;

            // Check if capturing pawn is in correct position
            if (currentPos.y != correctRank)
            {
                return validTiles;
            }

            // Get last moved pawn's position
            int lastMovedPawnX = lastMove.ToPosition.x;

            // Check if pawns are adjacent
            if (Math.Abs(currentPos.x - lastMovedPawnX) == 1)
            {
                int targetY = isWhite ? correctRank + 1 : correctRank - 1;
                string targetCoord = CoordinateHelper.XYToString(lastMovedPawnX, targetY);
                ChessTile targetTile = board.GetTile(targetCoord);

                if (targetTile != null)
                {
                    validTiles.Add(targetTile);

                    // TODO: Add buff to tile that allows en passant capture
                }
            }

            return validTiles;
        }
    }
}
