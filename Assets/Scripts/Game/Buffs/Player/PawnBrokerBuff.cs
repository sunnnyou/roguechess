namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PawnBrokerBuff", menuName = "Game/Buffs/PawnBrokerBuff")]
    public class PawnBrokerBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public PawnBrokerBuff()
        {
            this.UpdateFunction = this.PawnBrokerFnc;
        }

        public IChessObject PawnBrokerFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for PawnBroker buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
