namespace Assets.Scripts.UI
{
    using System.Collections.Generic;
    using Assets.Scripts.Game;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class InventoryUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GameObject inventoryPanel;

        [SerializeField]
        private Button closeInventoryButton;

        [Header("Gold Display")]
        [SerializeField]
        private TextMeshProUGUI goldText;

        [Header("Chess Pieces")]
        [SerializeField]
        private Transform chessPieceContainer;

        [SerializeField]
        private GameObject chessPieceSlotPrefab;

        [SerializeField]
        private TextMeshProUGUI chessPieceCountText;

        [SerializeField]
        private Sprite tileBlack;

        [SerializeField]
        private Sprite tileWhite;

        [Header("Consumables")]
        [SerializeField]
        private Transform consumableContainer;

        [SerializeField]
        private Transform consumableContainerCopy;

        [SerializeField]
        private GameObject consumableSlotPrefab;

        [SerializeField]
        private TextMeshProUGUI consumableCountText;

        [Header("Settings")]
        [SerializeField]
        private bool startClosed = true;

        [Header("Consumable Usage")]
        [SerializeField]
        private Color highlightColor = Color.green;

        private readonly List<InventorySlot> chessPieceSlots = new();
        private readonly List<InventorySlot> consumableSlots = new();
        private readonly List<InventorySlot> consumableSlotsCopy = new();
        private bool isInventoryOpen;

        // Consumable usage state
        private bool isConsumableSelectionMode;
        private BuffBase selectedConsumable;
        private int selectedConsumableSlotIndex = -1;
        private List<GameObject> highlightedObjects = new();
        private List<Color> originalColors = new();

        private void Start()
        {
            bool draggable = SceneManager.GetActiveScene().name != "Game";

            this.InitializeUI(draggable);
            this.SetupEventListeners();
            this.RefreshInventoryDisplay();

            // Handle scene-specific inventory logic

            if (this.startClosed)
            {
                this.CloseInventory();
            }
        }

        private void Update()
        {
            // Handle consumable selection mode
            if (this.isConsumableSelectionMode)
            {
                this.HandleConsumableSelectionInput();
            }
        }

        private void OnDestroy()
        {
            this.RemoveEventListeners();
        }

        // Initialization

        private void InitializeUI(bool draggable)
        {
            // Setup button listeners
            if (this.closeInventoryButton != null)
            {
                this.closeInventoryButton.onClick.AddListener(() =>
                {
                    MusicManager.Instance.PlayClickSound();
                    this.CloseInventory();
                });
            }

            // Create chess piece slots
            this.CreateChessPieceSlots(draggable);

            // Create consumable slots
            this.CreateConsumableSlots();
        }

        private void CreateChessPieceSlots(bool draggable)
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
                slot.Initialize(i, InventorySlot.SlotType.ChessPiece, draggable);
                slot.OnSlotClicked += this.OnChessPieceSlotClicked;

                // Chess board pattern
                var backgroundChild = slotObj.transform.Find("Background");
                if (backgroundChild != null)
                {
                    if (backgroundChild.TryGetComponent<Image>(out var image))
                    {
                        int row = i / 8;
                        int col = i % 8;
                        if ((row + col) % 2 == 0)
                        {
                            image.sprite = this.tileBlack;
                        }
                        else
                        {
                            image.sprite = this.tileWhite;
                        }
                    }
                }

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
                // Create original slot
                GameObject slotObj = Instantiate(
                    this.consumableSlotPrefab,
                    this.consumableContainer
                );
                if (!slotObj.TryGetComponent<InventorySlot>(out var slot))
                {
                    slot = slotObj.AddComponent<InventorySlot>();
                }
                slot.Initialize(i, InventorySlot.SlotType.Consumable, false);
                slot.OnSlotClicked += this.OnConsumableSlotClicked;
                this.consumableSlots.Add(slot);

                // Create copy slot (if copy container exists)
                if (this.consumableContainerCopy == null)
                {
                    this.consumableContainerCopy = GameObject
                        .Find("ConsumableContainerSidebar")
                        ?.transform;
                }
                GameObject slotObjCopy = Instantiate(
                    this.consumableSlotPrefab,
                    this.consumableContainerCopy
                );
                if (!slotObjCopy.TryGetComponent<InventorySlot>(out var slotCopy))
                {
                    slotCopy = slotObjCopy.AddComponent<InventorySlot>();
                }
                slotCopy.Initialize(i, InventorySlot.SlotType.Consumable, false);
                slotCopy.OnSlotClicked += this.OnConsumableSlotClicked; // Same handler
                this.consumableSlotsCopy.Add(slotCopy);
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
            InventoryManager.OnToggleInventory += this.ToggleInventory;
        }

        private void RemoveEventListeners()
        {
            InventoryManager.OnChessPieceAdded -= this.OnChessPieceAdded;
            InventoryManager.OnChessPieceRemoved -= this.OnChessPieceRemoved;
            InventoryManager.OnConsumableAdded -= this.OnConsumableAdded;
            InventoryManager.OnConsumableRemoved -= this.OnConsumableRemoved;
            InventoryManager.OnGoldChanged -= this.OnGoldChanged;
            InventoryManager.OnInventoryChanged -= this.OnInventoryChanged;
            InventoryManager.OnToggleInventory -= this.ToggleInventory;
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
            // if (this.isConsumableSelectionMode)
            // {
            //     this.HandleConsumableTargetSelection(slotIndex, true);
            //     return;
            // }

            ChessPiece piece = InventoryManager.Instance.GetChessPieceAt(slotIndex);
            if (piece != null)
            {
                Debug.Log($"Clicked on chess piece: {piece.PieceType} at slot {slotIndex}");
            }
        }

        private void OnConsumableSlotClicked(int slotIndex)
        {
            var consumable = InventoryManager.Instance.GetConsumableAt(slotIndex);
            if (consumable != null)
            {
                Debug.Log($"Clicked on consumable at slot {slotIndex}");

                // Check if consumable is usable in current scene
                string currentScene = SceneManager.GetActiveScene().name;
                bool isGameScene = currentScene == "Game";

                if (isGameScene && !consumable.UsableInGame)
                {
                    Debug.Log($"Consumable {consumable.name} is not usable in Game scene");
                    return;
                }

                if (!isGameScene && consumable.UsableInGame)
                {
                    Debug.Log($"Consumable {consumable.name} is only usable in Game scene");
                    return;
                }

                this.StartConsumableSelection(consumable, slotIndex);
            }
        }

        // Consumable Selection System

        private void StartConsumableSelection(BuffBase consumable, int slotIndex)
        {
            this.selectedConsumable = consumable;
            this.selectedConsumableSlotIndex = slotIndex;

            // Handle SelectionType.None - use immediately without selection
            if (consumable.SelectionType == SelectionType.None)
            {
                try
                {
                    consumable.BuffFunction(null);
                    this.ConsumeSelectedConsumable();
                    Debug.Log($"Used consumable {consumable.name} with no target");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to use consumable {consumable.name}: {e.Message}");
                }
                return;
            }

            this.isConsumableSelectionMode = true;
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Game")
            {
                // In game scene, highlight based on SelectionType
                if (consumable.SelectionType == SelectionType.Tile)
                {
                    this.HighlightGameTiles();
                    if (this.isInventoryOpen)
                    {
                        this.ToggleInventory();
                    }
                }
                else if (consumable.SelectionType == SelectionType.Piece)
                {
                    this.HighlightGamePieces();
                    if (this.isInventoryOpen)
                    {
                        this.ToggleInventory();
                    }
                }
            }
            else if (currentScene == "Shop")
            {
                // In shop scene, only pieces can be selected
                if (consumable.SelectionType == SelectionType.Piece)
                {
                    this.HighlightChessPieces();
                }
                else if (consumable.SelectionType == SelectionType.Tile)
                {
                    Debug.Log("Cannot select tiles in Shop scene");
                    this.CancelConsumableSelection();
                    return;
                }
            }

            Debug.Log(
                $"Started consumable selection mode for {consumable.name} with SelectionType.{consumable.SelectionType} in {currentScene} scene"
            );
        }

        private void HighlightChessPieces()
        {
            // Highlight chess pieces in inventory slots
            var chessPieces = InventoryManager.Instance.GetAllChessPieces();
            for (int i = 0; i < chessPieces.Length && i < this.chessPieceSlots.Count; i++)
            {
                if (chessPieces[i] != null)
                {
                    var slotObj = this.chessPieceSlots[i].gameObject;
                    if (slotObj.TryGetComponent<Image>(out var image))
                    {
                        this.highlightedObjects.Add(slotObj);
                        this.originalColors.Add(image.color);
                        image.color = this.highlightColor;
                    }
                }
            }
        }

        private void HighlightGameTiles()
        {
            // Find all tiles in the game scene
            var tiles = FindObjectsByType<ChessTile>(FindObjectsSortMode.None);
            foreach (var tile in tiles)
            {
                if (tile.TryGetComponent<Image>(out var image))
                {
                    this.highlightedObjects.Add(tile.gameObject);
                    this.originalColors.Add(image.color);
                    image.color = this.highlightColor;
                }
                else if (tile.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    this.highlightedObjects.Add(tile.gameObject);
                    this.originalColors.Add(spriteRenderer.color);
                    spriteRenderer.color = this.highlightColor;
                }
            }
        }

        private void HighlightGamePieces()
        {
            // Find all chess pieces in the game scene
            var chessPieces = FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);

            foreach (var piece in chessPieces)
            {
                if (piece.TryGetComponent<Image>(out var image))
                {
                    this.highlightedObjects.Add(piece.gameObject);
                    this.originalColors.Add(image.color);
                    image.color = this.highlightColor;
                }
                else if (piece.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    this.highlightedObjects.Add(piece.gameObject);
                    this.originalColors.Add(spriteRenderer.color);
                    spriteRenderer.color = this.highlightColor;
                }
            }
        }

        private void HandleConsumableSelectionInput()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(
                    Mouse.current.position.ReadValue()
                );
                bool targetFound = false;

                string currentScene = SceneManager.GetActiveScene().name;

                if (currentScene == "Game")
                {
                    // Check selection type and find appropriate target
                    if (this.selectedConsumable.SelectionType == SelectionType.Tile)
                    {
                        var hit = Physics2D.Raycast(mousePos, Vector2.zero);
                        if (
                            hit.collider != null
                            && hit.collider.TryGetComponent<ChessTile>(out var tile)
                        )
                        {
                            targetFound = this.ApplyConsumableToTile(tile);
                        }
                    }
                    else if (this.selectedConsumable.SelectionType == SelectionType.Piece)
                    {
                        var hit = Physics2D.Raycast(mousePos, Vector2.zero);
                        if (
                            hit.collider != null
                            && hit.collider.TryGetComponent<ChessPiece>(out var piece)
                        )
                        {
                            targetFound = this.ApplyConsumableToGamePiece(piece);
                        }
                    }
                }
                else if (currentScene == "Shop")
                {
                    // In shop, only pieces can be selected
                    if (this.selectedConsumable.SelectionType == SelectionType.Piece)
                    {
                        Vector2 mousePosition = Mouse.current.position.ReadValue();

                        PointerEventData pointerData = new PointerEventData(EventSystem.current)
                        {
                            position = mousePosition,
                        };

                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointerData, results);

                        foreach (RaycastResult result in results)
                        {
                            // First check the hit object itself
                            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
                            if (slot != null && slot.GetItem() is ChessPiece piece)
                            {
                                targetFound = this.ApplyConsumableToGamePiece(piece);
                            }
                            else
                            {
                                // Check if parent was hit
                                slot = result.gameObject.GetComponentInParent<InventorySlot>();
                                if (slot != null && slot.GetItem() is ChessPiece pieceParent)
                                {
                                    targetFound = this.ApplyConsumableToGamePiece(pieceParent);
                                }
                            }
                        }
                    }
                }

                if (!targetFound)
                {
                    // Cancel consumable selection if clicked elsewhere
                    this.CancelConsumableSelection();
                }
            }
            else if (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                this.CancelConsumableSelection();
            }
        }

        private bool ApplyConsumableToTile(ChessTile tile)
        {
            if (this.selectedConsumable == null)
            {
                return false;
            }
            try
            {
                tile.AddBuff(this.selectedConsumable);
                this.ConsumeSelectedConsumable(tile);
                Debug.Log($"Applied consumable {this.selectedConsumable.name} to tile");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply consumable to tile: {e.Message}");
                return false;
            }
        }

        private bool HandleConsumableTargetSelection(int slotIndex, bool fromSlotClick)
        {
            ChessPiece piece = InventoryManager.Instance.GetChessPieceAt(slotIndex);
            if (piece != null)
            {
                this.ApplyConsumableToGamePiece(piece);
            }
            return false;
        }

        private bool ApplyConsumableToGamePiece(ChessPiece piece)
        {
            if (this.selectedConsumable == null)
            {
                return false;
            }
            try
            {
                piece.AddBuff(this.selectedConsumable);
                Debug.Log(
                    $"Applied consumable {this.selectedConsumable.name} to chess piece {piece.PieceType}"
                );
                this.ConsumeSelectedConsumable(piece);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply consumable to chess piece: {e.Message}");
                return false;
            }
        }

        private void ConsumeSelectedConsumable(IChessObject chessObject = null)
        {
            if (this.selectedConsumableSlotIndex >= 0)
            {
                InventoryManager.Instance.UseConsumable(
                    this.selectedConsumableSlotIndex,
                    chessObject
                );
                this.EndConsumableSelection();
            }
        }

        private void CancelConsumableSelection()
        {
            Debug.Log("Consumable selection cancelled");
            this.EndConsumableSelection();
        }

        private void EndConsumableSelection()
        {
            // Restore original colors
            for (int i = 0; i < this.highlightedObjects.Count; i++)
            {
                if (this.highlightedObjects[i] != null)
                {
                    if (this.highlightedObjects[i].TryGetComponent<Image>(out var image))
                    {
                        image.color = this.originalColors[i];
                    }
                    else if (
                        this.highlightedObjects[i]
                            .TryGetComponent<SpriteRenderer>(out var spriteRenderer)
                    )
                    {
                        spriteRenderer.color = this.originalColors[i];
                    }
                }
            }

            // Clear selection state
            this.highlightedObjects.Clear();
            this.originalColors.Clear();
            this.isConsumableSelectionMode = false;
            this.selectedConsumable = null;
            this.selectedConsumableSlotIndex = -1;
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
            if (this.goldText != null)
            {
                this.goldText.text = InventoryManager.Instance.Gold.ToString();
            }
        }

        private void RefreshChessPieceDisplay()
        {
            var chessPieces = InventoryManager.Instance.GetAllChessPieces();
            int chessPieceCount = InventoryManager.Instance.GetChessPieceCount();

            // Update slots
            for (int i = 0; i < this.chessPieceSlots.Count; i++)
            {
                this.chessPieceSlots[i].SetItem(chessPieces[i]);
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

            // Update original slots
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

            // Update copy slots
            for (int i = 0; i < this.consumableSlotsCopy.Count; i++)
            {
                if (i < consumables.Count)
                {
                    this.consumableSlotsCopy[i].SetItem(consumables[i]);
                }
                else
                {
                    this.consumableSlotsCopy[i].ClearItem();
                }
            }

            // Update count text (assuming you want the same count)
            if (this.consumableCountText != null)
            {
                this.consumableCountText.text =
                    $"Consumables: {consumables.Count}/{InventoryManager.Instance.MaxConsumableSlots}";
            }
        }

        // Public Methods

        public void ToggleInventory()
        {
            MusicManager.Instance.PlayClickSound();
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

                // Cancel any active consumable selection
                if (this.isConsumableSelectionMode)
                {
                    this.CancelConsumableSelection();
                }

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
