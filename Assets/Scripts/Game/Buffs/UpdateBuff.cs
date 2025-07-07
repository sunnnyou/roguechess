namespace Assets.Scripts.Game.Buffs
{
    using System;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Update buffs update a piece or tile (disable/enable, sprite change, ...)
    public abstract class UpdateBuff : BuffBase
    {
        public Func<IChessObject, IChessObject> UpdateFunction { get; set; } // Function that is applied when a condition is met

        public new SelectionType SelectionType { get; set; }

        internal override object BuffFunction(IChessObject buffReceiver)
        {
            return this.UpdateFunction(buffReceiver);
        }
    }
}
