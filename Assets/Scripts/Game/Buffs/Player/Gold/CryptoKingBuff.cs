namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CryptoKingBuff", menuName = "Game/Buffs/CryptoKingBuff")]
    public class CryptoKingBuff : GoldBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public CryptoKingBuff()
        {
            this.GoldFunction = this.CryptoKingFnc;
        }

        public int CryptoKingFnc()
        {
            var playerPieceCountStart = InventoryManager.Instance.GetChessPieceCount();
            var playerPieceCountEnd = ChessBoard.Instance.GetAllPieces(true).Count;
            var enemyPieceCountStart = ChessBoard.Instance.GetTotalEnemyPieceCount();
            var enemyPieceCountEnd = ChessBoard.Instance.GetAllPieces(false).Count;

            if (enemyPieceCountStart == null)
            {
                return 0;
            }

            return (int)(
                enemyPieceCountStart
                - enemyPieceCountEnd
                - playerPieceCountStart
                + playerPieceCountEnd
            );
        }
    }
}
