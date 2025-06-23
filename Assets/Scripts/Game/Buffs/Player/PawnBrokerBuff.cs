namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PawnBrokerBuff", menuName = "Game/Buffs/PawnBrokerBuff")]
    public class PawnBrokerBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public PawnBrokerBuff()
        {
            this.UpdateFunction = this.PawnBrokerFnc;
        }

        public IChessObject PawnBrokerFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for PawnBroker buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
