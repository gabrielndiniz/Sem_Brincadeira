using UnityEngine;

namespace FPHorror.Audio
{
    public class RandomAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] sfx;
        [SerializeField] private Health playerHealth;

        [SerializeField] private float maxVolume = 1f;
        [SerializeField] private float minVolume = 0f;
        [SerializeField] private float maxPitch = 1.1f;
        [SerializeField] private float minPitch = 0.9f;
        [SerializeField] private bool playOnAwake = false;
        [SerializeField] private bool isThisMusic = false;

        private AudioSource _audioSource;
        private VolumeControl _volumeControl;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _volumeControl = GameObject.FindGameObjectWithTag("VolumeControl")?.GetComponent<VolumeControl>();

            if (_audioSource != null)
            {
                SetRandomPitchAndClip();

                if (playOnAwake)
                {
                    PlaySound();
                }
            }

            if (playerHealth != null)
            {
                playerHealth.OnDamageTaken.AddListener(PlaySound);
            }
        }

        private void SetRandomPitchAndClip()
        {
            _audioSource.pitch = Random.Range(minPitch, maxPitch);

            if (sfx.Length > 0)
            {
                _audioSource.clip = sfx[Random.Range(0, sfx.Length)];
            }
        }

        public void PlaySound()
        {
            float initialVolume = Random.Range(minVolume, maxVolume);
            _audioSource.volume = CalculateVolume(initialVolume);

            SetRandomPitchAndClip();

            if (_audioSource.enabled)
            {
                _audioSource.Play();
            }
        }

        private float CalculateVolume(float initialVolume)
        {
            float masterVolume = _volumeControl?.GetMasterVolume() ?? 1f;
            float specificVolume = isThisMusic
                ? _volumeControl?.GetMusicVolume() ?? 1f
                : _volumeControl?.GetSFXVolume() ?? 1f;

            return initialVolume * masterVolume * specificVolume / 10000f;
        }

        public void SetMusicVolume()
        {
            if (isThisMusic && _volumeControl != null)
            {
                _audioSource.volume = CalculateVolume(_audioSource.volume);
            }
        }
    }
}
