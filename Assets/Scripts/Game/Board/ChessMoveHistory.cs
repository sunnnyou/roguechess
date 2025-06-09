namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ChessMoveHistory
    {
        public ChessPiece MovedPiece { get; private set; }

        public Vector2Int? FromPosition { get; private set; }

        public Vector2Int? ToPosition { get; private set; }

        public ChessPiece MainCapturedPiece { get; private set; }

        public List<ChessPiece> AdditionalCapturedPieces { get; private set; }

        public bool WasWhiteTurn { get; private set; }

        public ChessMoveHistory(
            ChessPiece movedPiece,
            Vector2Int? fromPos,
            Vector2Int? toPos,
            ChessPiece mainCapturedPiece,
            List<ChessPiece> additionalCapturedPieces,
            bool wasWhiteTurn
        )
        {
            this.MovedPiece = movedPiece;
            this.FromPosition = fromPos;
            this.ToPosition = toPos;
            this.MainCapturedPiece = mainCapturedPiece;
            this.AdditionalCapturedPieces = additionalCapturedPieces;
            this.WasWhiteTurn = wasWhiteTurn;
        }
    }
}
