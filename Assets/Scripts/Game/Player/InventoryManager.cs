namespace Assets.Scripts.Game.Player
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs;
    using UnityEngine;

    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory Settings")]
        public int MaxChessPieceSlots = 20;
        public int MaxConsumableSlots = 5;
        public int Gold;

        [SerializeField]
        private List<BuffBase> consumables = new List<BuffBase>();

        [SerializeField]
        private List<ChessPiece> chessPieces = new List<ChessPiece>();

        // Events for UI updates
        public static event Action<ChessPiece> OnChessPieceAdded;
        public static event Action<ChessPiece> OnChessPieceRemoved;
        public static event Action<ChessPiece> OnChessPieceQuantityChanged;
        public static event Action<BuffBase> OnConsumableAdded;
        public static event Action<BuffBase> OnConsumableRemoved;
        public static event Action<int> OnGoldChanged;
        public static event Action OnInventoryChanged;

        private void Awake()
        {
            // Singleton pattern - ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                Debug.Log("InventoryManager: Created and marked as DontDestroyOnLoad");
            }
            else
            {
                Debug.Log("InventoryManager: Duplicate instance destroyed");
                Destroy(this.gameObject);
            }
        }

        public bool AddChessPiece(ChessPiece piece)
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

            this.chessPieces.Add(piece);
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

            if (this.chessPieces.Remove(piece))
            {
                OnChessPieceRemoved?.Invoke(piece);
                OnInventoryChanged?.Invoke();
                Debug.Log($"Removed {piece.PieceType} from inventory");
                return true;
            }

            Debug.LogWarning($"ChessPiece {piece.PieceType} not found in inventory");
            return false;
        }

        public bool RemoveChessPieceAt(int index)
        {
            if (index < 0 || index >= this.chessPieces.Count)
            {
                Debug.LogWarning($"Invalid chess piece index: {index}");
                return false;
            }

            ChessPiece piece = this.chessPieces[index];
            this.chessPieces.RemoveAt(index);
            OnChessPieceRemoved?.Invoke(piece);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Removed {piece.PieceType} from inventory at index {index}");
            return true;
        }

        public List<ChessPiece> GetAllChessPieces()
        {
            return new List<ChessPiece>(this.chessPieces);
        }

        public ChessPiece GetChessPieceAt(int index)
        {
            if (index < 0 || index >= this.chessPieces.Count)
            {
                return null;
            }

            return this.chessPieces[index];
        }

        public int GetChessPieceCount()
        {
            return this.chessPieces.Count;
        }

        public bool IsChessInventoryFull()
        {
            return this.chessPieces.Count >= this.MaxChessPieceSlots;
        }

        public int GetAvailableChessPieceSlots()
        {
            return this.MaxChessPieceSlots - this.chessPieces.Count;
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

            // Apply the consumable effect here
            // This would depend on your ConsumableBuffBases implementation
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
            this.chessPieces.Clear();
            this.consumables.Clear();
            this.Gold = 0;
            OnInventoryChanged?.Invoke();
            OnGoldChanged?.Invoke(this.Gold);
            Debug.Log("Inventory cleared");
        }

        public void ClearChessPieces()
        {
            this.chessPieces.Clear();
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
            return this.chessPieces.Count == 0 && this.consumables.Count == 0;
        }

        public string GetInventoryInfo()
        {
            return $"Chess Pieces: {this.chessPieces.Count}/{this.MaxChessPieceSlots}, "
                + $"Consumables: {this.consumables.Count}/{this.MaxConsumableSlots}, "
                + $"Gold: {this.Gold}";
        }

        [ContextMenu("Debug Print Inventory")]
        public void DebugPrintInventory()
        {
            Debug.Log("=== INVENTORY DEBUG ===");
            Debug.Log($"Gold: {this.Gold}");
            Debug.Log($"Chess Pieces ({this.chessPieces.Count}/{this.MaxChessPieceSlots}):");

            for (int i = 0; i < this.chessPieces.Count; i++)
            {
                Debug.Log($"  [{i}] {this.chessPieces[i].PieceType}");
            }

            Debug.Log($"Consumables ({this.consumables.Count}/{this.MaxConsumableSlots}):");
            for (int i = 0; i < this.consumables.Count; i++)
            {
                Debug.Log($"  [{i}] Consumable");
            }
            Debug.Log("======================");
        }
    }
}
