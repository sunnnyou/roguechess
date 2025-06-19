namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StonksBuff", menuName = "Game/Buffs/StonksBuff")]
    public class StonksBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public StonksBuff()
        {
            this.UpdateFunction = this.StonksFnc;
        }

        public IChessObject StonksFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Stonks buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
