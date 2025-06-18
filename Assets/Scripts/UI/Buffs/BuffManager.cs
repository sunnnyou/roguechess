namespace Assets.Scripts.UI.Buffs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BuffManager : MonoBehaviour
    {
        public static BuffManager Instance { get; private set; }

        [Header("Global Buff Data")]
        public Buff[] availableBuffs;

        void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps it between scenes
        }

        public Buff GetRandomBuff()
        {
            if (availableBuffs.Length == 0)
            {
                Debug.LogWarning("No buffs available in BuffManager!");
                return null;
            }

            int randomIndex = Random.Range(0, availableBuffs.Length);
            return availableBuffs[randomIndex];
        }

        public Buff[] GetAllBuffs()
        {
            return availableBuffs;
        }
    }
}
