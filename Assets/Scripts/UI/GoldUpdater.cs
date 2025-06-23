namespace Assets.Scripts.UI
{
    using Assets.Scripts.Game.Player;
    using TMPro;
    using UnityEngine;

    public class GoldUpdater : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI goldText;

        public void Start()
        {
            this.SetupEventListeners();
        }

        private void OnDestroy()
        {
            this.RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            InventoryManager.OnGoldChanged += this.OnGoldChanged;
        }

        private void RemoveEventListeners()
        {
            InventoryManager.OnGoldChanged -= this.OnGoldChanged;
        }

        private void OnGoldChanged(int newGoldAmount)
        {
            if (this.goldText != null)
            {
                this.goldText.text = newGoldAmount.ToString();
            }
        }
    }
}
