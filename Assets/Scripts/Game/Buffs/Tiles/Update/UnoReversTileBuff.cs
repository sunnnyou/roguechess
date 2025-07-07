namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "UnoReversTileBuff", menuName = "Game/Buffs/UnoReversTileBuff")]
    public class UnoReversTileBuff : UpdateBuff
    {
        private readonly ChessPiece pieceToProtect;

        public UnoReversTileBuff(ChessPiece piece)
        {
            this.pieceToProtect = piece;
            this.UpdateFunction = UnoReversTileFnc;
        }

        public static IChessObject UnoReversTileFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece newPiece || newPiece == null)
            {
                Debug.LogError("Invalid arguments for UnoReversTileFnc buff.");
                return null;
            }

            ChessBoard.Instance.UndoLastMove(true);

            newPiece.AddReduceLives(-newPiece.Strength, true);

            return null;
        }
    }
}
