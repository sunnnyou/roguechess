namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuiltDifferentBuff", menuName = "Game/Buffs/BuiltDifferentBuff")]
    public class BuiltDifferentBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public BuiltDifferentBuff()
        {
            this.UpdateFunction = this.BuiltDifferentFnc;
        }

        public IChessObject BuiltDifferentFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for BuiltDifferent buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
