namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public abstract class PieceMoveBuff : IBuff
    {
        public Func<int, int, ChessBoard, bool, List<ChessTile>> MoveFunction { get; set; } // Function that is applied when a condition is met

        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public bool IsActive { get; set; }

        public int Cost { get; set; }

        public int DurationTurns { get; set; }

        public int DurationRounds { get; set; }

        public bool WasUsed { get; set; }

        public object ApplyBuff(params object[] args)
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
                    "Invalid arguments for Movement buff. Expected int, int, ChessBoard, bool."
                );
                return new List<ChessTile>();
            }

            return this.MoveBuffFunction(currentX, currentY, board, isWhite);
        }
    }
}
