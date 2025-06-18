namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CryptoKingBuff", menuName = "Game/Buffs/CryptoKingBuff")]
    public class CryptoKingBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }
        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

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
