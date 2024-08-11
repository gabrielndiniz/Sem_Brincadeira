using System;
using FPHorror.Gameplay;
using FPHorror.Gameplay.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace FPHorror.Trigger
{
    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {
        [FormerlySerializedAs("additionalScript")]
        [Tooltip("Script adicional a ser ativado quando interagir")]
        [SerializeField]
        private BaseTrigger trigger;

        [Tooltip("Mensagem a ser exibida quando o pickup não está no inventário")]
        [SerializeField]
        private string messageIfNotInInventory;
        
        [Tooltip("Pickup necessário para interagir")]
        [SerializeField]
        private Pickup pickup;

        private bool _hasInteracted;
        private Inventory _playerInventory;

        private PlayerCharacterController playerController;
        private bool ready = false;

        private void Start()
        {
            playerController = FindObjectOfType<PlayerCharacterController>();
            _playerInventory = FindObjectOfType<Inventory>();
        }

        private void Update()
        {
            if (playerController.ReadyToInteract && ready)
            {
                HandleInteraction();
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ready = true;
            }
        }
        
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ready = false;
            }
        }

        private void HandleInteraction()
        {
            if (_playerInventory == null)
                return;

            var hasPickup = _playerInventory.HasPickup();

            if (hasPickup && pickup.IsInInventory)
            {
                // Ativa o script adicional
                if (trigger != null)
                {
                    trigger.ActivateTrigger();
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