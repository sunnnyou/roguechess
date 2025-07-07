namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Assets.Scripts.Game;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class RoundEndUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GameObject roundEndPanel;

        [SerializeField]
        private Button nextButton;

        [SerializeField]
        private TimerScript timer;

        [Header("Panel Structure")]
        [SerializeField]
        private TextMeshProUGUI headerTitle;

        [SerializeField]
        private TextMeshProUGUI nextButtonText;

        [SerializeField]
        private Image goldIconNext; // Gold icon for the next button

        [Header("Gold Display")]
        [SerializeField]
        private TextMeshProUGUI roundIncomeTime;

        [SerializeField]
        private TextMeshProUGUI roundIncomePieces;

        [SerializeField]
        private TextMeshProUGUI roundIncomeBuffs;

        [SerializeField]
        private TextMeshProUGUI roundIncomeBase;

        [SerializeField]
        private TextMeshProUGUI roundIncomeTotal;

        [Header("Display Items")]
        [SerializeField]
        private TextMeshProUGUI roundTime;

        [SerializeField]
        private TextMeshProUGUI roundPieces;

        [Header("Gold Icons")]
        [SerializeField]
        private Image goldIconTime;

        [SerializeField]
        private Image goldIconPieces;

        [SerializeField]
        private Image goldIconBuffs;

        [SerializeField]
        private Image goldIconBase;

        [SerializeField]
        private Image goldIconTotal;

        [Header("Divider")]
        [SerializeField]
        private GameObject totalDivider;

        [Header("Animation Settings")]
        [SerializeField]
        private float incrementDuration = 1f;

        [SerializeField]
        private AnimationCurve incrementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int currentIncomeTime;
        private int currentIncomePieces;
        private int currentIncomeBase;
        private int currentIncomeBuffs;
        private int currentIncomeTotal;

        private bool isAnimating;

        public void ShowRoundEndPanel(
            bool roundWon,
            int incomeTime,
            int incomePieces,
            int incomeBase,
            int incomeBuffs,
            List<BuffBase> buffs = null
        )
        {
            // Calculate income values
            this.currentIncomeTime = incomeTime;
            this.currentIncomePieces = incomePieces;
            this.currentIncomeBase = incomeBase;
            this.currentIncomeBuffs = incomeBuffs;

            // Calculate buffs if provided
            if (buffs != null)
            {
                this.currentIncomeBuffs += CalculateIncomeBuffs(buffs);
            }

            this.currentIncomeTotal = CalculateRoundIncome(
                this.currentIncomeTime,
                this.currentIncomePieces,
                this.currentIncomeBase,
                this.currentIncomeBuffs
            );

            // Setup UI based on roundWon
            this.SetupUI(roundWon);

            // Show panel and start animation
            this.roundEndPanel.SetActive(true);
            this.StartCoroutine(this.AnimateIncomeValues());
        }

        private void SetupUI(bool roundWon)
        {
            // Setup header
            this.headerTitle.text = roundWon ? "Round Won" : "Game Lost";

            // Setup footer button
            this.nextButton.gameObject.SetActive(false); // Hidden until animation completes
            this.nextButton.onClick.RemoveAllListeners();

            if (roundWon)
            {
                this.nextButtonText.text = $"Cash out: {this.currentIncomeTotal}";
                this.goldIconNext.gameObject.SetActive(true);
                this.nextButton.onClick.AddListener(() =>
                {
                    InventoryManager.Instance.Gold += this.currentIncomeTotal;
                    GameManager.LoadScene("Shop");
                });
            }
            else
            {
                this.nextButtonText.text = "Back to Main Menu";
                this.goldIconNext.gameObject.SetActive(false);
                this.nextButton.onClick.AddListener(() =>
                {
                    MusicManager.Instance.PlayClickSound();

                    Destroy(GameManager.Instance);
                    Destroy(InventoryManager.Instance.gameObject);
                    Destroy(ChessBoard.Instance.gameObject);

                    GameManager.LoadScene("MainMenu");
                });
            }

            // Reset all income displays
            this.roundIncomeTime.text = "0";
            this.roundIncomePieces.text = "0";
            this.roundIncomeBase.text = "0";
            this.roundIncomeBuffs.text = "0";
            this.roundIncomeTotal.text = "0";
        }

        private IEnumerator AnimateIncomeValues()
        {
            // TODO: // Hidden until animation completes of before

            if (this.isAnimating)
            {
                yield break;
            }

            this.isAnimating = true;

            // Animate Time Income
            yield return this.StartCoroutine(
                this.AnimateValue(this.roundIncomeTime, 0, this.currentIncomeTime)
            );

            // Animate Pieces Income
            yield return this.StartCoroutine(
                this.AnimateValue(this.roundIncomePieces, 0, this.currentIncomePieces)
            );

            // Animate Buffs Income
            yield return this.StartCoroutine(
                this.AnimateValue(this.roundIncomeBuffs, 0, this.currentIncomeBuffs)
            );

            // Animate Base Income
            yield return this.StartCoroutine(
                this.AnimateValue(this.roundIncomeBase, 0, this.currentIncomeBase)
            );

            // Animate Total Income (with slight delay for emphasis)
            yield return new WaitForSeconds(0.2f);
            yield return this.StartCoroutine(
                this.AnimateValue(this.roundIncomeTotal, 0, this.currentIncomeTotal)
            );

            // Show and enable the next button
            this.nextButton.gameObject.SetActive(true);
            this.nextButton.interactable = true;

            this.isAnimating = false;
        }

        private IEnumerator AnimateValue(
            TextMeshProUGUI textComponent,
            int startValue,
            int endValue
        )
        {
            float elapsedTime = 0f;

            while (elapsedTime < this.incrementDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / this.incrementDuration;
                float curveValue = this.incrementCurve.Evaluate(progress);

                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, curveValue));
                textComponent.text = currentValue.ToString();

                yield return null;
            }

            // Ensure final value is exact
            textComponent.text = endValue.ToString();
        }

        public void HideRoundEndPanel()
        {
            this.roundEndPanel.SetActive(false);
            this.isAnimating = false;
        }

        // Utility method to show panel with time-based income calculation
        public void ShowRoundEndPanel(
            bool roundWon,
            int countPiecesPlayer,
            int incomeBase,
            List<BuffBase> buffs = null
        )
        {
            int incomeTime = 0;
            int incomeBuffs = 0;

            if (this.timer != null)
            {
                incomeTime = CalculateIncomeTime(this.timer.TimeElapsed);
                this.roundTime.text = this.timer.TimerText.text;
            }

            if (buffs != null)
            {
                incomeBuffs = CalculateIncomeBuffs(buffs);
                this.roundPieces.text = countPiecesPlayer.ToString();
            }

            this.ShowRoundEndPanel(
                roundWon,
                incomeTime,
                countPiecesPlayer,
                incomeBase,
                incomeBuffs,
                buffs
            );
        }

        public static int CalculateRoundIncome(
            int incomeTime,
            int incomePieces,
            int baseIncome,
            int incomeBuffs
        )
        {
            return incomeTime + incomePieces + baseIncome + incomeBuffs;
        }

        public static int CalculateIncomeTime(float timeElapsed)
        {
            // Clamp timeElapsed between 0 and 600 seconds (10 minutes)
            float clampedTime = Mathf.Clamp(timeElapsed, 0f, 600f);
            // Calculate time-based income (linear decrease from 10 to 0)
            float timeIncome = 10f * (1f - (clampedTime / 600f));
            return Mathf.RoundToInt(timeIncome);
        }

        public static int CalculateIncomeBuffs(List<BuffBase> buffs)
        {
            // TODO: Implement buff income calculation
            int totalBuffIncome = 0;

            if (buffs != null)
            {
                foreach (var buff in buffs)
                {
                    // Add your buff income calculation logic here
                    // Example: totalBuffIncome += buff.IncomeBonus;
                }
            }

            return totalBuffIncome;
        }
    }
}
