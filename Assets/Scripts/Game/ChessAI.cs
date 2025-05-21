using System.Collections.Generic;
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

    public ChessAI(ChessBoard board, bool isWhiteAI, int depth)
    {
        this.board = board;
        this.isWhiteAI = isWhiteAI;
        this.searchDepth = depth;
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
            foreach (var move in ChessBoard.GetValidMoves(piece))
            {
                // Simulate move
                var originalTile = piece.CurrentTile;
                var targetCoord = ChessBoard.GetCoordinateFromPosition(
                    move.TargetPosition.x,
                    move.TargetPosition.y
                );
                var targetTile = this.board.GetTile(targetCoord);
                var capturedPiece = targetTile.CurrentPiece;

                this.board.MovePieceAI(piece, move);

                // Evaluate position
                int value = -this.EvaluatePosition();

                // Undo move
                this.board.MovePiece(piece, originalTile);
                if (capturedPiece != null)
                {
                    targetTile.PlacePiece(capturedPiece);
                }

                if (value > bestValue)
                {
                    bestValue = value;
                    bestPiece = piece;
                    bestMove = move;
                }
            }
        }

        return (bestPiece, bestMove);
    }

    private int EvaluatePosition()
    {
        int score = 0;

        // Material value
        foreach (var piece in this.board.GetAllPieces(true))
        {
            score += GetPieceValue(piece);
        }

        foreach (var piece in this.board.GetAllPieces(false))
        {
            score -= GetPieceValue(piece);
        }

        return this.isWhiteAI ? score : -score;
    }

    private static int GetPieceValue(ChessPiece piece)
    {
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
