namespace Assets.Scripts.Game.Buffs
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public abstract class IBuff
    {
        public abstract string BuffName { get; set; } // Name of the buff that is displayed in the shop or on hover in the UI

        public abstract string Description { get; set; } // Description that is displayed in the shop or on hover in the UI

        public abstract Sprite Icon { get; set; } // Icon for the buff displayed in the UI

        public abstract bool IsActive { get; set; } // If true, the buff method can be called to apply the buff

        public abstract int Cost { get; set; } // Cost in shop, -1 means it will not show up in the shop

        public abstract int DurationMoves { get; set; } // Duration in moves, -1 means permanent (Decreased by 1 each time the buff function is called)

        public abstract int DurationTurns { get; set; } // Duration in turns, -1 means permanent (Decreased by 1 each time the player's turn and then the enemy's turn is over)

        public abstract int DurationRounds { get; set; } // Duration in rounds, -1 means permanent (Decreased by 1 each time the player or the enemy have won the match/round)

        public abstract bool WasUsed { get; set; } // True if the buff function has been called at least once

        public int Turn { get; set; } // The number of times the Black player has made their move. Used for update DurationTurns

        public int Round { get; set; } // The number of times the player or enemy have won a match/round. Used for update DurationRounds

        public abstract object ApplyBuff(object buffReceiver, ChessBoard board); // Apply the buff

        public void UpdateDuration(int currentTurn, int currentRound)
        {
            if (this.Turn < currentTurn)
            {
                this.DurationTurns = currentTurn - this.Turn;
                this.Turn = currentTurn;
            }

            if (this.Round < currentRound)
            {
                this.DurationRounds = currentRound - this.Round;
                this.Round = currentRound;
            }

            if (this.DurationMoves == 0 || this.DurationTurns == 0 || this.DurationRounds == 0)
            {
                this.IsActive = false;
            }
        }
    }
}
