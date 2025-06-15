namespace Assets.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class NotificationManager : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [SerializeField]
        private GameObject uiPrefab;

        [SerializeField]
        private GameObject scrollContainer; // Reference to existing container in scene

        [Header("Animation Settings")]
        [SerializeField]
        private float slideDuration = 0.5f;

        [SerializeField]
        private Ease slideEase = Ease.OutBack;

        [Header("Stacking Settings")]
        [SerializeField]
        private float stackSpacing = 10f;

        [SerializeField]
        private bool autoScrollToNewest = true;

        private ScrollRect scrollRect;
        private RectTransform contentRect;
        private VerticalLayoutGroup layoutGroup;
        private readonly List<GameObject> uiObjectStack = new();

        public void Start()
        {
            this.InitializeComponents();
            this.SetupScrollContainer();
        }

        private void InitializeComponents()
        {
            // Validate prefab
            if (this.uiPrefab == null)
            {
                Debug.LogError("UI Prefab is not assigned!");
                return;
            }

            // Validate scroll container
            if (this.scrollContainer == null)
            {
                Debug.LogError(
                    "Scroll Container is not assigned! Please assign your existing scroll container from the scene."
                );
                return;
            }
        }

        private void SetupScrollContainer()
        {
            // Get references to components from the existing container
            this.scrollRect = this.scrollContainer.GetComponent<ScrollRect>();
            if (this.scrollRect == null)
            {
                Debug.LogError("Scroll Container must have a ScrollRect component!");
                return;
            }

            // Find the content object (should be assigned to ScrollRect.content)
            this.contentRect = this.scrollRect.content;
            if (this.contentRect == null)
            {
                Debug.LogError("ScrollRect must have a Content assigned!");
                return;
            }

            // Get or add layout group to content
            this.layoutGroup = this.contentRect.GetComponent<VerticalLayoutGroup>();
            if (this.layoutGroup == null)
            {
                this.layoutGroup = this.contentRect.gameObject.AddComponent<VerticalLayoutGroup>();
                Debug.Log("Added VerticalLayoutGroup to content");
            }

            // Configure layout group for proper stacking
            this.layoutGroup.childAlignment = TextAnchor.LowerCenter;
            this.layoutGroup.spacing = this.stackSpacing;
            this.layoutGroup.childControlHeight = false;
            this.layoutGroup.childControlWidth = false;
            this.layoutGroup.childForceExpandHeight = false;
            this.layoutGroup.childForceExpandWidth = false;
            this.layoutGroup.padding = new RectOffset(0, 0, 0, 10); // Small bottom padding

            // Ensure content size fitter exists for auto-sizing
            if (!this.contentRect.TryGetComponent<ContentSizeFitter>(out var contentFitter))
            {
                contentFitter = this.contentRect.gameObject.AddComponent<ContentSizeFitter>();
                Debug.Log("Added ContentSizeFitter to content");
            }

            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Ensure the content starts at the bottom
            this.contentRect.anchorMin = new Vector2(0.5f, 0f);
            this.contentRect.anchorMax = new Vector2(0.5f, 1f);
            this.contentRect.pivot = new Vector2(0.5f, 0f);
            this.contentRect.anchoredPosition = new Vector2(0, 0);

            Debug.Log("Scroll container setup completed successfully!");
        }

        /// <summary>
        /// Creates and animates a new UI object from the prefab, stacking it with existing objects
        /// </summary>
        /// <param name="text">Text to display on the UI object</param>
        /// <returns>The created GameObject</returns>
        public GameObject CreateSlideUpUI(string text)
        {
            if (this.uiPrefab == null || this.contentRect == null)
            {
                Debug.LogError("Missing required components!");
                return null;
            }

            // Instantiate the prefab as child of content
            GameObject newUIObject = Instantiate(this.uiPrefab, this.contentRect);

            if (!newUIObject.TryGetComponent<RectTransform>(out var uiRect))
            {
                Debug.LogError("UI Prefab must have a RectTransform component!");
                Destroy(newUIObject);
                return null;
            }

            // Set the text
            SetUIText(newUIObject, text);

            // Ensure proper sizing and positioning for layout group
            SetupUIObjectForLayout(uiRect);

            // Add to stack
            this.uiObjectStack.Add(newUIObject);

            // Setup entrance animation
            this.SetupEntranceAnimation(uiRect);

            // Update layout and scroll
            this.StartCoroutine(this.UpdateLayoutAndScrollDelayed());

            Debug.Log($"Created UI object: {text} (Total count: {this.uiObjectStack.Count})");

            return newUIObject;
        }

        /// <summary>
        /// Removes the newest (top) UI object and updates the stack
        /// </summary>
        /// <returns>True if an object was popped, false if stack was empty</returns>
        public bool PopNewestUI()
        {
            if (this.uiObjectStack.Count == 0)
            {
                return false;
            }

            GameObject newestObject = this.uiObjectStack[this.uiObjectStack.Count - 1];
            this.uiObjectStack.RemoveAt(this.uiObjectStack.Count - 1);

            Debug.Log($"Popping newest UI object (Remaining count: {this.uiObjectStack.Count})");

            // Animate the object out and destroy it
            this.AnimateObjectOut(newestObject);

            // Update layout
            this.StartCoroutine(this.UpdateLayoutAndScrollDelayed());

            return true;
        }

        /// <summary>
        /// Removes a specific UI object from the stack
        /// </summary>
        /// <param name="uiObject">The UI object to remove</param>
        /// <returns>True if object was found and removed</returns>
        public bool RemoveUIObject(GameObject uiObject)
        {
            if (uiObject == null || !this.uiObjectStack.Contains(uiObject))
            {
                return false;
            }

            this.uiObjectStack.Remove(uiObject);
            this.AnimateObjectOut(uiObject);
            this.StartCoroutine(this.UpdateLayoutAndScrollDelayed());

            Debug.Log($"Removed specific UI object (Remaining count: {this.uiObjectStack.Count})");

            return true;
        }

        /// <summary>
        /// Clears all UI objects from the stack
        /// </summary>
        public void ClearAllUI()
        {
            Debug.Log($"Clearing all UI objects (Count: {this.uiObjectStack.Count})");

            foreach (GameObject obj in this.uiObjectStack)
            {
                if (obj != null)
                {
                    this.AnimateObjectOut(obj);
                }
            }

            this.uiObjectStack.Clear();
            this.StartCoroutine(this.UpdateLayoutAndScrollDelayed());
        }

        /// <summary>
        /// Gets the current count of UI objects in the stack
        /// </summary>
        /// <returns>Number of UI objects currently displayed</returns>
        public int GetUIObjectCount()
        {
            return this.uiObjectStack.Count;
        }

        /// <summary>
        /// Force refresh the layout (useful for debugging)
        /// </summary>
        public void RefreshLayout()
        {
            if (this.contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentRect);
                Canvas.ForceUpdateCanvases();
            }
        }

        private static void SetupUIObjectForLayout(RectTransform uiRect)
        {
            // Ensure the UI object works well with the layout group
            uiRect.anchorMin = new Vector2(0.5f, 0.5f);
            uiRect.anchorMax = new Vector2(0.5f, 0.5f);
            uiRect.pivot = new Vector2(0.5f, 0.5f);

            // Make sure it has proper size (don't override if prefab has specific size)
            if (!uiRect.TryGetComponent<LayoutElement>(out var layoutElement))
            {
                layoutElement = uiRect.gameObject.AddComponent<LayoutElement>();
            }

            // Let the layout group handle positioning, but preserve size preferences
            layoutElement.ignoreLayout = false;

            // Ensure the object is active and visible
            uiRect.gameObject.SetActive(true);
        }

        private void SetupEntranceAnimation(RectTransform uiRect)
        {
            // Start with scale animation for smooth entrance
            Vector3 originalScale = uiRect.localScale;
            uiRect.localScale = new Vector3(0.8f, 0.8f, 1f);

            // Also start with slight transparency
            if (!uiRect.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = uiRect.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;

            // Animate scale and fade in
            uiRect.DOScale(originalScale, this.slideDuration).SetEase(this.slideEase);
            canvasGroup.DOFade(1f, this.slideDuration);
        }

        private void AnimateObjectOut(GameObject uiObject)
        {
            if (uiObject == null)
            {
                return;
            }

            // Fade out
            if (!uiObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = uiObject.AddComponent<CanvasGroup>();
            }

            canvasGroup
                .DOFade(0f, 0.25f * this.slideDuration)
                .OnComplete(() =>
                {
                    if (uiObject != null)
                    {
                        Destroy(uiObject);
                    }
                });
        }

        private IEnumerator UpdateLayoutAndScrollDelayed()
        {
            // Wait a frame for instantiation to complete
            yield return null;

            // Force layout rebuild
            if (this.contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentRect);
                Canvas.ForceUpdateCanvases();
            }

            // Wait another frame for layout to apply
            yield return null;

            // Auto-scroll to show newest item (bottom)
            if (this.autoScrollToNewest && this.scrollRect != null && this.uiObjectStack.Count > 0)
            {
                this.scrollRect.DOVerticalNormalizedPos(0f, this.slideDuration * 0.5f);
            }
        }

        private static void SetUIText(GameObject uiObject, string text)
        {
            // Try to find TextMeshProUGUI component in children
            TextMeshProUGUI tmpText = uiObject.GetComponentInChildren<TextMeshProUGUI>();

            if (tmpText != null)
            {
                tmpText.text = text;
                Debug.Log($"Set TMP text: {text}");
            }
            else
            {
                // Fallback to regular Text component
                Text regularText = uiObject.GetComponentInChildren<Text>();
                if (regularText != null)
                {
                    regularText.text = text;
                    Debug.Log($"Set regular text: {text}");
                }
                else
                {
                    Debug.LogWarning(
                        "No Text or TextMeshProUGUI component found in UI prefab children!"
                    );
                }
            }
        }
    }
}
