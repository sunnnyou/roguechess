namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnPassantTileBuff", menuName = "Game/Buffs/EnPassantTileBuff")]
    public class EnPassantTileBuff : UpdateBuff
    {
        public string BuffName { get; set; } = "En Passant Debuff";

        public string Description { get; set; } = "Makes a piece susceptible to en passant";

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public EnPassantTileBuff()
        {
            this.UpdateFunction = EnPassantDebuff;
        }

        public static IChessObject EnPassantDebuff(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Royal Ascension buff.");
                return null;
            }

            var currentPos = piece.CurrentTile.Position;
            var behindY = piece.IsWhite ? currentPos.y - 1 : currentPos.y + 1;
            var behindPos = new Vector2Int(currentPos.x, behindY);

            if (
                board.GetTile(behindPos, out ChessTile behindTile)
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
