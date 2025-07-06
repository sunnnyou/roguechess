namespace Assets.Scripts.Game
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource MusicSource;
        public AudioSource SfxSource;

        [Header("Audio Mixer")]
        public AudioMixerGroup MusicMixerGroup;

        [Header("Music Clips")]
        public AudioClip DefaultMusic;
        public AudioClip[] MusicClips;

        [Header("Settings")]
        [Range(0f, 1f)]
        public float MusicVolume = 0.7f;

        [Range(0f, 1f)]
        public float SfxVolume = 0.7f;
        public bool FadeTransitions = true;
        public float FadeSpeed = 1f;

        private string currentSceneName;
        private Coroutine fadeCoroutine;

        private bool isPlayingScroll;

        public void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance.gameObject);
                Instance.InitializeAudio();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Start()
        {
            // Start playing default music
            if (this.DefaultMusic != null && !this.MusicSource.isPlaying)
            {
                this.PlayMusic(this.DefaultMusic);
            }

            // Subscribe to scene change events
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        public void InitializeAudio()
        {
            // Ensure we have an AudioSource
            if (this.MusicSource == null)
            {
                this.MusicSource = this.gameObject.AddComponent<AudioSource>();
            }

            // Ensure we have an AudioSource
            if (this.SfxSource == null)
            {
                this.SfxSource = this.gameObject.AddComponent<AudioSource>();
            }

            this.MusicSource.loop = true;
            this.MusicSource.playOnAwake = false;
            this.MusicSource.volume = this.MusicVolume;
            this.MusicSource.ignoreListenerPause = true;

            this.SfxSource.loop = false;
            this.SfxSource.playOnAwake = false;
            this.SfxSource.volume = this.SfxVolume;
            this.SfxSource.ignoreListenerPause = true;

            // Assign to mixer group if available
            if (this.MusicMixerGroup != null)
            {
                this.MusicSource.outputAudioMixerGroup = this.MusicMixerGroup;
            }

            DontDestroyOnLoad(this.DefaultMusic);
            DontDestroyOnLoad(this.MusicSource.gameObject);
        }

        public void OnSceneLoaded(
            UnityEngine.SceneManagement.Scene scene,
            UnityEngine.SceneManagement.LoadSceneMode mode
        )
        {
            string sceneName = scene.name;

            // Apply scene-specific audio effects
            this.ApplySceneAudioEffects(sceneName);

            this.currentSceneName = sceneName;
        }

        public void PlayPickUpSound()
        {
            if (this.MusicClips.Length < 1 || this.MusicClips[0] == null || this.SfxSource == null)
            {
                return;
            }

            this.SfxSource.PlayOneShot(this.MusicClips[0]);
        }

        public void PlayClickSound()
        {
            if (this.MusicClips.Length < 2 || this.MusicClips[1] == null || this.SfxSource == null)
            {
                return;
            }

            this.SfxSource.PlayOneShot(this.MusicClips[1]);
        }

        public void PlayScrollSoundOpen()
        {
            if (
                this.MusicClips.Length < 3
                || this.MusicClips[2] == null
                || this.SfxSource == null
                || this.isPlayingScroll
            )
            {
                return;
            }
            this.isPlayingScroll = true;

            this.SfxSource.PlayOneShot(this.MusicClips[2]);

            this.isPlayingScroll = false;
        }

        public void PlayScrollSoundClose()
        {
            if (this.MusicClips.Length < 4 || this.MusicClips[3] == null || this.SfxSource == null)
            {
                return;
            }

            this.SfxSource.PlayOneShot(this.MusicClips[3]);
        }

        public void PlaySoundOverMusic(AudioClip clip)
        {
            if (clip == null || this.SfxSource == null)
            {
                return;
            }

            this.SfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            this.MusicSource.clip = clip;
            this.MusicSource.Play();
        }

        public void StopMusic()
        {
            this.MusicSource.Stop();
        }

        public void PauseMusic()
        {
            this.MusicSource.Pause();
        }

        public void ResumeMusic()
        {
            this.MusicSource.UnPause();
        }

        public void SetMusicVolume(float volume)
        {
            this.MusicVolume = Mathf.Clamp01(volume);
            this.MusicSource.volume = this.MusicVolume;
        }

        public System.Collections.IEnumerator FadeToNewMusic(AudioClip newClip)
        {
            // Fade out current music
            float startVolume = this.MusicSource.volume;

            while (this.MusicSource.volume > 0)
            {
                this.MusicSource.volume -= startVolume * this.FadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            // Change the clip
            this.MusicSource.clip = newClip;
            this.MusicSource.Play();

            // Fade in new music
            while (this.MusicSource.volume < this.MusicVolume)
            {
                this.MusicSource.volume +=
                    this.MusicVolume * this.FadeSpeed * Time.unscaledDeltaTime;
                yield return null;
            }

            this.MusicSource.volume = this.MusicVolume;
        }

        public void ChangeMusic(AudioClip newClip, bool fade = true)
        {
            if (newClip == null)
            {
                return;
            }

            if (fade && this.FadeTransitions)
            {
                if (this.fadeCoroutine != null)
                {
                    this.StopCoroutine(this.fadeCoroutine);
                }
                this.fadeCoroutine = this.StartCoroutine(this.FadeToNewMusic(newClip));
            }
            else
            {
                this.PlayMusic(newClip);
            }
        }

        // Method to apply audio effects for specific scenes
        public void ApplySceneAudioEffects(string sceneName)
        {
            if (this.MusicMixerGroup == null)
            {
                return;
            }

            // Example: Apply different effects based on scene
            switch (sceneName.ToLower())
            {
                case "shop":
                    // Apply muffled effect - low pass filter cuts high frequencies
                    this.MusicMixerGroup.audioMixer.SetFloat("CutoffFreq", 800f);
                    this.MusicMixerGroup.audioMixer.SetFloat("Reverb", 0.2f);
                    break;
                case "underwater":
                    // Apply underwater effect
                    this.MusicMixerGroup.audioMixer.SetFloat("CutoffFreq", 1000f);
                    this.MusicMixerGroup.audioMixer.SetFloat("Reverb", 0.8f);
                    break;
                case "cave":
                    // Apply cave reverb
                    this.MusicMixerGroup.audioMixer.SetFloat("CutoffFreq", 22000f);
                    this.MusicMixerGroup.audioMixer.SetFloat("Reverb", 0.6f);
                    break;
                default:
                    // Reset to normal
                    this.MusicMixerGroup.audioMixer.SetFloat("CutoffFreq", 22000f);
                    this.MusicMixerGroup.audioMixer.SetFloat("Reverb", 0f);
                    break;
            }
        }

        public void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }
    }
}
