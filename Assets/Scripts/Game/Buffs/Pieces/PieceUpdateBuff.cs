namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Buff that updates the piece's movement rules
    public abstract class PieceUpdateBuff : IBuff
    {
        public Func<ChessPiece, ChessBoard, ChessPiece> UpdateFunction { get; set; } // Function that is applied when a condition is met

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
                return null;
            }

            return this.UpdateFunction(piece, board);
        }
    }
}
