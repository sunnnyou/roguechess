using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WorldTooltip : MonoBehaviour
{
    [Header("Tooltip Settings")]
    [SerializeField]
    private string tooltipText = "Default Tooltip";

    [SerializeField]
    private float showDelay = 0.5f;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 2, 0);

    [Header("Tooltip UI Prefab (Optional)")]
    [SerializeField]
    public TextMeshProUGUI TipText;
    public RectTransform TipWindow;
    private Camera mainCamera;
    private Coroutine showTooltipCoroutine;
    private bool isMouseOver;

    public void Start()
    {
        this.mainCamera = Camera.main;
        if (this.mainCamera == null)
        {
            this.mainCamera = FindFirstObjectByType<Camera>();
        }

        // Ensure the object has a collider for mouse detection
        if (this.GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning(
                $"WorldTooltip on {this.gameObject.name} requires a Collider component for mouse detection."
            );
        }

        this.SetupTooltip();
    }

    private void SetupTooltip()
    {
        var tooltipPrefab = GameObject.Find("TooltipPrefab");

        if (tooltipPrefab == null)
        {
            GameObject canvas = GameObject.Find("UI Canvas");
            Transform childTransform = canvas.transform.Find("TooltipPrefab");
            if (childTransform != null)
            {
                childTransform.gameObject.SetActive(true);
                tooltipPrefab = childTransform.gameObject;
            }
        }
        this.TipWindow = tooltipPrefab.GetComponent<RectTransform>();
        this.TipText = this.TipWindow.GetComponentInChildren<TextMeshProUGUI>();
    }

    private IEnumerator ShowTooltipAfterDelay()
    {
        yield return new WaitForSeconds(this.showDelay);

        if (this.isMouseOver)
        {
            this.ShowTip(this.tooltipText, Mouse.current.position.ReadValue());
        }
    }

    private void ShowTip(string tip, Vector2 mousePos)
    {
        if (this.TipText == null || this.TipWindow == null)
        {
            this.SetupTooltip();
        }
        this.TipText.text = tip;
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.TipWindow);
        this.TipWindow.gameObject.SetActive(true);
        this.TipWindow.transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    private void HideTip()
    {
        if (this.TipText == null || this.TipWindow == null)
        {
            this.SetupTooltip();
        }
        this.TipText.text = default;
        this.TipWindow.gameObject.SetActive(false);
    }

    public void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mousePos2D = (Vector2)mouseWorldPos;

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            if (!this.isMouseOver)
            {
                this.isMouseOver = true;
                this.showTooltipCoroutine = this.StartCoroutine(this.ShowTooltipAfterDelay());
            }
        }
        else
        {
            if (this.isMouseOver)
            {
                this.isMouseOver = false;
                if (this.showTooltipCoroutine != null)
                {
                    this.StopCoroutine(this.showTooltipCoroutine);
                    this.showTooltipCoroutine = null;
                }

                this.HideTip();
            }
        }
    }

    // Public methods to modify tooltip at runtime
    public void SetTooltipText(string newText)
    {
        this.tooltipText = newText;
    }

    public void SetShowDelay(float delay)
    {
        this.showDelay = delay;
    }

    public void SetOffset(Vector3 newOffset)
    {
        this.offset = newOffset;
    }
}
