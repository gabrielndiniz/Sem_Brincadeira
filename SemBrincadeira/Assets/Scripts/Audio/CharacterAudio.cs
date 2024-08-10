using UnityEngine;

namespace FPHorror.Audio
{

    public abstract class CharacterAudio : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip[] footstepClips;
        [SerializeField] protected AudioClip[] landClips;

        protected virtual void PlayFootstepSound()
        {
            // Seleciona o som de passo com base no tipo de terreno
            AudioClip footstep = GetFootstepClipForTerrain();
            if (footstep != null)
            {
                audioSource.PlayOneShot(footstep);
            }
        }

        protected virtual void PlayLandSound()
        {
            if (landClips.Length > 0)
            {
                int index = Random.Range(0, landClips.Length);
                audioSource.PlayOneShot(landClips[index]);
            }
        }

        private AudioClip GetFootstepClipForTerrain()
        {
            // Adicione lógica para retornar o som de acordo com o terreno
            return footstepClips[0]; // Exemplo: Retornando o primeiro clipe
        }

        // Método abstrato para ser implementado por classes derivadas
        public abstract void PlayAttackSound();
        public abstract void PlayDamageSound();
    }

}