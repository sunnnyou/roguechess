namespace Assets.Scripts.Game.Board
{
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
    }
}
