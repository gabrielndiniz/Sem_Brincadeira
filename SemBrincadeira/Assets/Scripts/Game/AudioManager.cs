using UnityEngine;

namespace FPHorror.Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")] [SerializeField]
        private AudioSource musicSource;

        [SerializeField] private AudioSource sfxSource;

        [Header("Volume Settings")] [Range(0f, 1f)] [SerializeField]
        private float musicVolume = 1f;

        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance of AudioManager
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep this instance across scenes
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate instances
            }
        }

        private void Start()
        {
            UpdateAudioVolumes();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateAudioVolumes();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateAudioVolumes();
        }

        private void UpdateAudioVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
    }
}