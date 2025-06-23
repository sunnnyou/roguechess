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
        public abstract object BuffFunction(IChessObject buffReceiver);

        public object ApplyBuff(IChessObject buffReceiver)
        {
            if (buffReceiver == null)
            {
                Debug.LogError($"Invalid arguments for buff '{this.BuffName}'.");
                return null;
            }

            if (!this.IsActive)
            {
                Debug.LogWarning($"Buff '{this.BuffName}' is not active.");
                return null;
            }

            var result = this.BuffFunction(buffReceiver);
            this.UpdateDuration();
            return result;
        }

        private void UpdateDuration()
        {
            if (this.Turn < ChessBoard.Instance.CurrentRound)
            {
                this.DurationTurns = Mathf.Max(
                    -1,
                    this.DurationTurns - (ChessBoard.Instance.CurrentTurn - this.Turn)
                );
                this.Turn = ChessBoard.Instance.CurrentTurn;
            }

            if (this.Round < ChessBoard.Instance.CurrentRound)
            {
                this.DurationRounds = Mathf.Max(
                    -1,
                    this.DurationRounds - (ChessBoard.Instance.CurrentRound - this.Round)
                );
                this.Round = ChessBoard.Instance.CurrentRound;
            }

            this.DurationMoves = Mathf.Max(-1, this.DurationMoves - 1);

            if (this.DurationMoves == 0 || this.DurationTurns == 0 || this.DurationRounds == 0)
            {
                this.IsActive = false;
                Debug.LogWarning($"Buff '{this.BuffName}' set to inactive.");
            }

            this.WasUsed = true;
        }
    }
}
