namespace Assets.Scripts.UI
{
    using TMPro;
    using UnityEngine;

    public class TimerScript : MonoBehaviour
    {
        // Reference to the TextMeshPro text component
        public TextMeshProUGUI TimerText;

        // Timer variables
        public float TimeElapsed;
        private int minutes;
        private int seconds;

        public void Update()
        {
            // Update timer only if the game is running or timer is active
            this.TimeElapsed += Time.deltaTime;

            // Calculate minutes and seconds
            this.minutes = Mathf.FloorToInt(this.TimeElapsed / 60);
            this.seconds = Mathf.FloorToInt(this.TimeElapsed % 60);

            // Format the time as MM:SS
            this.TimerText.text = string.Format("{0:D2}:{1:D2}", this.minutes, this.seconds);
        }

        // Optionally, reset the timer
        public void ResetTimer()
        {
            this.TimeElapsed = 0f;
        }
    }
}
