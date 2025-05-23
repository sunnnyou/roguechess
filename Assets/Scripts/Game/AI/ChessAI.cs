namespace Assets.Scripts.Game.AI
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class ChessAI
    {
        private ChessBoard board;
        private bool isWhiteAI;
        private int searchDepth;
        private const int PAWN_VALUE = 100;
        private const int KNIGHT_VALUE = 320;
        private const int BISHOP_VALUE = 330;
        private const int ROOK_VALUE = 500;
        private const int QUEEN_VALUE = 900;
        private const int KING_VALUE = 20000;

        public ChessAI(ChessBoard board, bool isWhiteAI, int searchDepth)
        {
            this.board = board;
            this.isWhiteAI = isWhiteAI;
            this.searchDepth = searchDepth;
        }

        public void MakeMove()
        {
            var bestMove = this.FindBestMove();
            if (bestMove.piece != null && bestMove.move != null)
            {
                this.board.MovePieceAI(bestMove.piece, bestMove.move);
            }
        }

        public (ChessPiece piece, ChessMove move) FindBestMove()
        {
            var pieces = this.board.GetAllPieces(this.isWhiteAI);
            ChessPiece bestPiece = null;
            ChessMove bestMove = null;
            int bestValue = int.MinValue;

            foreach (var piece in pieces)
            {
                foreach (var move in piece.GetValidMoves())
                {
                    // Create a copy of the board for simulation
                    var boardCopy = ChessBoard.Instantiate(this.board);

                    // Find the corresponding piece in the copied board
                    var pieceCopy = FindCorrespondingPiece(boardCopy, piece);

                    if (pieceCopy != null)
                    {
                        // Create a corresponding move for the copied piece
                        var moveCopy = new ChessMove(
                            move.StartPosition,
                            move.TargetPosition,
                            move.IsCapture
                        );

                        // Simulate move on the copy
                        boardCopy.MovePieceAI(pieceCopy, moveCopy);

                        // Evaluate position on the copy
                        int value = EvaluatePosition(boardCopy);

                        // Adjust value based on AI color
                        if (!this.isWhiteAI)
                        {
                            value = -value;
                        }

                        if (value > bestValue)
                        {
                            bestValue = value;
                            bestPiece = piece;
                            bestMove = move;
                        }
                    }
                }
            }

            return (bestPiece, bestMove);
        }

        private static ChessPiece FindCorrespondingPiece(
            ChessBoard boardCopy,
            ChessPiece originalPiece
        )
        {
            // Find the piece in the copied board that corresponds to the original piece
            // This assumes pieces can be identified by their position and type
            var allPieces = boardCopy.GetAllPieces(originalPiece.IsWhite);

            foreach (var piece in allPieces)
            {
                var position = ChessBoard.GetPositionFromCoordinate(piece.CurrentTile.Coordinate);
                var originalPosition = ChessBoard.GetPositionFromCoordinate(
                    originalPiece.CurrentTile.Coordinate
                );
                if (
                    position.x == originalPosition.x
                    && position.y == originalPosition.y
                    && piece.PieceType == originalPiece.PieceType
                    && piece.IsWhite == originalPiece.IsWhite
                )
                {
                    return piece;
                }
            }

            return null;
        }

        public int EvaluatePosition()
        {
            return EvaluatePosition(this.board);
        }

        public static int EvaluatePosition(ChessBoard boardToEvaluate)
        {
            int score = 0;

            // Material value for white pieces
            foreach (var piece in boardToEvaluate.GetAllPieces(true))
            {
                score += GetPieceValue(piece);
            }

            // Material value for black pieces
            foreach (var piece in boardToEvaluate.GetAllPieces(false))
            {
                score -= GetPieceValue(piece);
            }

            return score;
        }

        public static int GetPieceValue(ChessPiece piece)
        {
            if (piece == null)
            {
                throw new System.ArgumentNullException(nameof(piece));
            }

            switch (piece.PieceType)
            {
                case ChessPieceType.Pawn:
                    return PAWN_VALUE;
                case ChessPieceType.Knight:
                    return KNIGHT_VALUE;
                case ChessPieceType.Bishop:
                    return BISHOP_VALUE;
                case ChessPieceType.Rook:
                    return ROOK_VALUE;
                case ChessPieceType.Queen:
                    return QUEEN_VALUE;
                case ChessPieceType.King:
                    return KING_VALUE;
                default:
                    return 0;
            }
        }
    }
}
