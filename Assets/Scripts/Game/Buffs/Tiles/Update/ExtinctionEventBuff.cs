namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExtinctionEventBuff", menuName = "Game/Buffs/ExtinctionEventBuff")]
    public class ExtinctionEventBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public ExtinctionEventBuff()
        {
            this.UpdateFunction = this.ExtinctionEventFnc;
        }

        public IChessObject ExtinctionEventFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for ExtinctionEvent buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
