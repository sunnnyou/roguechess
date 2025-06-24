namespace Assets.Scripts.Game
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private ChessBoard chessBoard;

        // Public accessors for other scripts
        public ChessBoard Chess => this.chessBoard;

        [Header("Game State")]
        public bool IsPaused = false;
        public float GameTime;

        public void Awake()
        {
            // Singleton pattern - ensure only one GameManager exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                this.InitializeManagers();
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Start()
        {
            // Subscribe to scene loaded events
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        private void Update()
        {
            if (!this.IsPaused)
            {
                this.GameTime += Time.deltaTime;
            }
        }

        private void InitializeManagers()
        {
            // Get manager components if they're on the same scene
            if (this.chessBoard == null)
            {
                this.chessBoard = FindAnyObjectByType<ChessBoard>();
            }

            // Make sure found managers persist across scenes
            if (this.chessBoard != null && this.chessBoard.transform.parent != this.transform)
            {
                DontDestroyOnLoad(this.chessBoard.gameObject);
            }

            // Initialize each manager
            this.chessBoard?.Initialize();

            Debug.Log("GameManager: All managers initialized");
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"GameManager: Scene '{scene.name}' loaded");

            // // Notify all managers that a new scene has loaded
            this.chessBoard?.OnSceneChanged(scene.name);
        }

        public void PauseGame()
        {
            this.IsPaused = true;
            Time.timeScale = 0f;
            // this.chessBoard?.OnGamePaused();
            // this.inventoryManager?.OnGamePaused();
        }

        public void UnpauseGame()
        {
            this.IsPaused = false;
            Time.timeScale = 1f;
            // this.chessBoard?.OnGameUnpaused();
            // this.inventoryManager?.OnGameUnpaused();
        }

        public static void LoadScene(string sceneName)
        {
            // // Save current state before loading new scene
            // this.chessBoard?.SaveCurrentGameState();
            // this.inventoryManager?.SaveInventoryData();

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public static void LoadScene(int sceneIndex)
        {
            // // Save current state before loading new scene
            // this.chessBoard?.SaveCurrentGameState();
            // this.inventoryManager?.SaveInventoryData();

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        public void QuitGame()
        {
            // this.chessBoard?.OnGameQuit();
            // this.inventoryManager?.SaveInventoryData();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // this.chessBoard?.OnApplicationPause();
                // this.inventoryManager?.SaveInventoryData();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // this.chessBoard?.OnApplicationFocus();
                // this.inventoryManager?.SaveInventoryData();
            }
        }
    }
}
