namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuiltDifferentBuff", menuName = "Game/Buffs/BuiltDifferentBuff")]
    public class BuiltDifferentBuff : UpdateBuff
    {
        public BuiltDifferentBuff()
        {
            this.UpdateFunction = this.BuiltDifferentFnc;
        }

        public IChessObject BuiltDifferentFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for BuiltDifferent buff.");
                return null;
            }

            piece.AddReduceLives(piece.Lives, true); // double hp

            this.IsActive = false;

            return piece;
        }
    }
}
