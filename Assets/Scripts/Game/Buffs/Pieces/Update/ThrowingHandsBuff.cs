namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ThrowingHandsBuff", menuName = "Game/Buffs/ThrowingHandsBuff")]
    public class ThrowingHandsBuff : UpdateBuff
    {
        private readonly int addedStrengthAmount = 1;

        public ThrowingHandsBuff()
        {
            this.UpdateFunction = this.ThrowingHandsFnc;
        }

        public IChessObject ThrowingHandsFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for ThrowingHands buff.");
                return null;
            }

            piece.AddReduceStrength(this.addedStrengthAmount, true);

            this.IsActive = false;

            return piece;
        }
    }
}
