namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using UnityEngine;

    // Helper class for position-related calculations
    public static class CoordinateHelper
    {
        // Utility method to convert from grid position to chess notation
        public static string XYToString(int x, int y)
        {
            char file = (char)('a' + x);
            int rank = y + 1;
            return $"{file}{rank}";
        }

        public static string VectorToString(Vector2Int position)
        {
            return XYToString(position.x, position.y);
        }

        // Utility method to parse chess notation into grid position
        public static void StringToXY(string coordinate, out int x, out int y)
        {
            if (string.IsNullOrEmpty(coordinate) || coordinate.Length < 2)
            {
                x = -1;
                y = -1;
                return;
            }

            char file = coordinate[0];
            x = file - 'a';

            string rankStr = coordinate.Substring(1);
            if (int.TryParse(rankStr, out int rank))
            {
                y = rank - 1;
            }
            else
            {
                y = -1;
            }
        }

        public static Vector2Int StringToVector(string coordinate)
        {
            StringToXY(coordinate, out int x, out int y);
            return new Vector2Int(x, y);
        }

        public static Vector2Int XYToVector(int x, int y)
        {
            return new Vector2Int(x, y);
        }

        public static List<(int x, int y)> GetSurroundingCoordinatesWithBounds(
            int x,
            int y,
            int minX = 0,
            int minY = 0,
            int? maxX = null,
            int? maxY = null
        )
        {
            maxX ??= ChessBoard.Instance.Height - 1;
            maxY ??= ChessBoard.Instance.Height - 1;

            var coordinates = new List<(int, int)>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // Skip the center point
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int newX = x + dx;
                    int newY = y + dy;

                    // Check bounds
                    if (newX >= minX && newX <= maxX && newY >= minY && newY <= maxY)
                    {
                        coordinates.Add((newX, newY));
                    }
                }
            }

            return coordinates;
        }
    }
}
