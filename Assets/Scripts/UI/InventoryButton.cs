namespace Assets.Scripts.UI
{
    using Assets.Scripts.Game.Player;
    using UnityEngine;
    using UnityEngine.UI;

    public class InventoryButton : MonoBehaviour
    {
        public Button InventoryButtonValue;

        public void Start()
        {
            this.InventoryButtonValue.onClick.AddListener(InventoryManager.ToggleInventory);
        }
    }
}
