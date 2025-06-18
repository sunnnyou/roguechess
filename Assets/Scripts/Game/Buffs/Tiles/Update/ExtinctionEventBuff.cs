namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExtinctionEventBuff", menuName = "Game/Buffs/ExtinctionEventBuff")]
    public class ExtinctionEventBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

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
