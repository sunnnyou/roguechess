namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs.Tiles.Update;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnPassantPieceBuff", menuName = "Game/Buffs/EnPassantPieceBuff")]
    public class EnPassantPieceBuff : MoveBuff
    {
        public new string BuffName = "EnPassantPieceBuff";

        public EnPassantPieceBuff()
        {
            this.MoveFunction = GetEnPassantTiles;
        }

        public static List<ChessTile> GetEnPassantTiles(ChessPiece piece)
        {
            var validTiles = new List<ChessTile>();

            if (piece == null)
            {
                return validTiles;
            }

            var isWhite = piece.IsWhite;
            var currentPos = piece.CurrentTile.Position;

            // Check if there are any moves in history
            if (
                ChessBoard.Instance.MoveHistory == null
                || ChessBoard.Instance.MoveHistory.Count == 0
            )
            {
                return validTiles;
            }

            // Get last move
            var lastMove = ChessBoard.Instance.MoveHistory[^1];

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

            if (
                lastMove.ToPosition is not Vector2Int toPos
                || lastMove.FromPosition is not Vector2Int fromPos
            )
            {
                return validTiles;
            }

            // Check if it was a double pawn push
            if (Math.Abs(toPos.y - fromPos.y) != 2)
            {
                return validTiles;
            }

            // Calculate correct rank for en passant capture
            int correctRank = isWhite ? ChessBoard.Instance.Height - 4 : 3;

            // Check if capturing pawn is in correct position
            if (currentPos.y != correctRank)
            {
                return validTiles;
            }

            // Get last moved pawn's position
            int lastMovedPawnX = toPos.x;

            // Check if pawns are adjacent
            if (Math.Abs(currentPos.x - lastMovedPawnX) == 1)
            {
                int targetY = isWhite ? correctRank + 1 : correctRank - 1;
                string targetCoord = CoordinateHelper.XYToString(lastMovedPawnX, targetY);

                if (ChessBoard.Instance.GetTile(targetCoord, out ChessTile targetTile))
                {
                    validTiles.Add(targetTile);
                    targetTile.AddBuff(new EnPassantTileBuff());
                }
            }

            return validTiles;
        }
    }
}
