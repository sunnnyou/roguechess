namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Defines how pieces can move
    [System.Serializable]
    public class MoveRule
    {
        public int XDirection;
        public int YDirection;
        public int MaxDistance = 1;
        public bool CanJumpOverPieces;
        public bool MustCapture; // For pieces that can only move when capturing
        public bool CannotCapture; // For pieces that cannot capture (like pawn forward movement)

        public static List<MoveRule> GetMoveRules()
        {
            return new List<MoveRule>();
        }

        public static List<ChessTile> GetValidTiles(
            List<MoveRule> moveRules,
            Vector2Int currentPosition,
            ChessBoard board,
            bool isWhite
        )
        {
            var validTiles = new List<ChessTile>();

            if (board == null || moveRules == null || moveRules.Count == 0)
            {
                return validTiles;
            }

            foreach (MoveRule rule in moveRules)
            {
                for (int distance = 1; distance <= rule.MaxDistance; distance++)
                {
                    int targetX = currentPosition.x + (rule.XDirection * distance);
                    int targetY = currentPosition.y + (rule.YDirection * distance);

                    // Check if target is within board bounds
                    if (
                        targetX < 0
                        || targetX >= board.Width
                        || targetY < 0
                        || targetY >= board.Height
                    )
                    {
                        break;
                    }

                    string targetCoord = CoordinateHelper.XYToString(targetX, targetY);

                    if (!board.GetTile(targetCoord, out ChessTile targetTile))
                    {
                        break;
                    }

                    // If there's a piece on this tile
                    if (
                        targetTile.CurrentPiece != null
                        && targetTile.CurrentPiece.gameObject.activeSelf
                    )
                    {
                        // If it's an enemy piece, we can capture it
                        if (targetTile.CurrentPiece.IsWhite != isWhite && !rule.CannotCapture)
                        {
                            validTiles.Add(targetTile);
                        }

                        // If we can't jump over pieces, stop checking this direction
                        if (!rule.CanJumpOverPieces)
                        {
                            break;
                        }
                    }
                    else if (!rule.MustCapture)
                    {
                        // Empty tile, valid move
                        validTiles.Add(targetTile);
                    }
                }
            }

            return validTiles;
        }
    }
}
