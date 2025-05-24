namespace Assets.Scripts.Game.AI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class ChessAI : MonoBehaviour
    {
        [Header("AI Configuration")]
        [Range(1, 6)]
        public int SearchDepth = 3;

        [Range(0.1f, 5.0f)]
        public float ThinkingTime = 1.0f;

        [Header("AI Personality")]
        public bool IsAggressive = false;
        public bool PreferCenter = true;
        public bool IsDefensive;

        private ChessBoard chessBoard;
        private System.Random random;

        // Piece values for evaluation
        private readonly Dictionary<ChessPieceType, int> pieceValues = new Dictionary<
            ChessPieceType,
            int
        >
        {
            { ChessPieceType.Pawn, 100 },
            { ChessPieceType.Knight, 320 },
            { ChessPieceType.Bishop, 330 },
            { ChessPieceType.Rook, 500 },
            { ChessPieceType.Queen, 900 },
            { ChessPieceType.King, 20000 },
        };

        // Position value tables for better positional play
        private readonly int[,] pawnTable = new int[8, 8]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5, 5, 10, 25, 25, 10, 5, 5 },
            { 0, 0, 0, 20, 20, 0, 0, 0 },
            { 5, -5, -10, 0, 0, -10, -5, 5 },
            { 5, 10, 10, -20, -20, 10, 10, 5 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        private readonly int[,] knightTable = new int[8, 8]
        {
            { -50, -40, -30, -30, -30, -30, -40, -50 },
            { -40, -20, 0, 0, 0, 0, -20, -40 },
            { -30, 0, 10, 15, 15, 10, 0, -30 },
            { -30, 5, 15, 20, 20, 15, 5, -30 },
            { -30, 0, 15, 20, 20, 15, 0, -30 },
            { -30, 5, 10, 15, 15, 10, 5, -30 },
            { -40, -20, 0, 5, 5, 0, -20, -40 },
            { -50, -40, -30, -30, -30, -30, -40, -50 },
        };

        private readonly int[,] centerTable = new int[8, 8]
        {
            { -20, -10, -10, -10, -10, -10, -10, -20 },
            { -10, 0, 0, 0, 0, 0, 0, -10 },
            { -10, 0, 5, 10, 10, 5, 0, -10 },
            { -10, 5, 5, 10, 10, 5, 5, -10 },
            { -10, 0, 10, 10, 10, 10, 0, -10 },
            { -10, 10, 10, 10, 10, 10, 10, -10 },
            { -10, 5, 0, 0, 0, 0, 5, -10 },
            { -20, -10, -10, -10, -10, -10, -10, -20 },
        };

        private void Awake()
        {
            this.chessBoard = this.GetComponent<ChessBoard>();
            this.random = new System.Random();
        }

        // Main AI decision method
        public IEnumerator MakeMove()
        {
            if (this.chessBoard == null || this.chessBoard.IsGameOver())
            {
                yield break;
            }

            this.chessBoard.SetAITurn(true);

            // Add thinking delay for more natural feel
            yield return new WaitForSeconds(this.ThinkingTime);

            ChessMove bestMove = this.FindBestMove();

            if (bestMove != null)
            {
                this.ExecuteMove(bestMove);
            }
            else
            {
                Debug.LogWarning("AI could not find a valid move!");
            }

            this.chessBoard.SetAITurn(false);
        }

        // Find the best move using minimax with alpha-beta pruning
        private ChessMove FindBestMove()
        {
            List<ChessMove> allMoves = this.GetAllValidMoves(false); // AI plays as black

            if (allMoves.Count == 0)
            {
                return null;
            }

            ChessMove bestMove = null;
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            // Shuffle moves for variety
            allMoves = allMoves.OrderBy(x => this.random.Next()).ToList();

            // Prioritize captures and checks
            allMoves = this.OrderMoves(allMoves).ToList();

            foreach (ChessMove move in allMoves)
            {
                // Make the move
                ChessPiece piece = this.GetPieceAt(move.StartPosition);
                if (piece == null)
                {
                    continue;
                }

                ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                if (targetTile == null)
                {
                    continue;
                }

                this.chessBoard.MovePiece(piece, targetTile);

                // Evaluate position
                int score = this.Minimax(this.SearchDepth - 1, alpha, beta, true);

                // Undo the move
                this.chessBoard.UndoLastMove();

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                alpha = Mathf.Max(alpha, score);
                if (beta <= alpha)
                {
                    break; // Alpha-beta pruning
                }
            }

            return bestMove;
        }

        // Minimax algorithm with alpha-beta pruning
        private int Minimax(int depth, int alpha, int beta, bool isMaximizingPlayer)
        {
            if (depth == 0)
            {
                return this.EvaluatePosition();
            }

            GameState gameState = this.chessBoard.GetGameState(!isMaximizingPlayer);
            if (gameState != GameState.Ongoing)
            {
                return EvaluateGameState(gameState, depth);
            }

            List<ChessMove> moves = this.GetAllValidMoves(!isMaximizingPlayer);
            moves = this.OrderMoves(moves).Take(20).ToList(); // Limit moves for speed

            if (isMaximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (ChessMove move in moves)
                {
                    ChessPiece piece = this.GetPieceAt(move.StartPosition);
                    if (piece == null)
                    {
                        continue;
                    }

                    ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                    if (targetTile == null)
                    {
                        continue;
                    }

                    this.chessBoard.MovePiece(piece, targetTile);
                    int eval = this.Minimax(depth - 1, alpha, beta, false);
                    this.chessBoard.UndoLastMove();

                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (ChessMove move in moves)
                {
                    ChessPiece piece = this.GetPieceAt(move.StartPosition);
                    if (piece == null)
                    {
                        continue;
                    }

                    ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                    if (targetTile == null)
                    {
                        continue;
                    }

                    this.chessBoard.MovePiece(piece, targetTile);
                    int eval = this.Minimax(depth - 1, alpha, beta, true);
                    this.chessBoard.UndoLastMove();

                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minEval;
            }
        }

        // Order moves by priority (captures first, then other good moves)
        private IEnumerable<ChessMove> OrderMoves(List<ChessMove> moves)
        {
            return moves.OrderByDescending(move =>
            {
                int score = 0;

                // Prioritize captures
                ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                if (targetTile != null ? targetTile.CurrentPiece : null != null)
                {
                    ChessPiece capturedPiece = targetTile.CurrentPiece;
                    ChessPiece movingPiece = this.GetPieceAt(move.StartPosition);

                    if (movingPiece != null && capturedPiece.IsWhite != movingPiece.IsWhite)
                    {
                        // MVV-LVA: Most Valuable Victim - Least Valuable Attacker
                        score +=
                            this.pieceValues[capturedPiece.PieceType]
                            - (this.pieceValues[movingPiece.PieceType] / 10);
                    }
                }

                // Prioritize center moves
                if (this.PreferCenter)
                {
                    score += this.centerTable[move.TargetPosition.x, move.TargetPosition.y];
                }

                return score;
            });
        }

        // Evaluate the current position
        private int EvaluatePosition()
        {
            int score = 0;

            // Material evaluation
            score += this.EvaluateMaterial();

            // Positional evaluation
            score += this.EvaluatePositions();

            // King safety
            score += this.EvaluateKingSafety();

            // Apply personality modifiers
            if (this.IsAggressive)
            {
                score += this.EvaluateAggression();
            }

            if (this.IsDefensive)
            {
                score += this.EvaluateDefense();
            }

            return score;
        }

        private int EvaluateMaterial()
        {
            int score = 0;

            foreach (var tile in this.chessBoard.Tiles.Values)
            {
                if (tile.CurrentPiece != null)
                {
                    int pieceValue = this.pieceValues[tile.CurrentPiece.PieceType];
                    if (tile.CurrentPiece.IsWhite)
                    {
                        score -= pieceValue; // White is opponent
                    }
                    else
                    {
                        score += pieceValue; // Black is AI
                    }
                }
            }

            return score;
        }

        private int EvaluatePositions()
        {
            int score = 0;

            foreach (var tile in this.chessBoard.Tiles.Values)
            {
                if (tile.CurrentPiece != null)
                {
                    Vector2Int pos = tile.GetPosition();
                    int positionValue = this.GetPositionValue(
                        tile.CurrentPiece.PieceType,
                        pos,
                        tile.CurrentPiece.IsWhite
                    );

                    if (tile.CurrentPiece.IsWhite)
                    {
                        score -= positionValue;
                    }
                    else
                    {
                        score += positionValue;
                    }
                }
            }

            return score;
        }

        private int GetPositionValue(ChessPieceType pieceType, Vector2Int position, bool isWhite)
        {
            int x = position.x;
            int y = isWhite ? position.y : 7 - position.y; // Flip for black pieces

            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    return this.pawnTable[y, x];
                case ChessPieceType.Knight:
                    return this.knightTable[y, x];
                default:
                    return this.centerTable[y, x];
            }
        }

        private int EvaluateKingSafety()
        {
            int score = 0;

            // Simple king safety - penalize exposed kings
            List<ChessPiece> whiteKings = this.chessBoard.GetPiecesOfType(
                ChessPieceType.King,
                true
            );
            List<ChessPiece> blackKings = this.chessBoard.GetPiecesOfType(
                ChessPieceType.King,
                false
            );

            if (whiteKings.Count > 0)
            {
                score += this.EvaluateKingExposure(whiteKings[0], true) * -1;
            }

            if (blackKings.Count > 0)
            {
                score += this.EvaluateKingExposure(blackKings[0], false);
            }

            return score;
        }

        private int EvaluateKingExposure(ChessPiece king, bool isWhite)
        {
            Vector2Int kingPos = king.CurrentTile.GetPosition();
            int exposure = 0;

            // Count how many squares around the king are empty or occupied by enemy pieces
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    Vector2Int checkPos = new Vector2Int(kingPos.x + dx, kingPos.y + dy);
                    if (this.chessBoard.IsValidPosition(checkPos))
                    {
                        ChessTile tile = this.chessBoard.GetTile(checkPos);
                        if (tile.CurrentPiece == null || tile.CurrentPiece.IsWhite != isWhite)
                        {
                            exposure += 10;
                        }
                    }
                }
            }

            return exposure;
        }

        private int EvaluateAggression()
        {
            int score = 0;
            List<ChessPiece> aiPieces = this.chessBoard.GetAllPieces(false);

            foreach (var piece in aiPieces)
            {
                var moves = piece.GetValidMoves();
                foreach (var move in moves)
                {
                    ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                    if (
                        targetTile != null
                            ? targetTile.CurrentPiece
                            : null != null && targetTile.CurrentPiece.IsWhite
                    )
                    {
                        score += 5; // Bonus for attacking enemy pieces
                    }
                }
            }

            return score;
        }

        private int EvaluateDefense()
        {
            int score = 0;
            List<ChessPiece> aiPieces = this.chessBoard.GetAllPieces(false);

            // Bonus for pieces that defend each other
            foreach (var piece in aiPieces)
            {
                var moves = piece.GetValidMoves();
                foreach (var move in moves)
                {
                    ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);
                    if (
                        targetTile != null
                            ? targetTile.CurrentPiece
                            : null != null && !targetTile.CurrentPiece.IsWhite
                    )
                    {
                        score += 3; // Bonus for defending own pieces
                    }
                }
            }

            return score;
        }

        private static int EvaluateGameState(GameState state, int depth)
        {
            switch (state)
            {
                case GameState.WhiteWins:
                    return -10000 + depth; // Prefer to lose later
                case GameState.BlackWins:
                    return 10000 - depth; // Prefer to win sooner
                case GameState.Stalemate:
                    return 0;
                default:
                    return 0;
            }
        }

        // Get all valid moves for a player
        private List<ChessMove> GetAllValidMoves(bool isWhite)
        {
            List<ChessMove> allMoves = new List<ChessMove>();
            List<ChessPiece> pieces = this.chessBoard.GetAllPieces(isWhite);

            foreach (var piece in pieces)
            {
                var validMoves = piece.GetValidMoves();
                allMoves.AddRange(validMoves);
            }

            return allMoves;
        }

        // Get piece at specific position
        private ChessPiece GetPieceAt(Vector2Int position)
        {
            ChessTile tile = this.chessBoard.GetTile(position);
            return tile != null ? tile.CurrentPiece : null;
        }

        // Execute the chosen move
        private void ExecuteMove(ChessMove move)
        {
            ChessPiece piece = this.GetPieceAt(move.StartPosition);
            if (piece != null)
            {
                this.chessBoard.MovePieceAI(piece, move);
                Debug.Log(
                    $"AI moved {piece.PieceType} from {ChessBoard.GetCoordinateFromPosition(move.StartPosition.x, move.StartPosition.y)} to {ChessBoard.GetCoordinateFromPosition(move.TargetPosition.x, move.TargetPosition.y)}"
                );
            }
        }

        // Check if it's AI's turn and make a move
        public void CheckAndMakeAIMove()
        {
            // AI plays as black, so trigger when it's not white's turn
            if (
                !this.chessBoard.IsWhiteTurn()
                && !this.chessBoard.IsGameOver()
                && !this.chessBoard.IsAITurn
            )
            {
                this.StartCoroutine(this.MakeMove());
            }
        }

        // Public method to manually trigger AI move
        public void TriggerAIMove()
        {
            if (!this.chessBoard.IsWhiteTurn() && !this.chessBoard.IsGameOver())
            {
                this.StartCoroutine(this.MakeMove());
            }
        }
    }
}
