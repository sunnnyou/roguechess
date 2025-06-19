namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuiltDifferentBuff", menuName = "Game/Buffs/BuiltDifferentBuff")]
    public class BuiltDifferentBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

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
