// AIManager.cs - Higher level AI management
namespace Assets.Scripts.Game.AI
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class AIManager : MonoBehaviour
    {
        [Header("AI Configuration")]
        public ChessAI ChessAI;
        public ChessBoard ChessBoard;

        [Header("Game Settings")]
        public bool EnableAI = true;
        public bool AIPlaysBlack = true;

        [Header("Difficulty Presets")]
        public AIDifficultyPreset[] DifficultyPresets = new AIDifficultyPreset[]
        {
            new AIDifficultyPreset
            {
                Name = "Easy",
                Depth = 2,
                ThinkingTime = 0.5f,
            },
            new AIDifficultyPreset
            {
                Name = "Medium",
                Depth = 3,
                ThinkingTime = 1.0f,
            },
            new AIDifficultyPreset
            {
                Name = "Hard",
                Depth = 4,
                ThinkingTime = 1.5f,
            },
            new AIDifficultyPreset
            {
                Name = "Expert",
                Depth = 5,
                ThinkingTime = 2.0f,
            },
        };

        [Header("Current Settings")]
        public int CurrentDifficultyIndex = 1; // Medium by default

        public void Start()
        {
            if (this.ChessAI == null)
            {
                this.ChessAI = FindObjectOfType<ChessAI>();
            }

            if (this.ChessBoard == null)
            {
                this.ChessBoard = FindObjectOfType<ChessBoard>();
            }

            this.ApplyDifficultyPreset(this.CurrentDifficultyIndex);

            if (this.ChessAI != null)
            {
                this.ChessAI.SetAISide(this.AIPlaysBlack);
            }
        }

        public void SetDifficulty(int presetIndex)
        {
            if (presetIndex >= 0 && presetIndex < this.DifficultyPresets.Length)
            {
                this.CurrentDifficultyIndex = presetIndex;
                this.ApplyDifficultyPreset(presetIndex);
            }
        }

        public void SetAISide(bool playsAsBlack)
        {
            this.AIPlaysBlack = playsAsBlack;
            if (this.ChessAI != null)
            {
                this.ChessAI.SetAISide(playsAsBlack);
            }
        }

        public void ToggleAI()
        {
            this.EnableAI = !this.EnableAI;
            if (this.ChessAI != null)
            {
                this.ChessAI.enabled = this.EnableAI;
            }
        }

        private void ApplyDifficultyPreset(int index)
        {
            if (index < 0 || index >= this.DifficultyPresets.Length || this.ChessAI == null)
            {
                return;
            }

            AIDifficultyPreset preset = this.DifficultyPresets[index];
            this.ChessAI.SearchDepth = preset.Depth;
            this.ChessAI.ThinkingTime = preset.ThinkingTime;

            Debug.Log(
                $"AI difficulty set to: {preset.Name} (Depth: {preset.Depth}, Time: {preset.ThinkingTime}s)"
            );
        }

        // For UI buttons
        public void OnDifficultyChanged(int newIndex)
        {
            this.SetDifficulty(newIndex);
        }

        public void OnAISideChanged(bool playsBlack)
        {
            this.SetAISide(playsBlack);
        }

        [System.Serializable]
        public class AIDifficultyPreset
        {
            public string Name;

            [Range(1, 6)]
            public int Depth = 3;

            [Range(0.1f, 5.0f)]
            public float ThinkingTime = 1.0f;
        }
    }
}
