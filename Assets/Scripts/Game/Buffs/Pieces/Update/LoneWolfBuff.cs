namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LoneWolfBuff", menuName = "Game/Buffs/LoneWolfBuff")]
    public class LoneWolfBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public LoneWolfBuff()
        {
            this.UpdateFunction = this.LoneWolfFnc;
        }

        public IChessObject LoneWolfFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for LoneWolf buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
