namespace Assets.Scripts.UI.Buffs
{
    using System.Threading.Tasks;
    using Assets.Scripts.Game.Buffs;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PaperScrollAnimator : MonoBehaviour
    {
        public RectTransform ScrollPanel;
        public AudioSource ScrollSoundOpen;
        public AudioSource ScrollSoundClose;

        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI CostText;
        public Image BuffImage;

        [Header("Data Source - Choose One")]
        public BuffDatabase BuffDatabase; // For ScriptableObject approach

        private IBuff currentBuff;

        private bool isOpen;
        private bool isAnimating; // Prevent multiple simultaneous animations

        public void Start()
        {
            // hide initially
            this.ScrollPanel.localScale = new Vector3(1, 0, 1);
        }

        public async Task OpenScrollAsync(bool playSound = true)
        {
            if (this.isOpen || this.isAnimating)
            {
                return;
            }

            this.isOpen = true;

            // Reset rotation first (important for consistency)
            this.ScrollPanel.rotation = Quaternion.identity;

            // Open scroll and await completion
            await this
                .ScrollPanel.DOScaleY(1f, 0.2f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();

            if (this.ScrollSoundOpen != null && playSound)
            {
                this.ScrollSoundOpen.Play();
            }

            this.isAnimating = false;
        }

        public async Task CloseScrollAsync(bool playSound = true)
        {
            if (!this.isOpen || this.isAnimating)
            {
                return;
            }

            this.isOpen = false;

            // Reset rotation first (important for consistency)
            this.ScrollPanel.rotation = Quaternion.identity;

            // Play closing sound
            if (this.ScrollSoundClose != null && playSound)
            {
                this.ScrollSoundClose.Play();
            }

            // Shrink Y scale to simulate closing and await completion
            await this
                .ScrollPanel.DOScaleY(0f, 0.15f)
                .SetEase(Ease.InBack)
                .AsyncWaitForCompletion();

            this.isAnimating = false;
        }

        // Synchronous versions for backward compatibility
        public void OpenScroll()
        {
            _ = this.OpenScrollAsync();
        }

        public void CloseScroll()
        {
            _ = this.CloseScrollAsync();
        }

        public async void Reroll()
        {
            if (this.isAnimating)
            {
                return; // Prevent overlapping rerolls
            }

            if (this.ScrollSoundOpen)
            {
                this.ScrollSoundOpen.Play();
            }

            // Close scroll and wait for it to complete
            await this.CloseScrollAsync(false);

            // Display new buff content
            this.DisplayRandomBuff();

            // Open scroll and wait for it to complete
            await this.OpenScrollAsync(false);
        }

        public async Task WiggleAsync()
        {
            if (this.isAnimating)
            {
                return;
            }

            await this
                .ScrollPanel.DORotate(new Vector3(0, 0, -1f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();

            this.isAnimating = false;
        }

        // Synchronous version for backward compatibility
        public void Wiggle()
        {
            _ = this.WiggleAsync();
        }

        public void DisplayRandomBuff()
        {
            // Try ScriptableObject first, then BuffManager
            if (this.BuffDatabase != null)
            {
                this.currentBuff = this.BuffDatabase.GetRandomBuff();
            }
            else
            {
                Debug.LogError("No buff data source found! Assign a BuffDatabase.");
                return;
            }

            // Update UI elements
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            if (this.currentBuff == null)
            {
                return;
            }

            // Update text components
            if (this.TitleText != null)
            {
                this.TitleText.text = this.currentBuff.BuffName;
            }

            if (this.DescriptionText != null)
            {
                this.DescriptionText.text = this.currentBuff.Description;
            }

            if (this.CostText != null)
            {
                this.CostText.text = this.currentBuff.Cost.ToString();
            }

            // Update image
            if (this.BuffImage != null && this.currentBuff.Icon != null)
            {
                this.BuffImage.sprite = this.currentBuff.Icon;
            }
        }

        // Public method to get the current buff (useful for other systems)
        public IBuff GetCurrentBuff()
        {
            return this.currentBuff;
        }

        // Method to apply the buff (you can expand this based on your game logic)
        public void ApplyCurrentBuff()
        {
            if (this.currentBuff != null)
            {
                Debug.Log($"Applied buff: {this.currentBuff.BuffName}");
                // Add your buff application logic here
            }
        }
    }
}
