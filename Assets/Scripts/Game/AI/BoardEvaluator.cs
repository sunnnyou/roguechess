namespace Assets.Scripts.Game.AI
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public static class BoardEvaluator
    {
        // Piece values for evaluation
        private static readonly Dictionary<ChessPieceType, float> PieceValues = new Dictionary<
            ChessPieceType,
            float
        >
        {
            { ChessPieceType.Pawn, 1.0f },
            { ChessPieceType.Knight, 3.0f },
            { ChessPieceType.Bishop, 3.0f },
            { ChessPieceType.Rook, 5.0f },
            { ChessPieceType.Queen, 9.0f },
            { ChessPieceType.King, 1000.0f },
            { ChessPieceType.Custom, 2.0f }, // Default value for custom pieces
        };

        // Position bonus tables (from white's perspective)
        private static readonly float[,] PawnPositionBonus = new float[8, 8]
        {
            { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
            { 0.1f, 0.1f, 0.2f, 0.3f, 0.3f, 0.2f, 0.1f, 0.1f },
            { 0.05f, 0.05f, 0.1f, 0.25f, 0.25f, 0.1f, 0.05f, 0.05f },
            { 0.0f, 0.0f, 0.0f, 0.2f, 0.2f, 0.0f, 0.0f, 0.0f },
            { 0.05f, -0.05f, -0.1f, 0.0f, 0.0f, -0.1f, -0.05f, 0.05f },
            { 0.05f, 0.1f, 0.1f, -0.2f, -0.2f, 0.1f, 0.1f, 0.05f },
            { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
        };

        private static readonly float[,] KnightPositionBonus = new float[8, 8]
        {
            { -0.5f, -0.4f, -0.3f, -0.3f, -0.3f, -0.3f, -0.4f, -0.5f },
            { -0.4f, -0.2f, 0.0f, 0.0f, 0.0f, 0.0f, -0.2f, -0.4f },
            { -0.3f, 0.0f, 0.1f, 0.15f, 0.15f, 0.1f, 0.0f, -0.3f },
            { -0.3f, 0.05f, 0.15f, 0.2f, 0.2f, 0.15f, 0.05f, -0.3f },
            { -0.3f, 0.0f, 0.15f, 0.2f, 0.2f, 0.15f, 0.0f, -0.3f },
            { -0.3f, 0.05f, 0.1f, 0.15f, 0.15f, 0.1f, 0.05f, -0.3f },
            { -0.4f, -0.2f, 0.0f, 0.05f, 0.05f, 0.0f, -0.2f, -0.4f },
            { -0.5f, -0.4f, -0.3f, -0.3f, -0.3f, -0.3f, -0.4f, -0.5f },
        };

        public static float EvaluateBoard(ChessBoard board, bool isWhiteTurn)
        {
            float score = 0f;

            // Material evaluation
            score += EvaluateMaterial(board);

            // Position evaluation
            score += EvaluatePositions(board);

            // Mobility evaluation (number of possible moves)
            score += EvaluateMobility(board, isWhiteTurn);

            // King safety
            score += EvaluateKingSafety(board);

            return score;
        }

        private static float EvaluateMaterial(ChessBoard board)
        {
            float score = 0f;

            foreach (var tile in board.Tiles.Values)
            {
                if (tile.CurrentPiece == null)
                {
                    continue;
                }

                ChessPiece piece = tile.CurrentPiece;
                float pieceValue = GetPieceValue(piece.PieceType);

                if (piece.IsWhite)
                {
                    score += pieceValue;
                }
                else
                {
                    score -= pieceValue;
                }
            }

            return score;
        }

        private static float EvaluatePositions(ChessBoard board)
        {
            float score = 0f;

            foreach (var tile in board.Tiles.Values)
            {
                if (tile.CurrentPiece == null)
                {
                    continue;
                }

                ChessPiece piece = tile.CurrentPiece;
                Vector2Int position = ChessBoard.GetPositionFromCoordinate(tile.Coordinate);

                float positionBonus = GetPositionBonus(piece.PieceType, position, piece.IsWhite);

                if (piece.IsWhite)
                {
                    score += positionBonus;
                }
                else
                {
                    score -= positionBonus;
                }
            }

            return score;
        }

        private static float EvaluateMobility(ChessBoard board, bool isWhiteTurn)
        {
            float whiteMobility = 0f;
            float blackMobility = 0f;

            List<ChessPiece> whitePieces = board.GetAllPieces(true);
            List<ChessPiece> blackPieces = board.GetAllPieces(false);

            foreach (var piece in whitePieces)
            {
                whiteMobility += piece.GetValidMoves().Count * 0.1f;
            }

            foreach (var piece in blackPieces)
            {
                blackMobility += piece.GetValidMoves().Count * 0.1f;
            }

            return whiteMobility - blackMobility;
        }

        private static float EvaluateKingSafety(ChessBoard board)
        {
            float score = 0f;

            // Check if kings are in check (negative score for being in check)
            if (board.IsInCheck(true))
            {
                score -= 50f;
            }

            if (board.IsInCheck(false))
            {
                score += 50f;
            }

            return score;
        }

        private static float GetPieceValue(ChessPieceType pieceType)
        {
            return PieceValues.ContainsKey(pieceType) ? PieceValues[pieceType] : 1.0f;
        }

        private static float GetPositionBonus(
            ChessPieceType pieceType,
            Vector2Int position,
            bool isWhite
        )
        {
            // Flip position for black pieces
            int row = isWhite ? position.y : 7 - position.y;
            int col = position.x;

            // Ensure position is within bounds
            if (row < 0 || row >= 8 || col < 0 || col >= 8)
            {
                return 0f;
            }

            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    return PawnPositionBonus[row, col];
                case ChessPieceType.Knight:
                    return KnightPositionBonus[row, col];
                default:
                    return 0f; // No specific position bonus for other pieces yet
            }
        }
    }
}
