namespace Assets.Scripts.Game.Player
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.UI;
    using UnityEngine;

    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory Settings")]
        public int Rows = 8;
        public int Columns = 8;
        public int MaxConsumableSlots = 5;
        public int Gold;
        public int Income = 3;

        [SerializeField]
        private List<BuffBase> consumables = new();
        public List<BuffBase> PlayerBuffs;

        private ChessPiece[] chessPieces;

        [NonSerialized]
        public int MaxChessPieceSlots;

        [NonSerialized]
        public bool InGame;

        // Events for UI updates
        public static event Action<ChessPiece> OnChessPieceAdded;
        public static event Action<ChessPiece> OnChessPieceRemoved;
        public static event Action<BuffBase> OnConsumableAdded;
        public static event Action<BuffBase> OnConsumableRemoved;
        public static event Action<int> OnGoldChanged;
        public static event Action OnInventoryChanged;
        public static event Action OnToggleInventory;

        // Awake instead of Start to make sure its always instantiated when used by other scripts
        private void Awake()
        {
            if (Instance == null)
            {
                Debug.Log("InventoryManager: Created and marked as DontDestroyOnLoad");
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            this.MaxChessPieceSlots = this.Rows * this.Columns;

            // Try to create chessboard with all chess pieces
            if (ChessBoard.Instance != null && this.chessPieces == null)
            {
                this.chessPieces = ChessBoard.Instance.GetAllPiecesArray(true);
            }
        }

        public int? GetArrayIndex(int x, int y)
        {
            if (x < 0 || x >= this.Columns || y < 0 || y >= this.Rows)
            {
                return null;
            }

            return (y * this.Columns) + x;
        }

        public (int x, int y) GetCoordinatesFromIndex(int index)
        {
            int x = index % this.Columns;
            int y = index / this.Columns;
            return (x, y);
        }

        public void SwapChessPiece(InventorySlot fromSlot, InventorySlot toSlot)
        {
            if (fromSlot == null || toSlot == null)
            {
                return;
            }

            int fromIndex = fromSlot.SlotIndex;
            int toIndex = toSlot.SlotIndex;
            if (fromIndex < 0 || toIndex < 0 || fromIndex == toIndex)
            {
                return;
            }

            var pieces = this.GetAllChessPieces();
            var pieceCount = this.GetChessPieceCount();
            if (toIndex >= this.MaxChessPieceSlots)
            {
                return;
            }

            var pieceFrom = pieces[fromIndex];
            var pieceTo = pieces[toIndex];

            // Swap items
            toSlot.SetItem(pieceFrom);
            if (pieceTo == null)
            {
                fromSlot.ClearItem();
            }
            else
            {
                fromSlot.SetItem(pieceTo);
            }

            var (xFrom, yFrom) = this.GetCoordinatesFromIndex(fromIndex);
            var (xTo, yTo) = this.GetCoordinatesFromIndex(toIndex);

            this.chessPieces[fromIndex] = pieceTo;
            this.chessPieces[toIndex] = pieceFrom;

            ChessBoard.Instance.SwitchChessPieces(
                CoordinateHelper.XYToString(xFrom, yFrom),
                CoordinateHelper.XYToString(xTo, yTo)
            );

            OnInventoryChanged?.Invoke();
        }

        public bool AddChessPiece(ChessPiece piece, int x, int y, bool replaceExisting)
        {
            if (piece == null)
            {
                Debug.LogWarning("ChessPiece is null");
                return false;
            }

            if (this.IsChessInventoryFull())
            {
                Debug.LogWarning("ChessPiece inventory is full!");
                return false;
            }

            if (!ChessBoard.Instance.AddChessPiece(piece, x, y, replaceExisting))
            {
                return false;
            }

            var index = this.GetArrayIndex(x, y);
            if (index == null)
            {
                return false;
            }

            this.chessPieces[(int)index] = piece;
            OnChessPieceAdded?.Invoke(piece);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Added {piece.PieceType} to inventory");
            return true;
        }

        public bool RemoveChessPiece(ChessPiece piece)
        {
            if (piece == null)
            {
                Debug.LogWarning("ChessPiece is null");
                return false;
            }

            if (!ChessBoard.RemoveChessPiece(piece))
            {
                return false;
            }

            int? index = this.GetArrayIndex(
                piece.CurrentTile.Position.x,
                piece.CurrentTile.Position.y
            );
            if (index == null)
            {
                return false;
            }

            this.chessPieces[(int)index] = null;

            OnChessPieceRemoved?.Invoke(piece);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Removed {piece.PieceType} from inventory");
            return true;
        }

        public bool RemoveChessPieceAt(int x, int y)
        {
            var piece = this.GetChessPieceAt(x, y);

            return this.RemoveChessPiece(piece);
        }

        public ChessPiece GetChessPieceAt(int index)
        {
            return this.chessPieces[index];
        }

        public ChessPiece GetChessPieceAt(int x, int y)
        {
            int? index = this.GetArrayIndex(x, y);
            if (index == null)
            {
                return null;
            }

            return this.chessPieces[(int)index];
        }

        public ChessPiece[] GetAllChessPieces()
        {
            return this.chessPieces;
        }

        public int GetChessPieceCount()
        {
            int count = 0;
            foreach (var element in this.chessPieces)
            {
                if (element == null)
                {
                    continue;
                }

                count++;
            }
            return count;
        }

        public bool IsChessInventoryFull()
        {
            return this.GetChessPieceCount() >= this.MaxChessPieceSlots;
        }

        public int GetAvailableChessPieceSlots()
        {
            return this.MaxChessPieceSlots - this.GetChessPieceCount();
        }

        public bool AddConsumable(BuffBase consumable)
        {
            if (consumable == null)
            {
                Debug.LogWarning("Consumable is null");
                return false;
            }

            if (this.IsConsumableInventoryFull())
            {
                Debug.LogWarning("Consumable inventory is full!");
                return false;
            }

            this.consumables.Add(consumable);
            OnConsumableAdded?.Invoke(consumable);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Added consumable to inventory");
            return true;
        }

        public bool RemoveConsumable(BuffBase consumable)
        {
            if (consumable == null)
            {
                Debug.LogWarning("Consumable is null");
                return false;
            }

            if (this.consumables.Remove(consumable))
            {
                OnConsumableRemoved?.Invoke(consumable);
                OnInventoryChanged?.Invoke();
                Debug.Log("Removed consumable from inventory");
                return true;
            }

            Debug.LogWarning("Consumable not found in inventory");
            return false;
        }

        public bool RemoveConsumableAt(int index)
        {
            if (index < 0 || index >= this.consumables.Count)
            {
                Debug.LogWarning($"Invalid consumable index: {index}");
                return false;
            }

            var consumable = this.consumables[index];
            this.consumables.RemoveAt(index);
            OnConsumableRemoved?.Invoke(consumable);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Removed consumable from inventory at index {index}");
            return true;
        }

        public bool UseConsumable(int index)
        {
            if (index < 0 || index >= this.consumables.Count)
            {
                Debug.LogWarning($"Invalid consumable index: {index}");
                return false;
            }

            var consumable = this.consumables[index];

            // TODO: use consumable function
            Debug.Log($"Used consumable at index {index}");

            // Remove the consumable after use
            return this.RemoveConsumableAt(index);
        }

        public List<BuffBase> GetAllConsumables()
        {
            return new List<BuffBase>(this.consumables);
        }

        public BuffBase GetConsumableAt(int index)
        {
            if (index < 0 || index >= this.consumables.Count)
            {
                return null;
            }

            return this.consumables[index];
        }

        public int GetConsumableCount()
        {
            return this.consumables.Count;
        }

        public bool IsConsumableInventoryFull()
        {
            return this.consumables.Count >= this.MaxConsumableSlots;
        }

        public int GetAvailableConsumableSlots()
        {
            return this.MaxConsumableSlots - this.consumables.Count;
        }

        public bool AddGold(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("Cannot add negative or zero gold");
                return false;
            }

            this.Gold += amount;
            OnGoldChanged?.Invoke(this.Gold);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Added {amount} gold. Total: {this.Gold}");
            return true;
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("Cannot spend negative or zero gold");
                return false;
            }

            if (this.Gold < amount)
            {
                Debug.LogWarning($"Not enough gold! Required: {amount}, Available: {this.Gold}");
                return false;
            }

            this.Gold -= amount;
            OnGoldChanged?.Invoke(this.Gold);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Spent {amount} gold. Remaining: {this.Gold}");
            return true;
        }

        public bool HasEnoughGold(int amount)
        {
            return this.Gold >= amount;
        }

        public void ClearInventory()
        {
            Array.Fill(this.chessPieces, null);
            this.consumables.Clear();
            this.Gold = 0;
            OnInventoryChanged?.Invoke();
            OnGoldChanged?.Invoke(this.Gold);
            Debug.Log("Inventory cleared");
        }

        public void ClearChessPieces()
        {
            Array.Fill(this.chessPieces, null);
            OnInventoryChanged?.Invoke();
            Debug.Log("Chess pieces cleared");
        }

        public void ClearConsumables()
        {
            this.consumables.Clear();
            OnInventoryChanged?.Invoke();
            Debug.Log("Consumables cleared");
        }

        public bool IsInventoryEmpty()
        {
            return this.GetChessPieceCount() == 0 && this.consumables.Count == 0;
        }

        public static void ToggleInventory()
        {
            OnToggleInventory?.Invoke();
        }
    }
}
