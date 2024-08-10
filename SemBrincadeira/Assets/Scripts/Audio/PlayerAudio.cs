using UnityEngine;

namespace FPHorror.Audio
{

    public class PlayerAudio : CharacterAudio
    {
        [SerializeField] private AudioClip[] damageClips;

        private void OnFootstep()
        {
            PlayFootstepSound();
        }

        private void OnLand()
        {
            PlayLandSound();
        }

        public override void PlayAttackSound()
        {
            // Jogador não ataca, então este método pode ficar vazio ou lançar uma exceção
        }

        public override void PlayDamageSound()
        {
            if (damageClips.Length > 0)
            {
                int index = Random.Range(0, damageClips.Length);
                audioSource.PlayOneShot(damageClips[index]);
            }
        }
    }
}