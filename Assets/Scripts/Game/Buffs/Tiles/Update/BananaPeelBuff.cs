namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BananaPeelBuff", menuName = "Game/Buffs/BananaPeelBuff")]
    public class BananaPeelBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public BananaPeelBuff()
        {
            this.UpdateFunction = this.BananaPeelFnc;
        }

        public IChessObject BananaPeelFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for BananaPeel buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
