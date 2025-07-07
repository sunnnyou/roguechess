namespace Assets.Scripts.Game.Buffs.Player.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SnapBuff", menuName = "Game/Buffs/SnapBuff")]
    public class SnapBuff : UpdateBuff
    {
        public SnapBuff()
        {
            this.UpdateFunction = this.SnapFnc;
        }

        public IChessObject SnapFnc(IChessObject chessObject)
        {
            var allEnemyPieces = ChessBoard.Instance.GetAllPieces(false);

            foreach (ChessPiece piece in allEnemyPieces)
            {
                if (Random.Range(0, 2) > 0)
                {
                    piece.AddReduceLives(-piece.Lives, true);
                }
            }
            return null;
        }
    }
}
