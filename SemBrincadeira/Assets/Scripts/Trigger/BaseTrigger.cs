using System.Collections;
using FPHorror.Game;
using UnityEngine;

namespace FPHorror.Trigger
{
    public class BaseTrigger : MonoBehaviour
    {
        [Tooltip("Som que será tocado ao ativar o trigger.")]
        public AudioClip TriggerSound;

        [Tooltip("Objeto que será ativado ao ativar o trigger.")]
        public GameObject TargetObjectToActivate;

        [Tooltip("Objeto que será desativado ao ativar o trigger.")]
        public GameObject TargetObjectToDeactivate;

        [Tooltip("Nova posição do objeto alvo após o trigger ser ativado.")]
        public Vector3 TargetPosition;

        [Tooltip("Duração da transição para a nova posição.")]
        public float TransitionDuration = 1f;

        [SerializeField]
        private AudioSource audioSource;
        private Vector3 initialPosition;

        public virtual void ActivateTrigger()
        {
            if (TargetObjectToActivate != null)
            {
                TargetObjectToActivate.SetActive(true);
            }
            
            if (TargetObjectToDeactivate != null)
            {
                TargetObjectToDeactivate.SetActive(false);
            }

            if (TriggerSound != null && audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = TriggerSound;
                audioSource.Play();
            }
            FindObjectOfType<ObjectiveManager>().CompleteObjective();
        }

        private IEnumerator MoveObject(GameObject obj, Vector3 start, Vector3 end, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                obj.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            obj.transform.position = end;
        }
    }
}