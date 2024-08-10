using System.Collections;
using UnityEngine;

namespace FPHorror.Trigger
{
    public class BaseTrigger : MonoBehaviour
    {
        [Tooltip("Som que será tocado ao ativar o trigger.")]
        public AudioClip TriggerSound;

        [Tooltip("Objeto que será movido ou modificado ao ativar o trigger.")]
        public GameObject TargetObject;

        [Tooltip("Nova posição do objeto alvo após o trigger ser ativado.")]
        public Vector3 TargetPosition;

        [Tooltip("Duração da transição para a nova posição.")]
        public float TransitionDuration = 1f;

        private AudioSource audioSource;
        private Vector3 initialPosition;

        protected virtual void Start()
        {
            if (TargetObject != null)
            {
                initialPosition = TargetObject.transform.position;
            }

            if (TriggerSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = TriggerSound;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ActivateTrigger();
            }
        }

        protected virtual void ActivateTrigger()
        {
            if (TargetObject != null)
            {
                StartCoroutine(MoveObject(TargetObject, initialPosition, TargetPosition, TransitionDuration));
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }
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