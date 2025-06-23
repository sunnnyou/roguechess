namespace Assets.Scripts.Game
{
    using UnityEngine;

    public abstract class BaseManager : MonoBehaviour
    {
        public bool IsInitialized { get; protected set; }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public virtual void OnSceneChanged(string sceneName) { }

        public virtual void OnGamePaused() { }

        public virtual void OnGameUnpaused() { }

        public virtual void OnGameQuit() { }

        public virtual void OnApplicationPause() { }

        public virtual void OnApplicationFocus() { }

        protected virtual void Awake()
        {
            // Managers can exist independently - they don't need to be children
            // Just ensure they persist across scenes if they're not already
            if (
                GameManager.Instance != null
                && this.transform.parent != GameManager.Instance.transform
            )
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}
