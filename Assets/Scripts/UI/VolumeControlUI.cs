namespace Assets.Scripts.UI
{
    using Assets.Scripts.Game;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class VolumeControlUI : MonoBehaviour
    {
        [Header("SFX Volume Controls")]
        public Slider SfxSlider;
        public TextMeshProUGUI SfxPercentText;

        [Header("Music Volume Controls")]
        public Slider MusicSlider;
        public TextMeshProUGUI MusicPercentText;

        private bool isUpdatingUI; // Prevent circular updates

        private void Start()
        {
            // Initialize UI with current values
            this.UpdateUIFromManager();

            // Setup slider listeners
            this.SfxSlider.onValueChanged.AddListener(this.OnSfxSliderChanged);
            this.MusicSlider.onValueChanged.AddListener(this.OnMusicSliderChanged);
        }

        private void UpdateUIFromManager()
        {
            if (MusicManager.Instance == null)
            {
                return;
            }

            this.isUpdatingUI = true;

            // Update sliders (0-1 range)
            this.SfxSlider.value = MusicManager.Instance.SfxVolume;
            this.MusicSlider.value = MusicManager.Instance.MusicVolume;

            // Update percentage text with % symbol
            this.SfxPercentText.text =
                Mathf.RoundToInt(MusicManager.Instance.SfxVolume * 100).ToString() + "%";
            this.MusicPercentText.text =
                Mathf.RoundToInt(MusicManager.Instance.MusicVolume * 100).ToString() + "%";

            this.isUpdatingUI = false;
        }

        // Slider event handlers
        private void OnSfxSliderChanged(float value)
        {
            if (this.isUpdatingUI)
            {
                return;
            }

            MusicManager.Instance.SetSfxVolume(value);

            this.isUpdatingUI = true;
            this.SfxPercentText.text = Mathf.RoundToInt(value * 100).ToString() + "%";
            this.isUpdatingUI = false;
        }

        private void OnMusicSliderChanged(float value)
        {
            if (this.isUpdatingUI)
            {
                return;
            }

            MusicManager.Instance.SetMusicVolume(value);

            this.isUpdatingUI = true;
            this.MusicPercentText.text = Mathf.RoundToInt(value * 100).ToString() + "%";
            this.isUpdatingUI = false;
        }

        // Optional: Method to refresh UI if values change elsewhere
        public void RefreshUI()
        {
            this.UpdateUIFromManager();
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (this.SfxSlider != null)
            {
                this.SfxSlider.onValueChanged.RemoveListener(this.OnSfxSliderChanged);
            }

            if (this.MusicSlider != null)
            {
                this.MusicSlider.onValueChanged.RemoveListener(this.OnMusicSliderChanged);
            }
        }
    }
}
