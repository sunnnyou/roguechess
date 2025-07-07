namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZombieBuff", menuName = "Game/Buffs/ZombieBuff")]
    public class ZombieBuff : UpdateBuff
    {
        private int roundUsed = -1;
        private readonly int healAmount = 1;

        public ZombieBuff()
        {
            this.UpdateFunction = this.ZombieFnc;
        }

        public IChessObject ZombieFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Zombie buff.");
                return null;
            }
            if (this.roundUsed != ChessBoard.Instance.CurrentRound && piece.Lives == 0)
            {
                piece.AddReduceStrength(this.healAmount, true);
                this.roundUsed = ChessBoard.Instance.CurrentRound;
            }

            return piece;
        }
    }
}
