namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PawnBrokerBuff", menuName = "Game/Buffs/PawnBrokerBuff")]
    public class PawnBrokerBuff : GoldBuff
    {
        private readonly int extraGold = 3;

        public PawnBrokerBuff()
        {
            this.GoldFunction = this.PawnBrokerFnc;
        }

        public int PawnBrokerFnc()
        {
            return this.extraGold;
        }
    }
}
