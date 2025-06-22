namespace Assets.Scripts.UI.Buffs
{
    using System.Threading.Tasks;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Player;
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
        public BuffDatabase BuffDatabase;

        private BuffBase currentBuff;

        private bool isOpen;
        private bool isAnimating; // Prevent multiple simultaneous animations
        private Color enabledColor;
        private Color disabledColor;
        private bool enoughGold;
        private bool enoughSpace;
        private bool changed;

        public void Start()
        {
            // hide initially
            this.ScrollPanel.localScale = new Vector3(1, 0, 1);

            // Setup colors
            var image = this.GetComponent<Image>();
            this.enabledColor = image.color;
            this.disabledColor = new Color(
                image.color.r * 0.7f,
                image.color.g * 0.7f,
                image.color.b * 0.7f
            ); // Slightly darker

            // Setup Event listener
            this.SetupEventListeners();

            // Display new buff content
            this.DisplayRandomBuff();

            // Open scroll and wait for it to complete
            this.OpenScrollAsync(false);

            this.enoughGold = InventoryManager.Instance.HasEnoughGold(this.currentBuff.Cost);
            this.enoughSpace = !InventoryManager.Instance.IsConsumableInventoryFull();
            this.changed = true;

            this.EnableBuying();
        }

        private void OnDestroy()
        {
            this.RemoveEventListeners();
        }

        // Event Listeners
        private void SetupEventListeners()
        {
            InventoryManager.OnGoldChanged += this.OnGoldChanged;
            InventoryManager.OnInventoryChanged += this.OnInventoryChanged;
        }

        private void RemoveEventListeners()
        {
            InventoryManager.OnGoldChanged -= this.OnGoldChanged;
            InventoryManager.OnInventoryChanged -= this.OnInventoryChanged;
        }

        // Event Handlers

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

            Button button = this.GetComponent<Button>();
            button.interactable = true;

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

        public void Wiggle()
        {
            if (this.isAnimating)
            {
                return;
            }

            this.ScrollPanel.rotation = Quaternion.identity;

            this.ScrollPanel.DORotate(new Vector3(0, 0, -1f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    this.isAnimating = false;
                    this.ScrollPanel.rotation = Quaternion.identity;
                });
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
        public BuffBase GetCurrentBuff()
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

        private void BuyBuff()
        {
            InventoryManager.Instance.SpendGold(this.currentBuff.Cost);
            InventoryManager.Instance.AddConsumable(this.currentBuff);
            this.CloseScroll();
            Button button = this.GetComponent<Button>();
            button.interactable = false;
        }

        private void DisableBuying()
        {
            if ((!this.enoughGold || !this.enoughSpace) && this.changed)
            {
                Button button = this.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(this.Wiggle);

                var image = this.GetComponent<Image>();
                image.color = this.disabledColor;
                this.changed = false;
            }
        }

        private void EnableBuying()
        {
            if (this.enoughGold && this.enoughSpace && this.changed)
            {
                Button button = this.GetComponent<Button>();
                button.onClick.AddListener(this.BuyBuff);

                var image = this.GetComponent<Image>();
                image.color = this.enabledColor;
                this.changed = false;
            }
        }

        private void OnGoldChanged(int newGoldAmount)
        {
            if (this.currentBuff == null)
            {
                return;
            }

            var uptEnoughGold = InventoryManager.Instance.HasEnoughGold(this.currentBuff.Cost);
            this.changed = uptEnoughGold != this.enoughGold;
            this.enoughGold = uptEnoughGold;

            if (this.enoughGold)
            {
                this.EnableBuying();
            }
            else
            {
                this.DisableBuying();
            }
        }

        private void OnInventoryChanged()
        {
            var uptEnoughSpace = !InventoryManager.Instance.IsConsumableInventoryFull();
            this.changed = uptEnoughSpace != this.enoughSpace;
            this.enoughSpace = uptEnoughSpace;
            if (this.enoughSpace)
            {
                this.EnableBuying();
            }
            else
            {
                this.DisableBuying();
            }
        }
    }
}
