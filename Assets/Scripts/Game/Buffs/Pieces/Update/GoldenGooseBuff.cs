namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GoldenGooseBuff", menuName = "Game/Buffs/GoldenGooseBuff")]
    public class GoldenGooseBuff : UpdateBuff
    {
        private int roundUsed = -1;
        private int goldGained = 5;

        public GoldenGooseBuff()
        {
            this.UpdateFunction = this.GoldenGooseFnc;
        }

        public IChessObject GoldenGooseFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for GoldenGoose buff.");
                return null;
            }

            if (this.roundUsed != ChessBoard.Instance.CurrentRound && piece.Lives == 0)
            {
                InventoryManager.Instance.AddGold(this.goldGained);
                this.roundUsed = ChessBoard.Instance.CurrentRound;
            }

            return piece;
        }
    }
}
