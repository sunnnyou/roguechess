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
        public bool IsSelected { get; private set; } = false;

        private object currentItem;
        private Color originalColor;

        public void Initialize(int slotIndex, SlotType slotType)
        {
            SlotIndex = slotIndex;
            Type = slotType;
            IsEmpty = true;

            // Setup UI components
            SetupComponents();

            // Set initial appearance
            UpdateVisualState();
        }

        private void SetupComponents()
        {
            // Find components if not assigned
            if (itemIcon == null)
                itemIcon = transform.Find("ItemIcon")?.GetComponent<Image>();

            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();

            if (quantityText == null)
                quantityText = transform
                    .Find("QuantityPanel/QuantityText")
                    ?.GetComponent<TextMeshProUGUI>();

            if (quantityPanel == null)
                quantityPanel = transform.Find("QuantityPanel")?.gameObject;

            if (slotButton == null)
                slotButton = GetComponent<Button>();

            // Store original color
            if (backgroundImage != null)
                originalColor = backgroundImage.color;

            // Setup button if available
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(SlotIndex));
            }
        }

        public void SetItem(object item)
        {
            currentItem = item;
            IsEmpty = false;

            if (item is ChessPiece chessPiece)
            {
                SetChessPieceItem(chessPiece);
            }
            else if (item is BuffBase consumable)
            {
                SetConsumableItem(consumable);
            }

            UpdateVisualState();
        }

        private void SetChessPieceItem(ChessPiece chessPiece)
        {
            // Set icon based on chess piece type
            if (itemIcon != null)
            {
                // You'll need to assign appropriate sprites for each piece type
                Sprite pieceSprite = GetChessPieceSprite(chessPiece.PieceType);
                if (pieceSprite != null)
                {
                    itemIcon.sprite = pieceSprite;
                    itemIcon.color = Color.white;
                    itemIcon.enabled = true;
                }
                else
                {
                    // Fallback: show text representation
                    itemIcon.enabled = false;
                }
            }

            // Chess pieces typically don't have quantity
            if (quantityPanel != null)
                quantityPanel.SetActive(false);
        }

        private void SetConsumableItem(BuffBase consumable)
        {
            // Set icon for consumable
            if (itemIcon != null)
            {
                // You'll need to assign appropriate sprites for consumables
                Sprite consumableSprite = GetConsumableSprite(consumable);
                if (consumableSprite != null)
                {
                    itemIcon.sprite = consumableSprite;
                    itemIcon.color = Color.white;
                    itemIcon.enabled = true;
                }
                else
                {
                    // Fallback: show default consumable icon
                    itemIcon.enabled = false;
                }
            }

            // Show quantity if applicable
            if (quantityPanel != null && quantityText != null)
            {
                // If your consumables have quantity, show it here
                // For now, we'll hide it
                quantityPanel.SetActive(false);
            }
        }

        public void ClearItem()
        {
            currentItem = null;
            IsEmpty = true;

            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (quantityPanel != null)
                quantityPanel.SetActive(false);

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (backgroundImage == null)
                return;

            Color targetColor;

            if (IsSelected)
            {
                targetColor = selectedColor;
            }
            else if (IsEmpty)
            {
                targetColor = emptySlotColor;
            }
            else
            {
                targetColor = filledSlotColor;
            }

            backgroundImage.color = targetColor;
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            UpdateVisualState();
        }

        #region Event Handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(SlotIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (backgroundImage != null && !IsSelected)
            {
                backgroundImage.color = hoverColor;
            }

            OnSlotHovered?.Invoke(SlotIndex);

            // Show tooltip if item exists
            if (!IsEmpty)
            {
                ShowTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsSelected)
            {
                UpdateVisualState();
            }

            OnSlotExited?.Invoke(SlotIndex);

            // Hide tooltip
            HideTooltip();
        }

        #endregion

        #region Tooltip Methods

        private void ShowTooltip()
        {
            if (currentItem == null)
                return;

            string tooltipText = GetTooltipText();

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
            if (currentItem is ChessPiece chessPiece)
            {
                return $"{chessPiece.PieceType}\n"
                    + $"IsWhite: {chessPiece.IsWhite}\n"
                    + $"Click to view details";
            }
            else if (currentItem is BuffBase consumable)
            {
                return $"Consumable Item\n" + $"Click to use";
            }

            return "Unknown Item";
        }

        #endregion

        #region Sprite Helpers

        private Sprite GetChessPieceSprite(ChessPieceType pieceType)
        {
            // You'll need to implement this based on your sprite system
            // This is a placeholder - replace with your actual sprite loading logic

            // Example implementation:
            // return Resources.Load<Sprite>($"ChessPieces/{pieceType}");

            // For now, return null and let the system handle it
            return null;
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

        #endregion

        #region Public Utility Methods

        public T GetItem<T>()
            where T : class
        {
            return currentItem as T;
        }

        public bool HasItem()
        {
            return !IsEmpty && currentItem != null;
        }

        public Type GetItemType()
        {
            return currentItem?.GetType();
        }

        #endregion
    }
}
