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

        public void Start()
        {
            this.HideTip();
        }

        private void ShowTip(string tip, Vector2 mousePos)
        {
            Debug.Log(tip);
            this.TipText.text = tip;
            this.TipWindow.sizeDelta = new Vector2(
                this.TipText.preferredWidth > 200 ? 200 : this.TipText.preferredWidth,
                this.TipText.preferredHeight
            );

            this.TipWindow.gameObject.SetActive(true);
            this.TipWindow.transform.position = new Vector2(
                mousePos.x,
                mousePos.y
            );
        }

        private void HideTip()
        {
            this.TipText.text = default;
            this.TipWindow.gameObject.SetActive(false);
        }
    }
}
