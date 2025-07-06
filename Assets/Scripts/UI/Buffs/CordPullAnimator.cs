namespace Assets.Scripts.UI.Buffs
{
    using Assets.Scripts.Game;
    using Assets.Scripts.Game.Player;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CordPullAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        public RectTransform CordRect;
        public Button CordButton;
        public float PullDistance = 100f;
        public float PullDuration = 0.3f;
        public float ReturnDuration = 0.5f;

        public PaperScrollAnimator PaperScrollAnimator1;
        public PaperScrollAnimator PaperScrollAnimator2;
        public PaperScrollAnimator PaperScrollAnimator3;
        public PaperScrollAnimator PaperScrollAnimator4;

        [Header("Easing")]
        public Ease PullEase = Ease.OutQuart;
        public Ease ReturnEase = Ease.OutQuart;

        public TextMeshProUGUI RerollCostTMP;

        private Vector2 originalPosition;
        private bool isAnimating;
        private int rerollCost = 3;
        private int rerollAddCost = 2;
        private bool inactive;

        public void Start()
        {
            if (this.CordRect == null)
            {
                this.CordRect = this.GetComponent<RectTransform>();
            }
            if (this.CordButton == null)
            {
                this.CordButton = this.GetComponent<Button>();
            }
            if (this.RerollCostTMP != null)
            {
                this.RerollCostTMP.text = this.rerollCost.ToString();
            }
            // Store original transform values
            this.originalPosition = this.CordRect.anchoredPosition;

            MusicManager.Instance.PlayScrollSoundOpen();

            this.CordButton.onClick.AddListener(this.PaperScrollAnimator1.Reroll);
            this.CordButton.onClick.AddListener(this.PaperScrollAnimator2.Reroll);
            this.CordButton.onClick.AddListener(this.PaperScrollAnimator3.Reroll);
            this.CordButton.onClick.AddListener(this.PaperScrollAnimator4.Reroll);

            this.SetupEventListeners();

            this.CheckGoldRequirement();
        }

        private void OnDestroy()
        {
            this.RemoveEventListeners();
        }

        // Event Listeners
        private void SetupEventListeners()
        {
            InventoryManager.OnGoldChanged += this.OnGoldChanged;
        }

        private void RemoveEventListeners()
        {
            InventoryManager.OnGoldChanged -= this.OnGoldChanged;
        }

        public void QuickCordPull()
        {
            if (this.isAnimating)
            {
                return;
            }

            this.isAnimating = true;

            this.CordButton.interactable = false;

            this.UpdateCost();

            if (!this.inactive)
            {
                MusicManager.Instance.PlayScrollSoundOpen();
            }

            this.CordRect.DOAnchorPosY(
                    this.originalPosition.y - this.PullDistance,
                    this.PullDuration
                )
                .SetEase(this.PullEase)
                .OnComplete(() =>
                {
                    this.CordRect.DOAnchorPosY(this.originalPosition.y, this.ReturnDuration)
                        .SetEase(this.ReturnEase)
                        .OnComplete(() =>
                        {
                            this.isAnimating = false;
                            this.CordButton.interactable = true;
                        });
                });
        }

        private void UpdateCost()
        {
            if (!this.inactive)
            {
                InventoryManager.Instance.SpendGold(this.rerollCost);
                this.rerollCost += this.rerollAddCost;
                if (this.RerollCostTMP != null)
                {
                    this.RerollCostTMP.text = this.rerollCost.ToString();
                }
            }
        }

        private void CheckGoldRequirement()
        {
            if (!InventoryManager.Instance.HasEnoughGold(this.rerollCost))
            {
                this.CordButton.onClick.RemoveAllListeners();
                this.inactive = true;
            }
        }

        private void OnGoldChanged(int newGoldAmount)
        {
            this.CheckGoldRequirement();
        }
    }
}
