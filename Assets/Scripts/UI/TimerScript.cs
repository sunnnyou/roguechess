namespace Assets.Scripts.UI
{
    using TMPro;
    using UnityEngine;

    public class TimerScript : MonoBehaviour
    {
        // Reference to the TextMeshPro text component
        public TextMeshProUGUI timerText;

        // Timer variables
        private float timeElapsed = 0f;
        private int minutes;
        private int seconds;

        void Update()
        {
            // Update timer only if the game is running or timer is active
            timeElapsed += Time.deltaTime;
            
            // Calculate minutes and seconds
            minutes = Mathf.FloorToInt(timeElapsed / 60);
            seconds = Mathf.FloorToInt(timeElapsed % 60);

            // Format the time as MM:SS
            timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }

        // Optionally, reset the timer
        public void ResetTimer()
        {
            timeElapsed = 0f;
        }
    }
}
