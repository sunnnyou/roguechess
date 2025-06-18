namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ThrowingHandsBuff", menuName = "Game/Buffs/ThrowingHandsBuff")]
    public class ThrowingHandsBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

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
