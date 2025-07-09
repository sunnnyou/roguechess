namespace Assets.Scripts.UI.Tooltip
{
    using System;
    using TMPro;
    using UnityEngine;

    public class TooltipManager : MonoBehaviour
    {
        public TextMeshProUGUI TipText;
        public RectTransform TipWindow;
        public static Action<string, Vector2> OnMouseHover;
        public static Action OnMouseLoseFocus;

        private void OnEnable()
        {
            OnMouseHover += this.ShowTip;
            OnMouseLoseFocus += this.HideTip;
        }

        private void OnDisable()
        {
            OnMouseHover -= this.ShowTip;
            OnMouseLoseFocus -= this.HideTip;
        }

        private void ShowTip(string tip, Vector2 mousePos)
        {
            this.TipText.text = tip;
            this.TipWindow.gameObject.SetActive(true);
            this.TipWindow.transform.position = new Vector2(mousePos.x, mousePos.y);
        }

        private void HideTip()
        {
            this.TipText.text = default;
            this.TipWindow.gameObject.SetActive(false);
        }
    }
}
