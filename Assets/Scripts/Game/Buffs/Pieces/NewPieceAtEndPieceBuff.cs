namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Buff that allows a piece to be switched to a new piece when it reaches the end of the board
    public class NewPieceAtEndPieceBuff : MoveBuff
    {
        public override string BuffName { get; set; } = "Final Evolution";

        public override string Description { get; set; } =
            "Allows a piece to be switched to a new piece when it reaches the end of the board.";

        public bool IsAtStart { get; set; };

        public override Func<
            int,
            int,
            ChessBoard,
            bool,
            List<ChessTile>
        > MoveBuffFunction { get; set; }

        public override Sprite Icon { get; set; }

        public override int Cost { get; set; }

        public override int DurationRounds { get; set; }

        public override bool WasUsed { get; set; }

        public override int DurationTurns { get; set; }

        public override bool IsActive { get; set; } = true;

        public PawnBuff(int additionalTiles = 1)
        {
            this.additionalTiles = additionalTiles;
            // TODO add function for reaching the end of the board
            // and turning into a queen or other piece
            this.MoveBuffFunctions = new List<Func<object[], object>>
            {
                this.DoubleMove,
                GetEnPassantTiles,
            };
        }

        public List<ChessTile> FinalEvolution(int currentX, int currentY, ChessBoard board, bool isWhite)
        {
            if (isWhite && currentY == board.Height || !isWhite && currentY == 0)
            {
                // Logic to switch to a new piece
                // For example, switch to a queen
                // This is just a placeholder; actual implementation may vary
                Debug.Log("Piece reached the end of the board and will be switched.");
                return new List<ChessTile>(); // Return the new piece's tile(s)
            }

            return new List<ChessTile>(); // No change if not at the end
        }
    }
}
