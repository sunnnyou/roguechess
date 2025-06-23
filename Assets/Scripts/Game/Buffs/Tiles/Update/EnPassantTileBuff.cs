namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnPassantTileBuff", menuName = "Game/Buffs/EnPassantTileBuff")]
    public class EnPassantTileBuff : UpdateBuff
    {
        public new string BuffName { get; set; } = "En Passant Debuff";

        public new string Description { get; set; } = "Makes a piece susceptible to en passant";

        public new Sprite Icon { get; set; }

        public new int Cost { get; set; }

        public new bool WasUsed { get; set; }

        public EnPassantTileBuff()
        {
            this.UpdateFunction = EnPassantDebuff;
        }

        public static IChessObject EnPassantDebuff(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Royal Ascension buff.");
                return null;
            }

            var currentPos = piece.CurrentTile.Position;
            var behindY = piece.IsWhite ? currentPos.y - 1 : currentPos.y + 1;
            var behindPos = new Vector2Int(currentPos.x, behindY);

            if (
                ChessBoard.Instance.GetTile(behindPos, out ChessTile behindTile)
                && behindTile.CurrentPiece is ChessPiece behindPiece
                && ChessPiece.FightPiece(behindPiece, piece)
            )
            {
                return behindPiece;
            }

            return null;
        }
    }
}
