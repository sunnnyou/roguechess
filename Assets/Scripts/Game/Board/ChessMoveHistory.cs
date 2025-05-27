namespace Assets.Scripts.Game.Board
{
    using UnityEngine;

    public class ChessMoveHistory
    {
        public ChessPiece MovedPiece { get; private set; }

        public Vector2Int FromPosition { get; private set; }

        public Vector2Int ToPosition { get; private set; }

        public ChessPiece CapturedPiece { get; private set; }

        public bool WasWhiteTurn { get; private set; }

        public ChessMoveHistory(
            ChessPiece movedPiece,
            Vector2Int fromPos,
            Vector2Int toPos,
            ChessPiece capturedPiece,
            bool wasWhiteTurn
        )
        {
            this.MovedPiece = movedPiece;
            this.FromPosition = fromPos;
            this.ToPosition = toPos;
            this.CapturedPiece = capturedPiece;
            this.WasWhiteTurn = wasWhiteTurn;
        }
    }
}
