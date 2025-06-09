namespace Assets.Scripts.UI.Tooltip
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class TooltipHover
        : MonoBehaviour,
            IPointerEnterHandler,
            IPointerExitHandler,
            IPointerMoveHandler
    {
        public string TipToShow;
        private float timeToWait = 0.5f;

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.StopAllCoroutines();
            this.StartCoroutine(this.StartTimer());
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            this.StopAllCoroutines();
            TooltipManager.OnMouseLoseFocus();
            this.StartCoroutine(this.StartTimer());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.StopAllCoroutines();
            TooltipManager.OnMouseLoseFocus();
        }

        private void ShowMessage()
        {
            TooltipManager.OnMouseHover(this.TipToShow, GetMousePosition());
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSecondsRealtime(this.timeToWait);
            this.ShowMessage();
        }

        private static Vector2 GetMousePosition()
        {
            // if (Mouse.current != null)
            // {
            return Mouse.current.position.ReadValue();
            // }

            // // Fallback to legacy input system
            // return Input.mousePosition;
        }
    }
}
