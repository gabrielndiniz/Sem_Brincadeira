using FPHorror.Gameplay;
using FPHorror.Gameplay.Player;
using UnityEngine;

namespace FPHorror.Trigger
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
        [Tooltip("Script adicional a ser ativado quando interagir")]
        public MonoBehaviour additionalScript;

        [Tooltip("Mensagem a ser exibida quando o pickup não está no inventário")]
        public string messageIfNotInInventory;

        private bool _hasInteracted;
        private Inventory _playerInventory;

        private void Start()
        {
            // Certifique-se de que o script adicional está desativado no início
            if (additionalScript != null)
            {
                additionalScript.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var playerController = other.GetComponent<PlayerCharacterController>();
            if (playerController != null)
            {
                _playerInventory = playerController.GetComponent<Inventory>();
                HandleInteraction();
            }
        }

        private void HandleInteraction()
        {
            if (_playerInventory == null)
                return;

            var hasPickup = _playerInventory.HasPickup();

            if (hasPickup)
            {
                // Ativa o script adicional
                if (additionalScript != null)
                {
                    additionalScript.enabled = true;
                }
            }
            else
            {
                // Mostrar mensagem ou feedback para o jogador
                Debug.Log(messageIfNotInInventory);
            }
        }
    }
}