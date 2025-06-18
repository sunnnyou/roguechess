namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "UnoReverseBuff", menuName = "Game/Buffs/UnoReverseBuff")]
    public class UnoReverseBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public UnoReverseBuff()
        {
            this.UpdateFunction = this.UnoReverseFnc;
        }

        public IChessObject UnoReverseFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for UnoReverse buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
