namespace Assets.Scripts.Game.Buffs
{
    using System;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Update buffs update a piece or tile (disable/enable, sprite change, ...)
    public abstract class UpdateBuff : BuffBase
    {
        public Func<IChessObject, ChessBoard, IChessObject> UpdateFunction { get; set; } // Function that is applied when a condition is met

        public bool IsActive { get; set; } = true;

        public int DurationMoves { get; set; } = -1;

        public int DurationTurns { get; set; } = -1;

        public int DurationRounds { get; set; } = -1;

        public override object BuffFunction(IChessObject buffReceiver, ChessBoard board)
        {
            return this.UpdateFunction(buffReceiver, board);
        }
    }
}
