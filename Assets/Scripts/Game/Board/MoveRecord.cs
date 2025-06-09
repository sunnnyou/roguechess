namespace Assets.Scripts.Game.Board
{
    // Move record to store all information needed to undo a move
    [System.Serializable]
    public class MoveRecord
    {
        [System.NonSerialized]
        public ChessPiece MovedPiece;

        [System.NonSerialized]
        public ChessTile FromTile;

        [System.NonSerialized]
        public ChessTile ToTile;

        [System.NonSerialized]
        public ChessPiece CapturedPiece; // null if no capture
        public bool WasWhiteTurn;
        public bool WasFirstMove; // for pawn/king/rook special moves

        public MoveRecord(
            ChessPiece movedPiece,
            ChessTile from,
            ChessTile to,
            ChessPiece captured,
            bool wasWhiteTurn,
            bool wasFirstMove
        )
        {
            this.MovedPiece = movedPiece;
            this.FromTile = from;
            this.ToTile = to;
            this.CapturedPiece = captured;
            this.WasWhiteTurn = wasWhiteTurn;
            this.WasFirstMove = wasFirstMove;
        }
    }
}
