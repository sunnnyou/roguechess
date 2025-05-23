namespace Assets.Scripts.Game.AI
{
    using System.Collections;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Main AI controller
    public class ChessAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [Range(1, 6)]
        public int SearchDepth = 3;

        [Range(0.1f, 5.0f)]
        public float ThinkingTime = 1.0f;

        public bool IsBlackAI = true; // True if AI plays as black

        [Header("Debug")]
        public bool ShowDebugLogs = true;

        private ChessBoard chessBoard;
        private bool isThinking;

        public void Start()
        {
            this.chessBoard = this.GetComponent<ChessBoard>();
            if (this.chessBoard == null)
            {
                this.chessBoard = FindObjectOfType<ChessBoard>();
            }

            if (this.chessBoard == null)
            {
                Debug.LogError("ChessAI: No ChessBoard found!");
            }
        }

        public void Update()
        {
            if (this.chessBoard == null || this.isThinking || this.chessBoard.IsGameOver())
            {
                return;
            }

            // Check if it's AI's turn
            bool isAITurn =
                (this.IsBlackAI && !this.chessBoard.IsWhiteTurn())
                || (!this.IsBlackAI && this.chessBoard.IsWhiteTurn());

            if (isAITurn && !this.chessBoard.IsAITurn)
            {
                this.StartCoroutine(this.MakeAIMove());
            }
        }

        private IEnumerator MakeAIMove()
        {
            this.isThinking = true;
            this.chessBoard.SetAITurn(true);

            if (this.ShowDebugLogs)
            {
                Debug.Log("AI is thinking...");
            }

            // Add some thinking time for better UX
            yield return new WaitForSeconds(this.ThinkingTime);

            ChessMove bestMove = this.GetBestMove(!this.IsBlackAI); // AI plays opposite color

            if (bestMove != null)
            {
                this.ExecuteMove(bestMove);

                if (this.ShowDebugLogs)
                {
                    Debug.Log($"AI executed move: {bestMove}");
                }
            }
            else
            {
                Debug.LogWarning("AI couldn't find a valid move!");
            }

            this.chessBoard.SetAITurn(false);
            this.isThinking = false;
        }

        public ChessMove GetBestMove(bool isWhitePlayer)
        {
            List<ChessMove> allMoves = this.GetAllValidMoves(isWhitePlayer);

            if (allMoves.Count == 0)
            {
                return null;
            }

            ChessMove bestMove = null;
            float bestScore = isWhitePlayer ? float.MinValue : float.MaxValue;

            foreach (ChessMove move in allMoves)
            {
                // Simulate the move
                BoardState originalState = this.CaptureCurrentBoardState();
                this.SimulateMove(move);

                // Evaluate using minimax
                float score = this.Minimax(
                    this.SearchDepth - 1,
                    !isWhitePlayer,
                    float.MinValue,
                    float.MaxValue
                );

                // Restore the board
                this.RestoreBoardState(originalState);

                // Check if this move is better
                if ((isWhitePlayer && score > bestScore) || (!isWhitePlayer && score < bestScore))
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            if (bestMove != null)
            {
                bestMove.Score = bestScore;
            }

            return bestMove;
        }

        private float Minimax(int depth, bool isMaximizing, float alpha, float beta)
        {
            if (depth == 0)
            {
                return BoardEvaluator.EvaluateBoard(this.chessBoard, isMaximizing);
            }

            List<ChessMove> moves = this.GetAllValidMoves(isMaximizing);

            if (moves.Count == 0)
            {
                // Check for checkmate or stalemate
                if (this.chessBoard.IsInCheck(isMaximizing))
                {
                    return isMaximizing ? -10000 + depth : 10000 - depth; // Checkmate
                }

                return 0; // Stalemate
            }

            if (isMaximizing)
            {
                float maxScore = float.MinValue;
                foreach (ChessMove move in moves)
                {
                    BoardState originalState = this.CaptureCurrentBoardState();
                    this.SimulateMove(move);

                    float score = this.Minimax(depth - 1, false, alpha, beta);
                    maxScore = Mathf.Max(maxScore, score);

                    this.RestoreBoardState(originalState);

                    alpha = Mathf.Max(alpha, score);
                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }

                return maxScore;
            }
            else
            {
                float minScore = float.MaxValue;
                foreach (ChessMove move in moves)
                {
                    BoardState originalState = this.CaptureCurrentBoardState();
                    this.SimulateMove(move);

                    float score = this.Minimax(depth - 1, true, alpha, beta);
                    minScore = Mathf.Min(minScore, score);

                    this.RestoreBoardState(originalState);

                    beta = Mathf.Min(beta, score);
                    if (beta <= alpha)
                    {
                        break; // Alpha-beta pruning
                    }
                }

                return minScore;
            }
        }

        private List<ChessMove> GetAllValidMoves(bool isWhitePlayer)
        {
            List<ChessMove> allMoves = new List<ChessMove>();
            List<ChessPiece> pieces = this.chessBoard.GetAllPieces(isWhitePlayer);

            foreach (ChessPiece piece in pieces)
            {
                List<ChessMove> pieceMoves = piece.GetValidMoves();
                allMoves.AddRange(pieceMoves);
            }

            return allMoves;
        }

        private void ExecuteMove(ChessMove move)
        {
            string startCoord = ChessBoard.GetCoordinateFromPosition(
                move.StartPosition.x,
                move.StartPosition.y
            );
            ChessTile startTile = this.chessBoard.GetTile(startCoord);

            if (startTile?.CurrentPiece != null)
            {
                this.chessBoard.MovePieceAI(startTile.CurrentPiece, move);
            }
        }

        private void SimulateMove(ChessMove move)
        {
            ChessTile startTile = this.chessBoard.GetTile(move.StartPosition);
            ChessTile targetTile = this.chessBoard.GetTile(move.TargetPosition);

            if (startTile?.CurrentPiece != null && targetTile != null)
            {
                this.chessBoard.MovePiece(startTile.CurrentPiece, targetTile);
            }
        }

        private BoardState CaptureCurrentBoardState()
        {
            BoardState state = new BoardState();
            state.Pieces = new Dictionary<string, PieceState>();

            foreach (var kvp in this.chessBoard.Tiles)
            {
                if (kvp.Value.CurrentPiece != null)
                {
                    ChessPiece piece = kvp.Value.CurrentPiece;
                    state.Pieces[kvp.Key] = new PieceState
                    {
                        PieceType = piece.PieceType,
                        IsWhite = piece.IsWhite,
                        Coordinate = kvp.Key,
                    };
                }
            }

            return state;
        }

        private void RestoreBoardState(BoardState state)
        {
            // Clear all current pieces
            foreach (var tile in this.chessBoard.Tiles.Values)
            {
                if (tile.CurrentPiece != null)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(tile.CurrentPiece.gameObject);
                    }
                    else
                    {
                        Destroy(tile.CurrentPiece.gameObject);
                    }

                    tile.CurrentPiece = null;
                }
            }

            // Restore pieces from state
            foreach (var kvp in state.Pieces)
            {
                PieceState pieceState = kvp.Value;
                this.chessBoard.SpawnPiece(
                    pieceState.PieceType,
                    pieceState.IsWhite,
                    pieceState.Coordinate
                );
            }
        }

        // Helper classes for board state management
        private class BoardState
        {
            public Dictionary<string, PieceState> Pieces;
        }

        private class PieceState
        {
            public ChessPieceType PieceType;
            public bool IsWhite;
            public string Coordinate;
        }

        // Public method to set AI difficulty
        public void SetDifficulty(int depth)
        {
            this.SearchDepth = Mathf.Clamp(depth, 1, 6);
        }

        // Public method to set which side AI plays
        public void SetAISide(bool playAsBlack)
        {
            this.IsBlackAI = playAsBlack;
        }
    }
}
