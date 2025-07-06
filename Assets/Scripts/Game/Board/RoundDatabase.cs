namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyRoundDatabase : MonoBehaviour
    {
#pragma warning disable SA1118
        public static List<EnemyRound> Rounds = new()
        {
            new EnemyRound(
                "The Siege Line",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "g7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "e7"),
                }
            ),
            new EnemyRound(
                "Diagonal Doom",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "f8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f7"),
                    new EnemyPiece(ChessPieceType.Knight, "g6"),
                    new EnemyPiece(ChessPieceType.Pawn, "b6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d7"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                }
            ),
            new EnemyRound(
                "Cavalry Charge",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Knight, "b8"),
                    new EnemyPiece(ChessPieceType.Knight, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "g6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                }
            ),
            new EnemyRound(
                "Rook Fortress",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "Queen's Entry",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Pawn, "b6"),
                    new EnemyPiece(ChessPieceType.Pawn, "c7"),
                    new EnemyPiece(ChessPieceType.Pawn, "e7"),
                    new EnemyPiece(ChessPieceType.Pawn, "g6"),
                }
            ),
            new EnemyRound(
                "Domino Breakers",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Knight, "g7"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "The Crescent",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c7"),
                    new EnemyPiece(ChessPieceType.Knight, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a6"),
                    new EnemyPiece(ChessPieceType.Pawn, "b7"),
                    new EnemyPiece(ChessPieceType.Pawn, "e7"),
                    new EnemyPiece(ChessPieceType.Pawn, "g6"),
                }
            ),
            new EnemyRound(
                "Skyhook",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Knight, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "g7"),
                }
            ),
            new EnemyRound(
                "Trap Door",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "f7"),
                    new EnemyPiece(ChessPieceType.Knight, "c6"),
                    new EnemyPiece(ChessPieceType.Knight, "g8"),
                    new EnemyPiece(ChessPieceType.Pawn, "b6"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                    new EnemyPiece(ChessPieceType.Pawn, "h7"),
                }
            ),
            new EnemyRound(
                "Lurkers",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c7"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "b8"),
                    new EnemyPiece(ChessPieceType.Knight, "g6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f6"),
                }
            ),
            new EnemyRound(
                "Royal Guard",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c7"),
                    new EnemyPiece(ChessPieceType.Knight, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a6"),
                    new EnemyPiece(ChessPieceType.Pawn, "b7"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                    new EnemyPiece(ChessPieceType.Pawn, "g7"),
                }
            ),
            new EnemyRound(
                "Scorched Field",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "f8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h7"),
                    new EnemyPiece(ChessPieceType.Knight, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "g6"),
                }
            ),
            new EnemyRound(
                "Knights of Ruin",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Knight, "b6"),
                    new EnemyPiece(ChessPieceType.Knight, "c8"),
                    new EnemyPiece(ChessPieceType.Knight, "f6"),
                    new EnemyPiece(ChessPieceType.Knight, "g8"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "b6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "Linebreakers",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c7"),
                    new EnemyPiece(ChessPieceType.Bishop, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "a6"),
                    new EnemyPiece(ChessPieceType.Pawn, "b7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "Queen's Wrath",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Queen, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "The Swarm",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "g7"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "b7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "e6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                }
            ),
            new EnemyRound(
                "The Full Court",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "b8"),
                    new EnemyPiece(ChessPieceType.Knight, "g8"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c7"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "Twin Queens",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Queen, "e7"),
                    new EnemyPiece(ChessPieceType.Pawn, "b6"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "g6"),
                    new EnemyPiece(ChessPieceType.Pawn, "g7"),
                }
            ),
            new EnemyRound(
                "The Iron Mask",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "b6"),
                    new EnemyPiece(ChessPieceType.Knight, "g7"),
                    new EnemyPiece(ChessPieceType.Pawn, "a6"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d7"),
                    new EnemyPiece(ChessPieceType.Pawn, "f6"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
            new EnemyRound(
                "Final Boss: Dread Throne",
                new[]
                {
                    new EnemyPiece(ChessPieceType.King, "e8"),
                    new EnemyPiece(ChessPieceType.Queen, "d8"),
                    new EnemyPiece(ChessPieceType.Queen, "e7"),
                    new EnemyPiece(ChessPieceType.Rook, "a8"),
                    new EnemyPiece(ChessPieceType.Rook, "h8"),
                    new EnemyPiece(ChessPieceType.Bishop, "c8"),
                    new EnemyPiece(ChessPieceType.Bishop, "f8"),
                    new EnemyPiece(ChessPieceType.Knight, "b6"),
                    new EnemyPiece(ChessPieceType.Knight, "g6"),
                    new EnemyPiece(ChessPieceType.Pawn, "a7"),
                    new EnemyPiece(ChessPieceType.Pawn, "b7"),
                    new EnemyPiece(ChessPieceType.Pawn, "c6"),
                    new EnemyPiece(ChessPieceType.Pawn, "d6"),
                    new EnemyPiece(ChessPieceType.Pawn, "f7"),
                    new EnemyPiece(ChessPieceType.Pawn, "g7"),
                    new EnemyPiece(ChessPieceType.Pawn, "h6"),
                }
            ),
        };
#pragma warning restore SA1118

        public static EnemyRound GetRound(int round)
        {
            if (round < 1)
            {
                return Rounds[0];
            }
            else if (round > Rounds.Count)
            {
                return new EnemyRound(
                    string.Empty,
                    ChessEnemyGenerator.GenerateEnemyLayout(round).ToArray()
                );
            }
            else
            {
                return Rounds[round - 1];
            }
        }
    }
}
