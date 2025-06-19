namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ThrowingHandsBuff", menuName = "Game/Buffs/ThrowingHandsBuff")]
    public class ThrowingHandsBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public ThrowingHandsBuff()
        {
            this.UpdateFunction = this.ThrowingHandsFnc;
        }

        public IChessObject ThrowingHandsFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for ThrowingHands buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
