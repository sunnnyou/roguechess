namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ChessEnemyGenerator : MonoBehaviour
    {
        private static readonly char[] Files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private static readonly int[] Ranks = { 6, 7, 8 };

        public static List<EnemyPiece> GenerateEnemyLayout(int level)
        {
            int minPieces = Mathf.Min(16, 5 + ((level - 30) / 3)); // Caps at 16
            var layout = new List<EnemyPiece>();
            var occupied = new HashSet<string>();

            // Always place the King first
            string kingPos = GetUniqueRandomPosition(occupied);
            layout.Add(new EnemyPiece(ChessPieceType.King, kingPos));

            int remaining = minPieces - 1;

            while (remaining > 0)
            {
                var type = GetWeightedPieceType(level);
                string pos = GetUniqueRandomPosition(occupied);

                layout.Add(new EnemyPiece(type, pos));
                remaining--;
            }

            return layout;
        }

        private static ChessPieceType GetWeightedPieceType(int level)
        {
            float roll = Random.value;
            float queenChance = Mathf.Clamp01((level - 30) * 0.01f); // Up to 70% by level 100
            float rookChance = Mathf.Clamp01((level - 30) * 0.008f);
            float bishopChance = 0.15f;
            float knightChance = 0.15f;
            float pawnChance = 1.0f - queenChance - rookChance - bishopChance - knightChance;

            if (roll < queenChance)
            {
                return ChessPieceType.Queen;
            }

            if (roll < queenChance + rookChance)
            {
                return ChessPieceType.Rook;
            }

            if (roll < queenChance + rookChance + bishopChance)
            {
                return ChessPieceType.Bishop;
            }

            if (roll < queenChance + rookChance + bishopChance + knightChance)
            {
                return ChessPieceType.Knight;
            }

            return ChessPieceType.Pawn;
        }

        private static string GetUniqueRandomPosition(HashSet<string> occupied)
        {
            string pos;
            do
            {
                char file = Files[Random.Range(0, Files.Length)];
                int rank = Ranks[Random.Range(0, Ranks.Length)];
                pos = $"{file}{rank}";
            } while (occupied.Contains(pos));

            occupied.Add(pos);
            return pos;
        }

        // Example usage:
        // var layout = ChessEnemyGenerator.GenerateEnemyLayout(35);
        // foreach (var piece in layout) Debug.Log(piece);
    }
}
