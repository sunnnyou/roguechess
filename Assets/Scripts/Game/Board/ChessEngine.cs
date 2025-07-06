namespace Assets.Scripts.Game.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using UnityEngine;

    public class ChessEngine : MonoBehaviour
    {
        [Header("Engine Settings")]
        [SerializeField]
        private int searchDepth = 3;

        [SerializeField]
        private float thinkingTimeMin = 0.5f;

        [SerializeField]
        private float thinkingTimeMax = 2.0f;

        [SerializeField]
        private bool enableRandomness = true;

        [SerializeField]
        private float randomnessFactor = 0.1f;

        // Piece values for evaluation
        private readonly Dictionary<ChessPieceType, int> pieceValues = new()
        {
            { ChessPieceType.Pawn, 100 },
            { ChessPieceType.Knight, 320 },
            { ChessPieceType.Bishop, 330 },
            { ChessPieceType.Rook, 500 },
            { ChessPieceType.Queen, 900 },
            { ChessPieceType.King, 20000 },
        };

        public bool Thinking { get; private set; }

        // Position bonus tables for piece placement
        private readonly int[][] pawnTable = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 50, 50, 50, 50, 50, 50, 50, 50 },
            new int[] { 10, 10, 20, 30, 30, 20, 10, 10 },
            new int[] { 5, 5, 10, 25, 25, 10, 5, 5 },
            new int[] { 0, 0, 0, 20, 20, 0, 0, 0 },
            new int[] { 5, -5, -10, 0, 0, -10, -5, 5 },
            new int[] { 5, 10, 10, -20, -20, 10, 10, 5 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        private readonly int[][] knightTable = new int[][]
        {
            new int[] { -50, -40, -30, -30, -30, -30, -40, -50 },
            new int[] { -40, -20, 0, 0, 0, 0, -20, -40 },
            new int[] { -30, 0, 10, 15, 15, 10, 0, -30 },
            new int[] { -30, 5, 15, 20, 20, 15, 5, -30 },
            new int[] { -30, 0, 15, 20, 20, 15, 0, -30 },
            new int[] { -30, 5, 10, 15, 15, 10, 5, -30 },
            new int[] { -40, -20, 0, 5, 5, 0, -20, -40 },
            new int[] { -50, -40, -30, -30, -30, -30, -40, -50 },
        };

        private readonly int[][] bishopTable = new int[][]
        {
            new int[] { -20, -10, -10, -10, -10, -10, -10, -20 },
            new int[] { -10, 0, 0, 0, 0, 0, 0, -10 },
            new int[] { -10, 0, 5, 10, 10, 5, 0, -10 },
            new int[] { -10, 5, 5, 10, 10, 5, 5, -10 },
            new int[] { -10, 0, 10, 10, 10, 10, 0, -10 },
            new int[] { -10, 10, 10, 10, 10, 10, 10, -10 },
            new int[] { -10, 5, 0, 0, 0, 0, 5, -10 },
            new int[] { -20, -10, -10, -10, -10, -10, -10, -20 },
        };

        private readonly int[][] rookTable = new int[][]
        {
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new int[] { 5, 10, 10, 10, 10, 10, 10, 5 },
            new int[] { -5, 0, 0, 0, 0, 0, 0, -5 },
            new int[] { -5, 0, 0, 0, 0, 0, 0, -5 },
            new int[] { -5, 0, 0, 0, 0, 0, 0, -5 },
            new int[] { -5, 0, 0, 0, 0, 0, 0, -5 },
            new int[] { -5, 0, 0, 0, 0, 0, 0, -5 },
            new int[] { 0, 0, 0, 5, 5, 0, 0, 0 },
        };

        private readonly int[][] queenTable = new int[][]
        {
            new int[] { -20, -10, -10, -5, -5, -10, -10, -20 },
            new int[] { -10, 0, 0, 0, 0, 0, 0, -10 },
            new int[] { -10, 0, 5, 5, 5, 5, 0, -10 },
            new int[] { -5, 0, 5, 5, 5, 5, 0, -5 },
            new int[] { 0, 0, 5, 5, 5, 5, 0, -5 },
            new int[] { -10, 5, 5, 5, 5, 5, 0, -10 },
            new int[] { -10, 0, 5, 0, 0, 0, 0, -10 },
            new int[] { -20, -10, -10, -5, -5, -10, -10, -20 },
        };

        private readonly int[][] kingMiddleGameTable = new int[][]
        {
            new int[] { -30, -40, -40, -50, -50, -40, -40, -30 },
            new int[] { -30, -40, -40, -50, -50, -40, -40, -30 },
            new int[] { -30, -40, -40, -50, -50, -40, -40, -30 },
            new int[] { -30, -40, -40, -50, -50, -40, -40, -30 },
            new int[] { -20, -30, -30, -40, -40, -30, -30, -20 },
            new int[] { -10, -20, -20, -20, -20, -20, -20, -10 },
            new int[] { 20, 20, 0, 0, 0, 0, 20, 20 },
            new int[] { 20, 30, 10, 0, 0, 10, 30, 20 },
        };

        private readonly int[][] kingEndGameTable = new int[][]
        {
            new int[] { -50, -40, -30, -20, -20, -30, -40, -50 },
            new int[] { -30, -20, -10, 0, 0, -10, -20, -30 },
            new int[] { -30, -10, 20, 30, 30, 20, -10, -30 },
            new int[] { -30, -10, 30, 40, 40, 30, -10, -30 },
            new int[] { -30, -10, 30, 40, 40, 30, -10, -30 },
            new int[] { -30, -10, 20, 30, 30, 20, -10, -30 },
            new int[] { -30, -30, 0, 0, 0, 0, -30, -30 },
            new int[] { -50, -30, -30, -30, -30, -30, -30, -50 },
        };

        private System.Random random;

        private void Awake()
        {
            this.random = new System.Random();
        }

        public void MakeBestMove(bool isWhite)
        {
            if (this.Thinking)
            {
                return;
            }
            this.StartCoroutine(this.ThinkAndMove(isWhite));
        }

        private System.Collections.IEnumerator ThinkAndMove(bool isWhite)
        {
            if (this.Thinking)
            {
                yield break;
            }

            this.Thinking = true;

            // Add some thinking time for realism
            float thinkTime = Random.Range(this.thinkingTimeMin, this.thinkingTimeMax);
            yield return new WaitForSeconds(thinkTime);

            ChessMove bestMove = this.GetBestMove(isWhite);

            if (bestMove != null)
            {
                ExecuteMove(bestMove);
            }
            else
            {
                Debug.LogWarning("No valid moves found for AI");
            }
            this.Thinking = false;
        }

        public ChessMove GetBestMove(bool isWhite)
        {
            var allMoves = GetAllValidMoves(isWhite);

            if (allMoves.Count == 0)
            {
                return null;
            }

            ChessMove bestMove = null;
            int bestScore = isWhite ? int.MinValue : int.MaxValue;

            foreach (var move in allMoves)
            {
                // Make the move temporarily
                var moveHistory = MakeMove(move);

                // Evaluate the position
                int score = this.Minimax(
                    this.searchDepth - 1,
                    !isWhite,
                    int.MinValue,
                    int.MaxValue
                );

                // Add randomness if enabled
                if (this.enableRandomness)
                {
                    int randomBonus = (int)(this.random.NextDouble() * this.randomnessFactor * 100);
                    score += isWhite ? randomBonus : -randomBonus;
                }

                // Undo the move
                ChessBoard.Instance.UndoMove(moveHistory);

                // Check if this is the best move
                if ((isWhite && score > bestScore) || (!isWhite && score < bestScore))
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
        {
            if (depth == 0)
            {
                return this.EvaluatePosition();
            }

            var moves = GetAllValidMoves(isMaximizing);

            if (moves.Count == 0)
            {
                // Check if it's checkmate or stalemate
                if (IsInCheck(isMaximizing))
                {
                    return isMaximizing
                        ? -20000 + (this.searchDepth - depth)
                        : 20000 - (this.searchDepth - depth);
                }
                return 0; // Stalemate
            }

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                foreach (var move in moves)
                {
                    var moveHistory = MakeMove(move);
                    int eval = this.Minimax(depth - 1, false, alpha, beta);
                    ChessBoard.Instance.UndoMove(moveHistory);

                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in moves)
                {
                    var moveHistory = MakeMove(move);
                    int eval = this.Minimax(depth - 1, true, alpha, beta);
                    ChessBoard.Instance.UndoMove(moveHistory);

                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }
                return minEval;
            }
        }

        private int EvaluatePosition()
        {
            int score = 0;

            // Material evaluation
            foreach (var tile in ChessBoard.Instance.GetAllTiles())
            {
                if (tile.CurrentPiece != null)
                {
                    var piece = tile.CurrentPiece;
                    int pieceValue = this.pieceValues[piece.PieceType];
                    int positionBonus = this.GetPositionBonus(piece);

                    int totalValue = pieceValue + positionBonus;

                    if (piece.IsWhite)
                    {
                        score += totalValue;
                    }
                    else
                    {
                        score -= totalValue;
                    }
                }
            }

            // Add mobility bonus (number of legal moves)
            score += GetAllValidMoves(true).Count * 10;
            score -= GetAllValidMoves(false).Count * 10;

            return score;
        }

        private int GetPositionBonus(ChessPiece piece)
        {
            int x = piece.CurrentTile.Position.x;
            int y = piece.CurrentTile.Position.y;

            // Flip coordinates for black pieces
            if (!piece.IsWhite)
            {
                y = 7 - y;
            }

            // Ensure coordinates are within bounds
            x = Mathf.Clamp(x, 0, 7);
            y = Mathf.Clamp(y, 0, 7);

            return piece.PieceType switch
            {
                ChessPieceType.Pawn => this.pawnTable[y][x],
                ChessPieceType.Knight => this.knightTable[y][x],
                ChessPieceType.Bishop => this.bishopTable[y][x],
                ChessPieceType.Rook => this.rookTable[y][x],
                ChessPieceType.Queen => this.queenTable[y][x],
                ChessPieceType.King => IsEndGame()
                    ? this.kingEndGameTable[y][x]
                    : this.kingMiddleGameTable[y][x],
                _ => 0,
            };
        }

        private static bool IsEndGame()
        {
            // Simple endgame detection: few pieces left on board
            int pieceCount = 0;
            foreach (var tile in ChessBoard.Instance.GetAllTiles())
            {
                if (tile.CurrentPiece != null)
                {
                    pieceCount++;
                }
            }
            return pieceCount <= 10;
        }

        private static List<ChessMove> GetAllValidMoves(bool isWhite)
        {
            var moves = new List<ChessMove>();
            var pieces = ChessBoard.Instance.GetAllPieces(isWhite);

            foreach (var piece in pieces)
            {
                var pieceMoves = piece.GetValidMoves();
                moves.AddRange(pieceMoves);
            }

            return moves;
        }

        private static bool IsInCheck(bool isWhiteKing)
        {
            var kings = ChessBoard.Instance.GetPiecesOfType(ChessPieceType.King, isWhiteKing);

            foreach (var king in kings)
            {
                if (ChessBoard.Instance.IsInCheck(isWhiteKing, king))
                {
                    return true;
                }
            }

            return false;
        }

        private static ChessMoveHistory MakeMove(ChessMove move)
        {
            if (
                !ChessBoard.Instance.GetTile(move.StartPosition, out ChessTile fromTile)
                || !ChessBoard.Instance.GetTile(move.TargetPosition, out ChessTile toTile)
            )
            {
                return null;
            }

            var piece = fromTile.CurrentPiece;

            var moveHistory = toTile.UpdatePiece(piece, true, false);

            return moveHistory;
        }

        private static void ExecuteMove(ChessMove move)
        {
            if (!ChessBoard.Instance.GetTile(move.StartPosition, out ChessTile fromTile))
            {
                Debug.LogError($"Could not find from tile at {move.StartPosition}");
                return;
            }

            if (!ChessBoard.Instance.GetTile(move.TargetPosition, out ChessTile toTile))
            {
                Debug.LogError($"Could not find target tile at {move.TargetPosition}");
                return;
            }

            var piece = fromTile.CurrentPiece;
            if (piece == null)
            {
                Debug.LogError($"No piece found at {move.StartPosition}");
                return;
            }

            // Use the board's MovePiece method to ensure proper game state updates
            ChessBoard.Instance.MovePiece(piece, toTile);
        }

        // Public methods for external control
        public void SetSearchDepth(int depth)
        {
            this.searchDepth = Mathf.Clamp(depth, 1, 6);
        }

        public void SetThinkingTime(float min, float max)
        {
            this.thinkingTimeMin = Mathf.Max(0.1f, min);
            this.thinkingTimeMax = Mathf.Max(this.thinkingTimeMin, max);
        }

        public void SetRandomness(bool enabled, float factor = 0.1f)
        {
            this.enableRandomness = enabled;
            this.randomnessFactor = Mathf.Clamp01(factor);
        }
    }
}
