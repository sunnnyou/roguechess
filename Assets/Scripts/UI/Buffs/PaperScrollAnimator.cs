namespace Assets.Scripts.UI.Buffs
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PaperScrollAnimator : MonoBehaviour
    {
        public RectTransform ScrollPanel;
        public AudioSource ScrollSoundOpen;
        public AudioSource ScrollSoundClose;

        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI costText;
        public Image buffImage;

        [Header("Data Source - Choose One")]
        public BuffDatabase buffDatabase; // For ScriptableObject approach

        private Buff currentBuff;

        private bool isOpen;

        public void Start()
        {
            // hide initially
            this.ScrollPanel.localScale = new Vector3(1, 0, 1);
        }

        public void OpenScroll()
        {
            if (this.isOpen)
            {
                return;
            }

            this.isOpen = true;

            // Reset rotation first (important for consistency)
            this.ScrollPanel.rotation = Quaternion.identity;

            // Open scroll
            this.ScrollPanel.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack);

            if (this.ScrollSoundOpen != null)
            {
                this.ScrollSoundOpen.Play();
            }
        }

        public void CloseScroll()
        {
            if (!this.isOpen)
            {
                return;
            }

            this.isOpen = false;

            // Reset rotation first (important for consistency)
            this.ScrollPanel.rotation = Quaternion.identity;

            // Shrink Y scale to simulate closing
            this.ScrollPanel.DOScaleY(0f, 0.15f).SetEase(Ease.InBack);

            // Play closing sound
            if (this.ScrollSoundClose != null)
            {
                this.ScrollSoundClose.Play();
            }
        }

        public void Reroll()
        {
            this.CloseScroll();
            this.DisplayRandomBuff();
            this.OpenScroll();
        }

        public void Wiggle()
        {
            this.ScrollPanel.DORotate(new Vector3(0, 0, -1f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public void DisplayRandomBuff()
        {
            // Try ScriptableObject first, then BuffManager
            if (buffDatabase != null)
            {
                currentBuff = buffDatabase.GetRandomBuff();
            }
            else if (BuffManager.Instance != null)
            {
                currentBuff = BuffManager.Instance.GetRandomBuff();
            }
            else
            {
                Debug.LogError(
                    "No buff data source found! Assign a BuffDatabase or ensure BuffManager exists in scene."
                );
                return;
            }

            // Update UI elements
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (currentBuff == null)
                return;

            // Update text components
            if (titleText != null)
                titleText.text = currentBuff.title;

            if (descriptionText != null)
                descriptionText.text = currentBuff.description;

            if (costText != null)
                costText.text = currentBuff.cost.ToString();

            // Update image
            if (buffImage != null && currentBuff.image != null)
                buffImage.sprite = currentBuff.image;
        }

        // Public method to get the current buff (useful for other systems)
        public Buff GetCurrentBuff()
        {
            return currentBuff;
        }

        // Method to apply the buff (you can expand this based on your game logic)
        public void ApplyCurrentBuff()
        {
            if (currentBuff != null)
            {
                Debug.Log($"Applied buff: {currentBuff.title}");
                // Add your buff application logic here
            }
        }
    }
}
