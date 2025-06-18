namespace Assets.Scripts.UI.Buffs
{
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

        [Header("Easing")]
        public Ease PullEase = Ease.OutQuart;
        public Ease ReturnEase = Ease.OutQuart;

        public TextMeshProUGUI RerollCostTMP;

        private Vector2 originalPosition;
        private bool isAnimating;
        private int rerollCost = 3;
        private int rerollAddCost = 2;

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
            this.UpdateCost();

            // Store original transform values
            this.originalPosition = this.CordRect.anchoredPosition;
        }

        // Simple one-liner for quick use
        public void QuickCordPull()
        {
            if (this.isAnimating)
            {
                return;
            }

            this.isAnimating = true;

            this.CordButton.interactable = false;

            this.rerollCost += this.rerollAddCost;
            this.UpdateCost();

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
            if (this.RerollCostTMP != null)
            {
                this.RerollCostTMP.text = this.rerollCost.ToString();
            }
        }
    }
}
