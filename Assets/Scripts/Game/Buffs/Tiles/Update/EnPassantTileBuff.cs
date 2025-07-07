namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnPassantTileBuff", menuName = "Game/Buffs/EnPassantTileBuff")]
    public class EnPassantTileBuff : UpdateBuff
    {
        public EnPassantTileBuff()
        {
            this.UpdateFunction = this.EnPassantDebuff;
        }

        public IChessObject EnPassantDebuff(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for EnPassantDebuff buff.");
                return null;
            }

            var currentPos = piece.CurrentTile.Position;
            var behindY = piece.IsWhite ? currentPos.y - 1 : currentPos.y + 1;
            var behindPos = new Vector2Int(currentPos.x, behindY);

            if (
                ChessBoard.Instance.GetTile(behindPos, out ChessTile behindTile)
                && behindTile.CurrentPiece is ChessPiece behindPiece
            )
            {
                this.WasUsed = true;
                if (ChessPiece.FightPiece(behindPiece, piece, true))
                {
                    return behindPiece;
                }
            }

            return null;
        }
    }
}
