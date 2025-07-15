namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class OptionsManagerUI : MonoBehaviour
    {
        [Header("Options Configuration")]
        public List<Transform> OptionsContainer;
        public List<Button> OptionsButtons;
        public TextMeshProUGUI OptionsTitleText;
        public List<string> OptionsTitleList;
        public Transform MainMenuButtonContainer;

        [Header("Control Buttons")]
        public Button CloseButton;
        public Button OpenButton;
        public Transform OptionsTransform;

        private void Start()
        {
            this.InitializeOptions();
            this.SetupButtonListeners();
        }

        private void InitializeOptions()
        {
            // Disable all options containers at start
            if (this.OptionsContainer != null)
            {
                foreach (Transform container in this.OptionsContainer)
                {
                    if (container != null)
                    {
                        container.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void SetupButtonListeners()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            {
                this.MainMenuButtonContainer.gameObject.SetActive(false);
            }
            // Setup listeners for options buttons
            if (this.OptionsButtons != null)
            {
                for (int i = 0; i < this.OptionsButtons.Count; i++)
                {
                    int index = i; // Capture index for closure
                    if (this.OptionsButtons[i] != null)
                    {
                        this.OptionsButtons[i]
                            .onClick.AddListener(() => this.OnOptionButtonClicked(index));
                    }
                }
            }

            // Setup close button listener
            if (this.CloseButton != null)
            {
                this.CloseButton.onClick.AddListener(this.OnCloseButtonClicked);
            }

            // Setup open button listener
            if (this.OpenButton != null)
            {
                this.OpenButton.onClick.AddListener(this.OnOpenButtonClicked);
            }
        }

        private void OnOptionButtonClicked(int index)
        {
            if (this.OptionsContainer == null || index < 0 || index >= this.OptionsContainer.Count)
            {
                return;
            }

            // Check if the target container is already active
            bool isAlreadyActive = this.OptionsContainer[index].gameObject.activeSelf;

            // Deactivate all other containers
            for (int i = 0; i < this.OptionsContainer.Count; i++)
            {
                if (i != index && this.OptionsContainer[i] != null)
                {
                    this.OptionsContainer[i].gameObject.SetActive(false);
                }
            }

            // Activate the selected container (if not already active)
            if (!isAlreadyActive && this.OptionsContainer[index] != null)
            {
                this.OptionsContainer[index].gameObject.SetActive(true);
            }

            // Update title text
            this.UpdateTitleText(index);

            // Call custom function
            this.OnOptionSelected(index);
        }

        private void UpdateTitleText(int index)
        {
            if (
                this.OptionsTitleText != null
                && this.OptionsTitleList != null
                && index >= 0
                && index < this.OptionsTitleList.Count
            )
            {
                this.OptionsTitleText.text = this.OptionsTitleList[index];
            }
        }

        private void OnCloseButtonClicked()
        {
            this.DeactivateAllOptionsContainer();
            if (this.OptionsTransform != null)
            {
                this.OptionsTransform.gameObject.SetActive(false);
            }
        }

        private void OnOpenButtonClicked()
        {
            this.DeactivateAllOptionsContainer();
            if (this.OptionsTransform != null)
            {
                this.OptionsTransform.gameObject.SetActive(
                    !this.OptionsTransform.gameObject.activeSelf
                );
            }
        }

        public void DeactivateAllOptionsContainer()
        {
            MusicManager.Instance.PlayClickSound();
            this.OptionsTitleText.text = string.Empty;
            foreach (var option in this.OptionsContainer)
            {
                if (option != null)
                {
                    option.gameObject.SetActive(false);
                }
            }
        }

        protected virtual void OnOptionSelected(int optionIndex)
        {
            MusicManager.Instance.PlayClickSound();
            Debug.Log($"Option {optionIndex} selected");
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (this.OptionsButtons != null)
            {
                foreach (Button button in this.OptionsButtons)
                {
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                    }
                }
            }

            if (this.CloseButton != null)
            {
                this.CloseButton.onClick.RemoveAllListeners();
            }

            if (this.OpenButton != null)
            {
                this.OpenButton.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// /// Initialize the options manager with required components
        /// </summary>
        public void Initialize(
            List<Transform> containers,
            List<Button> buttons,
            TextMeshProUGUI titleText,
            List<string> titleList,
            Button close,
            Button open,
            Transform optionsRoot
        )
        {
            this.OptionsContainer = containers;
            this.OptionsButtons = buttons;
            this.OptionsTitleText = titleText;
            this.OptionsTitleList = titleList;
            this.CloseButton = close;
            this.OpenButton = open;
            this.OptionsTransform = optionsRoot;

            this.InitializeOptions();
            this.SetupButtonListeners();
        }

        /// <summary>
        /// /// Manually select an option by index
        /// </summary>
        public void SelectOption(int index)
        {
            this.OnOptionButtonClicked(index);
        }

        public static void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        public static void BackToMainMenu()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }
            Destroy(GameManager.Instance);
            Destroy(InventoryManager.Instance.gameObject);
            Destroy(ChessBoard.Instance.gameObject);
            GameManager.LoadScene("MainMenu");
        }
    }
}
