namespace Assets.Scripts.Game.Buffs
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public abstract class BuffBase : ScriptableObject
    {
        [Header("UI Settings")]
        public string BuffName;
        public string Description;
        public Sprite Icon;

        [Header("Buff Mechanics")]
        public bool IsActive = true;
        public int Cost = -1;
        public int DurationMoves = -1;
        public int DurationTurns = -1;
        public int DurationRounds = -1;

        [HideInInspector]
        public bool WasUsed = false;

        [HideInInspector]
        public int Turn;

        [HideInInspector]
        public int Round;

        /// <summary> Override this to define what the buff does. </summary>
        public abstract object BuffFunction(IChessObject buffReceiver, ChessBoard board);

        public object ApplyBuff(IChessObject buffReceiver, ChessBoard board)
        {
            if (buffReceiver == null || board == null)
            {
                Debug.LogError($"Invalid arguments for buff '{this.BuffName}'.");
                return null;
            }

            if (!this.IsActive)
            {
                Debug.LogWarning($"Buff '{this.BuffName}' is not active.");
                return null;
            }

            var result = this.BuffFunction(buffReceiver, board);
            this.UpdateDuration(board.CurrentTurn, board.CurrentRound);
            return result;
        }

        private void UpdateDuration(int currentTurn, int currentRound)
        {
            if (this.Turn < currentTurn)
            {
                this.DurationTurns = Mathf.Max(-1, this.DurationTurns - (currentTurn - this.Turn));
                this.Turn = currentTurn;
            }

            if (this.Round < currentRound)
            {
                this.DurationRounds = Mathf.Max(
                    -1,
                    this.DurationRounds - (currentRound - this.Round)
                );
                this.Round = currentRound;
            }

            this.DurationMoves = Mathf.Max(-1, this.DurationMoves - 1);

            if (this.DurationMoves == 0 || this.DurationTurns == 0 || this.DurationRounds == 0)
            {
                this.IsActive = false;
            }

            this.WasUsed = true;
        }
    }
}
