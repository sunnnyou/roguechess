namespace Assets.Scripts.Game.Buffs
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Move buffs add valid moves to a chess piece
    public abstract class MoveBuff : BuffBase
    {
        public Func<Vector2Int, ChessBoard, bool, List<ChessTile>> MoveFunction { get; set; } // Function that is applied when a condition is met

        public bool IsActive { get; set; } = true;

        public int DurationMoves { get; set; } = -1;

        public int DurationTurns { get; set; } = -1;

        public int DurationRounds { get; set; } = -1;

        public override object BuffFunction(IChessObject buffReceiver, ChessBoard board)
        {
            if (buffReceiver is not ChessPiece piece)
            {
                Debug.LogError(
                    $"Invalid arguments for MoveBuff '{this.BuffName}'. Expected ChessPiece for buffReceiver."
                );
                return null;
            }

            return this.MoveFunction(piece.CurrentTile.Position, board, piece.IsWhite);
        }
    }
}
