using FPHorror.Gameplay.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FPHorror.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        [FormerlySerializedAs("_cameraTransform")] [SerializeField]
        private Transform pickupTransform;
        
        [Tooltip("Quantidade de vida para regenerar por segundo")]
        public float regenerationAmountPerSecond = 5f;

        [Tooltip("Delay antes de começar a regenerar após receber dano")]
        public float regenerationDelay = 3f;

        [Tooltip("Porcentagem máxima de vida que pode ser regenerada")]
        public float maxRegenerationPercentage = 0.5f;

        [Tooltip("Prefab para o efeito de brilho piscante")]
        public GameObject flickerEffectPrefab;

        private bool _isBeingHeld = false;
        private bool _isInInventory = false;

        private GameObject _uiFlickerEffect;
        private float _regenerationTimer = 0f;
        private Health _playerHealth;
        private Inventory _playerInventory;
        private CanvasGroup _uiBackground;

        private PlayerCharacterController playerController;
        

        public bool IsInInventory
        {
            get => _isInInventory;
            set => _isInInventory = value;
        }

        private void Start()
        {

            _uiBackground = FindObjectOfType<CanvasGroup>(); // Assumindo que há um único CanvasGroup no jogo
            if (_uiBackground != null)
            {
                _uiBackground.alpha = 0f;
            }
            else
            {
                Debug.LogError("No CanvasGroup found in the scene. Make sure there's a CanvasGroup component.");
            }
            if (_uiBackground != null)
            {
                _uiBackground.alpha = 0f;
            }
        }

        private void Update()
        {
            if (_isBeingHeld)
            {
                transform.position = pickupTransform.position + pickupTransform.forward * 2f;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x+17, transform.rotation.eulerAngles.y+17,transform.rotation.eulerAngles.z+ 17);

                if (playerController.ReadyToInteract) // Botão esquerdo do mouse
                {
                    if (!_isInInventory)
                    {
                        AddToInventory();
                    }
                    else
                    {
                        RemoveFromScreen();
                    }
                }
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            playerController = other.GetComponent<PlayerCharacterController>();
            if (playerController != null)
            {
                
                _playerInventory = playerController.GetComponent<Inventory>();
                if (_playerInventory != null)
                {
                    _playerHealth = playerController.GetComponent<Health>();
                    ShowPickupOnScreen();
                }
                
            }
        }

        private void ShowPickupOnScreen()
        {
            if (_playerHealth == null)
                return;

            _isBeingHeld = true;

            // Mostra o efeito de brilho piscante
            if (flickerEffectPrefab != null)
            {
                _uiFlickerEffect = Instantiate(flickerEffectPrefab, pickupTransform.position, Quaternion.identity);
                _uiFlickerEffect.transform.SetParent(pickupTransform);
            }

            // Pausa o jogo
            Time.timeScale = 0f;
            if (_uiBackground != null)
            {
                _uiBackground.alpha = 0.5f; // Fundo semiopaco
            }
        }

        private void RemoveFromScreen()
        {
            if (!playerController.ReadyToInteract)
            {
                return;
            }
            // Adiciona ao inventário
            if (_playerInventory != null)
            {
                _playerInventory.AddPickup(this);
            }

            // Remove o efeito de brilho piscante
            if (_uiFlickerEffect != null)
            {
                Destroy(_uiFlickerEffect);
            }

            // Retorna ao estado normal
            Time.timeScale = 1f;
            if (_uiBackground != null)
            {
                _uiBackground.alpha = 0f; // Fundo transparente
            }

            _isBeingHeld = false;
            _isInInventory = true;
            gameObject.SetActive(false);
        }

        private void AddToInventory()
        {
            if (!playerController.ReadyToInteract)
            {
                return;
            }
            // Adiciona o objeto ao inventário
            if (_playerInventory != null)
            {
                _playerInventory.AddPickup(this);
                _isInInventory = true;
            }

            // Remove o efeito de brilho piscante
            if (_uiFlickerEffect != null)
            {
                Destroy(_uiFlickerEffect);
            }

            // Retorna ao estado normal
            Time.timeScale = 1f;
            if (_uiBackground != null)
            {
                _uiBackground.alpha = 0f; // Fundo transparente
            }

            _isBeingHeld = false;
            gameObject.SetActive(false);
        }
    }
}
