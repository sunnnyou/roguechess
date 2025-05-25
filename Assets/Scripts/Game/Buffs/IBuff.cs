namespace Assets.Scripts.Game.Buffs
{
    using UnityEngine;

    public interface IBuff
    {
        public abstract string BuffName { get; set; }

        public abstract string Description { get; set; } // Description that is displayed in the shop or on hover

        public abstract Sprite Icon { get; set; } // Icon for the buff displayed in the UI

        public abstract bool IsActive { get; set; } // If true, the ApplyBuff method will be called when the buff is applied

        public abstract int Cost { get; set; } // Cost in shop, -1 means it will not show up in the shop

        public abstract int DurationTurns { get; set; } // Duration in turns, 0 means permanent

        public abstract int DurationRounds { get; set; } // Duration in rounds, 0 means permanent

        public abstract bool WasUsed { get; set; } // If true, the buff has been used at least once

        public abstract object ApplyBuff(params object[] args); // Apply the buff to the target, if any
    }
}
