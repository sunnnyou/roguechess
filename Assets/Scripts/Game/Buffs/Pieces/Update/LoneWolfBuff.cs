namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LoneWolfBuff", menuName = "Game/Buffs/LoneWolfBuff")]
    public class LoneWolfBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public LoneWolfBuff()
        {
            this.UpdateFunction = this.LoneWolfFnc;
        }

        public IChessObject LoneWolfFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for LoneWolf buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
