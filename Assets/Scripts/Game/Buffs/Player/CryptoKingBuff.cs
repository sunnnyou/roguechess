namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CryptoKingBuff", menuName = "Game/Buffs/CryptoKingBuff")]
    public class CryptoKingBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public CryptoKingBuff()
        {
            this.UpdateFunction = this.CryptoKingFnc;
        }

        public IChessObject CryptoKingFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for CryptoKing buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
