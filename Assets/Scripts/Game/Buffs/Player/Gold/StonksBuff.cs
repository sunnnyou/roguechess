namespace Assets.Scripts.Game.Buffs.Player
{
    using System;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StonksBuff", menuName = "Game/Buffs/StonksBuff")]
    public class StonksBuff : GoldBuff
    {
        private readonly float increaseRate = 1.25f;

        public StonksBuff()
        {
            this.GoldFunction = this.StonksFnc;
        }

        public int StonksFnc()
        {
            return (int)((InventoryManager.Instance.Gold * this.increaseRate) + 0.5); // Round to nearest integer away from zero
        }
    }
}
