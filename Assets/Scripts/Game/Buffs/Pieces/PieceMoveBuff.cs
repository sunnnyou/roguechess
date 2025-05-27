namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public abstract class PieceMoveBuff : IBuff
    {
        public Func<Vector2Int, ChessBoard, bool, List<ChessTile>> MoveFunction { get; set; } // Function that is applied when a condition is met

        public override bool IsActive { get; set; } = true;

        public override int DurationMoves { get; set; } = -1;

        public override int DurationTurns { get; set; } = -1;

        public override int DurationRounds { get; set; } = -1;

        public override object ApplyBuff(object buffReceiver, ChessBoard board)
        {
            if (buffReceiver is not ChessPiece piece || board == null)
            {
                Debug.LogError(
                    "Invalid arguments for Movement buff. Expected ChessPiece, ChessBoard"
                );
                return new List<ChessPiece>();
            }

            var additionalValidTiles = this.MoveFunction(
                piece.CurrentTile.Position,
                board,
                piece.IsWhite
            );
            this.WasUsed = true;
            this.DurationMoves--;
            this.UpdateDuration(board.CurrentTurn, board.CurrentRound);
            return additionalValidTiles;
        }
    }
}
