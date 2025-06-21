namespace Assets.Scripts.UI
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class InventoryUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GameObject inventoryPanel;

        [SerializeField]
        private Button inventoryToggleButton;

        [SerializeField]
        private Button closeInventoryButton;

        [Header("Gold Display")]
        [SerializeField]
        private TextMeshProUGUI goldTextInventory;

        [SerializeField]
        private TextMeshProUGUI goldTextMenu;

        [Header("Chess Pieces")]
        [SerializeField]
        private Transform chessPieceContainer;

        [SerializeField]
        private GameObject chessPieceSlotPrefab;

        [SerializeField]
        private TextMeshProUGUI chessPieceCountText;

        [Header("Consumables")]
        [SerializeField]
        private Transform consumableContainer;

        [SerializeField]
        private GameObject consumableSlotPrefab;

        [SerializeField]
        private TextMeshProUGUI consumableCountText;

        [Header("Settings")]
        [SerializeField]
        private KeyCode inventoryToggleKey = KeyCode.I;

        [SerializeField]
        private bool startClosed = true;

        private readonly List<InventorySlot> chessPieceSlots = new();
        private readonly List<InventorySlot> consumableSlots = new();
        private bool isInventoryOpen;

        private void Start()
        {
            this.InitializeUI();
            this.SetupEventListeners();
            this.RefreshInventoryDisplay();

            if (this.startClosed)
            {
                this.CloseInventory();
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[Key.I].wasPressedThisFrame)
            {
                this.ToggleInventory();
            }
        }

        private void OnDestroy()
        {
            this.RemoveEventListeners();
        }

        // Initialization

        private void InitializeUI()
        {
            // Setup button listeners
            if (this.inventoryToggleButton != null)
            {
                this.inventoryToggleButton.onClick.AddListener(this.ToggleInventory);
            }

            if (this.closeInventoryButton != null)
            {
                this.closeInventoryButton.onClick.AddListener(this.CloseInventory);
            }

            // Create chess piece slots
            this.CreateChessPieceSlots();

            // Create consumable slots
            this.CreateConsumableSlots();
        }

        private void CreateChessPieceSlots()
        {
            if (this.chessPieceContainer == null || this.chessPieceSlotPrefab == null)
            {
                Debug.LogError("Chess piece container or slot prefab is not assigned!");
                return;
            }

            int maxSlots = InventoryManager.Instance.MaxChessPieceSlots;
            var chessPieceList = InventoryManager.Instance.GetAllChessPieces();

            for (int i = 0; i < maxSlots; i++)
            {
                GameObject slotObj = Instantiate(
                    this.chessPieceSlotPrefab,
                    this.chessPieceContainer
                );
                if (!slotObj.TryGetComponent<InventorySlot>(out var slot))
                {
                    slot = slotObj.AddComponent<InventorySlot>();
                }
                slot.Initialize(i, InventorySlot.SlotType.ChessPiece);
                slot.OnSlotClicked += this.OnChessPieceSlotClicked;

                // Make slots darker in a chessboard pattern
                int row = i / 8;
                int col = i % 8;
                if ((row + col) % 2 == 0)
                {
                    var backgroundChild = slotObj.transform.Find("Background");
                    if (backgroundChild != null)
                    {
                        if (backgroundChild.TryGetComponent<Image>(out var image))
                        {
                            image.color = new Color(
                                image.color.r * 0.5525606f,
                                image.color.g * 0.4851331f,
                                image.color.b * 0.3209617f,
                                image.color.a
                            );
                        }
                    }
                }

                // var iconChild = slotObj.transform.Find("ItemIcon");
                // ChessPiece chessPiece = chessPieceList.ElementAtOrDefault(i);

                // if (iconChild != null && chessPiece != null)
                // {
                //     if (iconChild.TryGetComponent<Image>(out var image))
                //     {
                //         image.sprite = chessPiece.SpriteRenderer.sprite;
                //         image.gameObject.SetActive(true);
                //     }
                // }

                this.chessPieceSlots.Add(slot);
            }
        }

        private void CreateConsumableSlots()
        {
            if (this.consumableContainer == null || this.consumableSlotPrefab == null)
            {
                Debug.LogError("Consumable container or slot prefab is not assigned!");
                return;
            }

            int maxSlots = InventoryManager.Instance.MaxConsumableSlots;

            for (int i = 0; i < maxSlots; i++)
            {
                GameObject slotObj = Instantiate(
                    this.consumableSlotPrefab,
                    this.consumableContainer
                );

                if (!slotObj.TryGetComponent<InventorySlot>(out var slot))
                {
                    slot = slotObj.AddComponent<InventorySlot>();
                }

                slot.Initialize(i, InventorySlot.SlotType.Consumable);
                slot.OnSlotClicked += this.OnConsumableSlotClicked;
                this.consumableSlots.Add(slot);
            }
        }

        // Event Listeners

        private void SetupEventListeners()
        {
            InventoryManager.OnChessPieceAdded += this.OnChessPieceAdded;
            InventoryManager.OnChessPieceRemoved += this.OnChessPieceRemoved;
            InventoryManager.OnConsumableAdded += this.OnConsumableAdded;
            InventoryManager.OnConsumableRemoved += this.OnConsumableRemoved;
            InventoryManager.OnGoldChanged += this.OnGoldChanged;
            InventoryManager.OnInventoryChanged += this.OnInventoryChanged;
        }

        private void RemoveEventListeners()
        {
            InventoryManager.OnChessPieceAdded -= this.OnChessPieceAdded;
            InventoryManager.OnChessPieceRemoved -= this.OnChessPieceRemoved;
            InventoryManager.OnConsumableAdded -= this.OnConsumableAdded;
            InventoryManager.OnConsumableRemoved -= this.OnConsumableRemoved;
            InventoryManager.OnGoldChanged -= this.OnGoldChanged;
            InventoryManager.OnInventoryChanged -= this.OnInventoryChanged;
        }

        // Event Handlers

        private void OnChessPieceAdded(ChessPiece piece)
        {
            this.RefreshChessPieceDisplay();
        }

        private void OnChessPieceRemoved(ChessPiece piece)
        {
            this.RefreshChessPieceDisplay();
        }

        private void OnConsumableAdded(BuffBase consumable)
        {
            this.RefreshConsumableDisplay();
        }

        private void OnConsumableRemoved(BuffBase consumable)
        {
            this.RefreshConsumableDisplay();
        }

        private void OnGoldChanged(int newGoldAmount)
        {
            this.UpdateGoldDisplay();
        }

        private void OnInventoryChanged()
        {
            this.RefreshInventoryDisplay();
        }

        private void OnChessPieceSlotClicked(int slotIndex)
        {
            ChessPiece piece = InventoryManager.Instance.GetChessPieceAt(slotIndex);
            if (piece != null)
            {
                Debug.Log($"Clicked on chess piece: {piece.PieceType} at slot {slotIndex}");
                // Add your chess piece interaction logic here
                // For example: show details, equip, or use the piece
            }
        }

        private void OnConsumableSlotClicked(int slotIndex)
        {
            var consumable = InventoryManager.Instance.GetConsumableAt(slotIndex);
            if (consumable != null)
            {
                Debug.Log($"Clicked on consumable at slot {slotIndex}");
                // Use the consumable
                _ = InventoryManager.Instance.UseConsumable(slotIndex);
            }
        }

        // UI Updates

        public void RefreshInventoryDisplay()
        {
            this.UpdateGoldDisplay();
            this.RefreshChessPieceDisplay();
            this.RefreshConsumableDisplay();
        }

        private void UpdateGoldDisplay()
        {
            if (this.goldTextInventory != null)
            {
                this.goldTextInventory.text = $"Gold: {InventoryManager.Instance.Gold}";
            }
            if (this.goldTextMenu != null)
            {
                this.goldTextMenu.text = $"Gold: {InventoryManager.Instance.Gold}";
            }
        }

        private void RefreshChessPieceDisplay()
        {
            var chessPieces = InventoryManager.Instance.GetAllChessPieces();
            int chessPieceCount = InventoryManager.Instance.GetChessPieceCount();

            // Update slots
            for (int i = 0; i < this.chessPieceSlots.Count; i++)
            {
                if (i < chessPieceCount)
                {
                    this.chessPieceSlots[i].SetItem(chessPieces[i]);
                }
                else
                {
                    this.chessPieceSlots[i].ClearItem();
                }
            }

            // Update count text
            if (this.chessPieceCountText != null)
            {
                this.chessPieceCountText.text =
                    $"Chess Pieces: {chessPieceCount}/{InventoryManager.Instance.MaxChessPieceSlots}";
            }
        }

        private void RefreshConsumableDisplay()
        {
            var consumables = InventoryManager.Instance.GetAllConsumables();

            // Update slots
            for (int i = 0; i < this.consumableSlots.Count; i++)
            {
                if (i < consumables.Count)
                {
                    this.consumableSlots[i].SetItem(consumables[i]);
                }
                else
                {
                    this.consumableSlots[i].ClearItem();
                }
            }

            // Update count text
            if (this.consumableCountText != null)
            {
                this.consumableCountText.text =
                    $"Consumables: {consumables.Count}/{InventoryManager.Instance.MaxConsumableSlots}";
            }
        }

        // Public Methods

        public void ToggleInventory()
        {
            if (this.isInventoryOpen)
            {
                this.CloseInventory();
            }
            else
            {
                this.OpenInventory();
            }
        }

        public void OpenInventory()
        {
            if (this.inventoryPanel != null)
            {
                this.inventoryPanel.SetActive(true);
                this.isInventoryOpen = true;
                this.RefreshInventoryDisplay();
                Debug.Log("Inventory opened");
            }
        }

        public void CloseInventory()
        {
            if (this.inventoryPanel != null)
            {
                this.inventoryPanel.SetActive(false);
                this.isInventoryOpen = false;
                Debug.Log("Inventory closed");
            }
        }
    }
}
