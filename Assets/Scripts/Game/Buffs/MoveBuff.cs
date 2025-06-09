namespace Assets.Scripts.Game.Buffs
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Move buffs add valid moves to a chess piece
    public abstract class MoveBuff : IBuff
    {
        public Func<Vector2Int, ChessBoard, bool, List<ChessTile>> MoveFunction { get; set; } // Function that is applied when a condition is met

        public override bool IsActive { get; set; } = true;

        public override int DurationMoves { get; set; } = -1;

        public override int DurationTurns { get; set; } = -1;

        public override int DurationRounds { get; set; } = -1;

        internal override object BuffFunction(IChessObject buffReceiver, ChessBoard board)
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
