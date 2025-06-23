namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AIShieldBuff", menuName = "Game/Buffs/AIShieldBuff")]
    public class AIShieldBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public AIShieldBuff()
        {
            this.UpdateFunction = this.AIShieldFnc;
        }

        public IChessObject AIShieldFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for AIShield buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
