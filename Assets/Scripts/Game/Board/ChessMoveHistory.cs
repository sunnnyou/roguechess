namespace Assets.Scripts.Game.Board
{
    public class ChessMoveHistory
    {
        public ChessPiece MovedPiece { get; private set; }

        public string FromCoordinate { get; private set; }

        public string ToCoordinate { get; private set; }

        public ChessPiece CapturedPiece { get; private set; }

        public bool WasWhiteTurn { get; private set; }

        public ChessMoveHistory(
            ChessPiece movedPiece,
            string fromCoord,
            string toCoord,
            ChessPiece capturedPiece,
            bool wasWhiteTurn
        )
        {
            this.MovedPiece = movedPiece;
            this.FromCoordinate = fromCoord;
            this.ToCoordinate = toCoord;
            this.CapturedPiece = capturedPiece;
            this.WasWhiteTurn = wasWhiteTurn;
        }
    }
}
