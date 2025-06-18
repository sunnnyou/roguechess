namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AIShieldBuff", menuName = "Game/Buffs/AIShieldBuff")]
    public class AIShieldBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public AIShieldBuff()
        {
            this.UpdateFunction = this.AIShieldFnc;
        }

        public IChessObject AIShieldFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for AIShield buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
