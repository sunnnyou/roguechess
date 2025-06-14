namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.UI.Tooltip;
    using UnityEngine;

    // UI Manager for handling object selection
    public class SelectionUIManager : MonoBehaviour
    {
        private GameObject selectionPanel;
        private IChessObject selectedObject;
        private Action<IChessObject> selectionCallback;
        private readonly List<UnityEngine.UI.Button> selectionButtons = new();
        private UnityEngine.UI.Button confirmButton;

        // UI Elements
        private UnityEngine.UI.Text titleText;
        private UnityEngine.UI.Text descriptionText;
        private GameObject buttonContainer;

        // Colors
        private Color buttonSelectedColor = new(0.3f, 0.7f, 1f, 1f);
        private Color buttonHoverColor = new(1f, 1f, 1f, 0.8f);

        private void Start()
        {
            // empty for now
        }

        // Shows selection UI with customizable options.
        public void ShowSelectionUI(
            ChessBoard board,
            List<IChessObject> selectableObjects,
            Action<IChessObject> callback,
            string title = null,
            string description = null,
            List<string> tooltips = null,
            string confirmButtonText = "Select",
            float width = 400f,
            float height = 300f,
            Vector2? position = null,
            float scale = 1f
        )
        {
            if (selectableObjects == null || selectableObjects.Count == 0)
            {
                Debug.LogError("No selectable objects provided.");
                return;
            }

            if (tooltips != null && tooltips.Count != selectableObjects.Count)
            {
                Debug.LogError("Tooltips list size must match selectable objects list size.");
                return;
            }

            this.selectionCallback = callback;
            this.selectedObject = null;

            this.CreateSelectionPanel(
                board,
                selectableObjects,
                title,
                description,
                tooltips,
                confirmButtonText,
                width,
                height,
                position ?? Vector2.zero,
                scale
            );

            this.selectionPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game during selection
        }

        private void CreateSelectionPanel(
            ChessBoard board,
            List<IChessObject> selectableObjects,
            string title,
            string description,
            List<string> tooltips,
            string confirmButtonText,
            float width,
            float height,
            Vector2 position,
            float scale
        )
        {
            // Create main panel
            this.selectionPanel = new GameObject("SelectionPanel");
            this.selectionPanel.transform.SetParent(this.gameObject.transform, false);

            var panelImage = this.selectionPanel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            var rectTransform = this.selectionPanel.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = position;
            rectTransform.localScale = Vector3.one * scale;

            // Create vertical layout
            var verticalLayout =
                this.selectionPanel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            verticalLayout.spacing = 10f;
            verticalLayout.padding = new RectOffset(20, 20, 20, 20);
            verticalLayout.childControlHeight = false;
            verticalLayout.childControlWidth = true;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = true;

            // Add title if provided
            if (!string.IsNullOrEmpty(title))
            {
                this.CreateTitleText(title, board);
            }

            // Add description if provided
            if (!string.IsNullOrEmpty(description))
            {
                this.CreateDescriptionText(description, board);
            }

            // Create button container
            this.CreateButtonContainer(selectableObjects, tooltips);

            // Create confirm button
            this.CreateConfirmButton(confirmButtonText, board);
        }

        private void CreateTitleText(string title, ChessBoard board)
        {
            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(this.selectionPanel.transform, false);

            this.titleText = titleGO.AddComponent<UnityEngine.UI.Text>();
            this.titleText.text = title;
            this.titleText.font = board.MainFont;
            this.titleText.fontSize = 40;
            this.titleText.fontStyle = FontStyle.Bold;
            this.titleText.color = Color.white;
            this.titleText.alignment = TextAnchor.MiddleCenter;

            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.sizeDelta = new Vector2(0, 40);
        }

        private void CreateDescriptionText(string description, ChessBoard board)
        {
            var descGO = new GameObject("Description");
            descGO.transform.SetParent(this.selectionPanel.transform, false);

            this.descriptionText = descGO.AddComponent<UnityEngine.UI.Text>();
            this.descriptionText.text = description;
            this.descriptionText.font = board.MainFont;
            this.descriptionText.fontSize = 25;
            this.descriptionText.color = Color.gray;
            this.descriptionText.alignment = TextAnchor.MiddleCenter;

            var descRT = descGO.GetComponent<RectTransform>();
            descRT.sizeDelta = new Vector2(0, 30);
        }

        private void CreateButtonContainer(
            List<IChessObject> selectableObjects,
            List<string> tooltips
        )
        {
            this.buttonContainer = new GameObject("ButtonContainer");
            this.buttonContainer.transform.SetParent(this.selectionPanel.transform, false);

            var gridLayout = this.buttonContainer.AddComponent<UnityEngine.UI.GridLayoutGroup>();

            gridLayout.cellSize = new Vector2(100, 100);
            gridLayout.spacing = new Vector2(15, 15);
            gridLayout.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = Mathf.Min(4, selectableObjects.Count);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;

            var containerRT = this.buttonContainer.GetComponent<RectTransform>();

            containerRT.sizeDelta = new Vector2(0, 150);

            // Center anchors
            containerRT.anchorMin = new Vector2(0.5f, 0.5f);
            containerRT.anchorMax = new Vector2(0.5f, 0.5f);
            containerRT.pivot = new Vector2(0.5f, 0.5f);

            // Create selection buttons
            this.selectionButtons.Clear();
            for (int i = 0; i < selectableObjects.Count; i++)
            {
                this.CreateSelectionButton(selectableObjects[i], tooltips?[i], i);
            }
        }

        private void CreateSelectionButton(IChessObject targetObject, string tooltip, int index)
        {
            var buttonGO = new GameObject($"SelectionButton_{index}");
            buttonGO.transform.SetParent(this.buttonContainer.transform, false);

            var buttonImage = buttonGO.AddComponent<UnityEngine.UI.Image>();
            buttonImage.color = Color.white;

            var button = buttonGO.AddComponent<UnityEngine.UI.Button>();
            var sprite = targetObject.SpriteRenderer.sprite;

            // Set sprite if provided
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
                buttonImage.type = UnityEngine.UI.Image.Type.Simple;
                buttonImage.preserveAspect = true;

                // Make sprite fill more of the button
                var rectTransform = buttonGO.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(90, 90);
            }
            else
            {
                buttonImage.color = Color.gray;
            }

            // Add selection logic
            button.onClick.AddListener(() => this.SelectObject(targetObject, button));

            // Add Tooltip
            if (!string.IsNullOrEmpty(tooltip))
            {
                var tooltipUI = buttonGO.AddComponent<TooltipHover>();
                tooltipUI.TipToShow = tooltip;
            }

            // Pointer enter color effect
            var eventTrigger = buttonGO.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter,
            };
            enterEntry.callback.AddListener(
                (data) =>
                {
                    if (buttonImage.color != this.buttonSelectedColor)
                    {
                        buttonImage.color = this.buttonHoverColor;
                    }
                }
            );
            eventTrigger.triggers.Add(enterEntry);

            // Pointer exit color effect
            var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit,
            };
            exitEntry.callback.AddListener(
                (data) =>
                {
                    if (this.selectedObject != targetObject)
                    {
                        buttonImage.color = Color.white;
                    }
                }
            );
            eventTrigger.triggers.Add(exitEntry);

            // Add to button list
            this.selectionButtons.Add(button);
        }

        private void CreateConfirmButton(string confirmButtonText, ChessBoard board)
        {
            var confirmGO = new GameObject("ConfirmButton");
            confirmGO.transform.SetParent(this.selectionPanel.transform, false);

            var confirmImage = confirmGO.AddComponent<UnityEngine.UI.Image>();
            confirmImage.color = new Color(0.2f, 0.7f, 0.2f, 1f);

            this.confirmButton = confirmGO.AddComponent<UnityEngine.UI.Button>();
            this.confirmButton.interactable = false; // Disabled until selection is made

            // Add text to button
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(confirmGO.transform, false);

            var buttonText = textGO.AddComponent<UnityEngine.UI.Text>();
            buttonText.text = confirmButtonText;
            buttonText.font = board.MainFont;
            buttonText.fontSize = 30;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;

            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.anchoredPosition = Vector2.zero;

            var confirmRT = confirmGO.GetComponent<RectTransform>();
            confirmRT.sizeDelta = new Vector2(0, 40);

            this.confirmButton.onClick.AddListener(this.ConfirmSelection);
        }

        private void SelectObject(IChessObject obj, UnityEngine.UI.Button button)
        {
            this.selectedObject = obj;

            // Update button appearances
            foreach (var btn in this.selectionButtons)
            {
                btn.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            }

            // Highlight selected button
            button.GetComponent<UnityEngine.UI.Image>().color = this.buttonSelectedColor;

            // Enable confirm button
            this.confirmButton.interactable = true;
            this.confirmButton.GetComponent<UnityEngine.UI.Image>().color = new Color(
                0.2f,
                0.8f,
                0.2f,
                1f
            );
        }

        private void ConfirmSelection()
        {
            if (this.selectedObject == null || this.selectionCallback == null)
            {
                Debug.LogError("No object selected or callback is null.");
                return;
            }

            this.selectionCallback(this.selectedObject);
            this.HideSelectionUI();
        }

        private void HideSelectionUI()
        {
            if (this.selectionPanel != null)
            {
                Destroy(this.selectionPanel);
                this.selectionPanel = null;
            }

            Time.timeScale = 1f;
            this.selectedObject = null;
            this.selectionCallback = null;
            this.selectionButtons.Clear();
        }
    }
}
