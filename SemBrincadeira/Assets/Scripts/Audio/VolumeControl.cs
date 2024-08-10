using UnityEngine;

namespace FPHorror.Audio
{
    public class VolumeControl : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0, 1)]
        [SerializeField] private float musicVolume = 1f;
        [Range(0, 1)]
        [SerializeField] private float sfxVolume = 1f;

        public float GetMasterVolume()
        {
            return masterVolume;
        }

        public float GetMusicVolume()
        {
            return musicVolume;
        }

        public float GetSFXVolume()
        {
            return sfxVolume;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp(volume, 0f, 1f);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp(volume, 0f, 1f);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp(volume, 0f, 1f);
        }

    }
}