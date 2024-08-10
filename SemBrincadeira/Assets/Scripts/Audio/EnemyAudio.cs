using UnityEngine;

namespace FPHorror.Audio
{


    public class EnemyAudio : CharacterAudio
    {
        [SerializeField] private AudioClip[] attackClips;
        [SerializeField] private AudioClip[] damageClips;

        public override void PlayAttackSound()
        {
            if (attackClips.Length > 0)
            {
                int index = Random.Range(0, attackClips.Length);
                audioSource.PlayOneShot(attackClips[index]);
            }
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