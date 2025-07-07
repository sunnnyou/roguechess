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
        public int DurationTurns = -1;
        public int DurationRounds = -1;

        [Header("Usability")]
        public bool UsableInGame;
        public SelectionType SelectionType = SelectionType.None;
        public bool UsableOnce;

        [HideInInspector]
        public bool WasUsed = false;

        [HideInInspector]
        public int Turn;

        [HideInInspector]
        public int Round;

        protected BuffBase()
        {
            if (ChessBoard.Instance != null && ChessBoard.Instance.Buffs != null)
            {
                ChessBoard.Instance.Buffs.Add(this);
            }
        }

        internal abstract object BuffFunction(IChessObject buffReceiver);

        public virtual void RemoveBuff()
        {
            // Optional implementation
        }

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
            return result;
        }

        public void UpdateDuration()
        {
            if (this.DurationTurns != -1 && this.Turn < ChessBoard.Instance.CurrentRound)
            {
                this.DurationTurns = Mathf.Max(
                    -1,
                    this.DurationTurns - (ChessBoard.Instance.CurrentTurn - this.Turn)
                );
                this.Turn = ChessBoard.Instance.CurrentTurn;
            }

            if (this.DurationRounds != -1 && this.Round < ChessBoard.Instance.CurrentRound)
            {
                this.DurationRounds = Mathf.Max(
                    -1,
                    this.DurationRounds - (ChessBoard.Instance.CurrentRound - this.Round)
                );
                this.Round = ChessBoard.Instance.CurrentRound;
            }

            if (
                (this.WasUsed && this.UsableOnce)
                || this.DurationTurns == 0
                || this.DurationRounds == 0
            )
            {
                this.RemoveBuff();
                this.IsActive = false;
                Debug.LogWarning($"Buff '{this.BuffName}' set to inactive.");
            }
        }
    }
}
