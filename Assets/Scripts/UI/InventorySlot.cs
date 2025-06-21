namespace Assets.Scripts.UI
{
    using System;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class InventorySlot
        : MonoBehaviour,
            IPointerClickHandler,
            IPointerEnterHandler,
            IPointerExitHandler
    {
        [Header("UI Components")]
        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private TextMeshProUGUI quantityText;

        [SerializeField]
        private GameObject quantityPanel;

        [SerializeField]
        private Button slotButton;

        [Header("Visual Settings")]
        [SerializeField]
        private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        [SerializeField]
        private Color filledSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        [SerializeField]
        private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);

        [SerializeField]
        private Color selectedColor = new Color(0.6f, 0.4f, 0.2f, 0.8f);

        public enum SlotType
        {
            ChessPiece,
            Consumable,
        }

        // Events
        public Action<int> OnSlotClicked;
        public Action<int> OnSlotHovered;
        public Action<int> OnSlotExited;

        // Properties
        public int SlotIndex { get; private set; }
        public SlotType Type { get; private set; }
        public bool IsEmpty { get; private set; } = true;
        public bool IsSelected { get; private set; }

        private object currentItem;
        private Color originalColor;

        public void Initialize(int slotIndex, SlotType slotType)
        {
            this.SlotIndex = slotIndex;
            this.Type = slotType;
            this.IsEmpty = true;

            // Setup UI components
            this.SetupComponents();

            // Set initial appearance
            this.UpdateVisualState();
        }

        private void SetupComponents()
        {
            // Find components if not assigned
            if (this.itemIcon == null)
            {
                this.itemIcon = this.transform.Find("ItemIcon")?.GetComponent<Image>();
            }

            if (this.backgroundImage == null)
            {
                this.backgroundImage = this.GetComponent<Image>();
            }

            if (this.quantityText == null)
            {
                this.quantityText = this
                    .transform.Find("QuantityPanel/QuantityText")
                    ?.GetComponent<TextMeshProUGUI>();
            }

            if (this.quantityPanel == null)
            {
                this.quantityPanel = this.transform.Find("QuantityPanel")?.gameObject;
            }

            if (this.slotButton == null)
            {
                this.slotButton = this.GetComponent<Button>();
            }

            // Store original color
            if (this.backgroundImage != null)
            {
                this.originalColor = this.backgroundImage.color;
            }

            // Setup button if available
            if (this.slotButton != null)
            {
                this.slotButton.onClick.AddListener(() => this.OnSlotClicked?.Invoke(this.SlotIndex)
                );
            }
        }

        public void SetItem(object item)
        {
            this.currentItem = item;
            this.IsEmpty = false;

            if (item is ChessPiece chessPiece)
            {
                this.SetChessPieceItem(chessPiece);
            }
            else if (item is BuffBase consumable)
            {
                this.SetConsumableItem(consumable);
            }

            this.UpdateVisualState();
        }

        private void SetChessPieceItem(ChessPiece chessPiece)
        {
            // Set icon based on chess piece type
            if (this.itemIcon != null)
            {
                // You'll need to assign appropriate sprites for each piece type
                Sprite pieceSprite = GetChessPieceSprite(chessPiece);
                if (pieceSprite != null)
                {
                    this.itemIcon.sprite = pieceSprite;
                    this.itemIcon.color = Color.white;
                    this.itemIcon.enabled = true;
                }
                else
                {
                    // Fallback: show text representation
                    this.itemIcon.enabled = false;
                }
            }

            // Chess pieces typically don't have quantity
            if (this.quantityPanel != null)
            {
                this.quantityPanel.SetActive(false);
            }
        }

        private void SetConsumableItem(BuffBase consumable)
        {
            // Set icon for consumable
            if (this.itemIcon != null)
            {
                // You'll need to assign appropriate sprites for consumables
                Sprite consumableSprite = this.GetConsumableSprite(consumable);
                if (consumableSprite != null)
                {
                    this.itemIcon.sprite = consumableSprite;
                    this.itemIcon.color = Color.white;
                    this.itemIcon.enabled = true;
                }
                else
                {
                    // Fallback: show default consumable icon
                    this.itemIcon.enabled = false;
                }
            }

            // Show quantity if applicable
            if (this.quantityPanel != null && this.quantityText != null)
            {
                // If your consumables have quantity, show it here
                // For now, we'll hide it
                this.quantityPanel.SetActive(false);
            }
        }

        public void ClearItem()
        {
            this.currentItem = null;
            this.IsEmpty = true;

            if (this.itemIcon != null)
            {
                this.itemIcon.sprite = null;
                this.itemIcon.enabled = false;
            }

            if (this.quantityPanel != null)
            {
                this.quantityPanel.SetActive(false);
            }

            this.UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (this.backgroundImage == null)
            {
                return;
            }

            Color targetColor;

            if (this.IsSelected)
            {
                targetColor = this.selectedColor;
            }
            else if (this.IsEmpty)
            {
                targetColor = this.emptySlotColor;
            }
            else
            {
                targetColor = this.filledSlotColor;
            }

            this.backgroundImage.color = targetColor;
        }

        public void SetSelected(bool selected)
        {
            this.IsSelected = selected;
            this.UpdateVisualState();
        }

        // Event Handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            this.OnSlotClicked?.Invoke(this.SlotIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.backgroundImage != null && !this.IsSelected)
            {
                this.backgroundImage.color = this.hoverColor;
            }

            this.OnSlotHovered?.Invoke(this.SlotIndex);

            // Show tooltip if item exists
            if (!this.IsEmpty)
            {
                this.ShowTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.IsSelected)
            {
                this.UpdateVisualState();
            }

            this.OnSlotExited?.Invoke(this.SlotIndex);

            // Hide tooltip
            this.HideTooltip();
        }

        // Tooltip Methods

        private void ShowTooltip()
        {
            if (this.currentItem == null)
            {
                return;
            }

            string tooltipText = this.GetTooltipText();

            // You can implement a tooltip system here
            // For now, we'll just log it
            Debug.Log($"Tooltip: {tooltipText}");

            // If you have a tooltip manager, call it here:
            // TooltipManager.Instance.ShowTooltip(tooltipText, transform.position);
        }

        private void HideTooltip()
        {
            // Hide tooltip
            // TooltipManager.Instance.HideTooltip();
        }

        private string GetTooltipText()
        {
            if (this.currentItem is ChessPiece chessPiece)
            {
                return $"{chessPiece.PieceType}\n"
                    + $"IsWhite: {chessPiece.IsWhite}\n"
                    + $"Click to view details";
            }
            else if (this.currentItem is BuffBase consumable)
            {
                return $"Consumable Item\n" + $"Click to use";
            }

            return "Unknown Item";
        }

        // Sprite Helpers

        private static Sprite GetChessPieceSprite(ChessPiece chessPiece)
        {
            return chessPiece.SpriteRenderer.sprite;
        }

        private Sprite GetConsumableSprite(BuffBase consumable)
        {
            // You'll need to implement this based on your sprite system
            // This is a placeholder - replace with your actual sprite loading logic

            // Example implementation:
            // return Resources.Load<Sprite>("Consumables/DefaultConsumable");

            // For now, return null and let the system handle it
            return null;
        }

        // Public Utility Methods

        public T GetItem<T>()
            where T : class
        {
            return this.currentItem as T;
        }

        public bool HasItem()
        {
            return !this.IsEmpty && this.currentItem != null;
        }

        public Type GetItemType()
        {
            return this.currentItem?.GetType();
        }
    }
}
