namespace Assets.Scripts.Game.Buffs
{
    using System;
    using Assets.Scripts.Game.Board;

    // Gold modification buffs that are triggered at the end of the round
    public abstract class GoldBuff : BuffBase
    {
        public Func<int> GoldFunction { get; set; } // Function that is applied when a condition is met

        public new SelectionType SelectionType { get; set; }

        internal override object BuffFunction(IChessObject buffReceiver)
        {
            return this.GoldFunction();
        }
    }
}
