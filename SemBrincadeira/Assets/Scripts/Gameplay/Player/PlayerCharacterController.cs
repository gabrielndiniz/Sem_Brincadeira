using System.Collections;
using Cinemachine;
using FPHorror.Game;
using FPHorror.Widget;
using UnityEngine;
using UnityEngine.Events;

namespace FPHorror.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [SerializeField] private MessageDisplay messageDisplay;

        public PlayerDeathEvent OnPlayerDeath = new PlayerDeathEvent();

        [Header("References")]
        [SerializeField] [Tooltip("Reference to the main camera used for the player")]
        private Camera playerCamera;

        [SerializeField] [Tooltip("Audio source for footsteps, jump, etc...")]
        private AudioSource audioSource;

        [Header("Movement")]
        [SerializeField] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        private float maxSpeedOnGround = 10f;
        
        [SerializeField] [Tooltip("Multiply on Sprint")]
        private float sprintSpeedModifier = 1.5f;

        [SerializeField] [Tooltip("Sharpness for the movement when grounded")]
        private float movementSharpnessOnGround = 15;

        [SerializeField] [Tooltip("Max movement speed when crouching")]
        [Range(0, 1)]
        private float maxSpeedCrouchedRatio = 0.5f;

        [Header("Rotation")]
        [SerializeField] [Tooltip("Rotation speed for moving the camera")]
        private float rotationSpeed = 200f;

        [Header("Stance")]
        [SerializeField] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        private float cameraHeightRatio = 0.9f;

        [SerializeField] [Tooltip("Height of character when standing")]
        private float capsuleHeightStanding = 1.8f;

        [SerializeField] [Tooltip("Height of character when crouching")]
        private float capsuleHeightCrouching = 0.9f;

        [SerializeField] [Tooltip("Speed of crouching transitions")]
        private float crouchingSharpness = 10f;

        [Header("Audio")]
        [SerializeField] [Tooltip("Amount of footstep sounds played when moving one meter")]
        private float footstepSfxFrequency = 1f;

        [SerializeField] [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        private float footstepSfxFrequencyWhileSprinting = 1f;

        [SerializeField] [Tooltip("Sound played for footsteps")]
        private AudioClip footstepSfx;

        
        private float nextFootstepTime = 0f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsDead { get; private set; }
        public bool IsCrouching { get; private set; }

        public Camera PlayerCamera
        {
            get => playerCamera;
            set => playerCamera = value;
        }

        public AudioSource AudioSource
        {
            get => audioSource;
            set => audioSource = value;
        }

        public float MaxSpeedOnGround
        {
            get => maxSpeedOnGround;
            set => maxSpeedOnGround = value;
        }

        public float MovementSharpnessOnGround
        {
            get => movementSharpnessOnGround;
            set => movementSharpnessOnGround = value;
        }

        public float MaxSpeedCrouchedRatio
        {
            get => maxSpeedCrouchedRatio;
            set => maxSpeedCrouchedRatio = value;
        }

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        public float CameraHeightRatio
        {
            get => cameraHeightRatio;
            set => cameraHeightRatio = value;
        }

        public float CapsuleHeightStanding
        {
            get => capsuleHeightStanding;
            set => capsuleHeightStanding = value;
        }

        public float CapsuleHeightCrouching
        {
            get => capsuleHeightCrouching;
            set => capsuleHeightCrouching = value;
        }

        public float CrouchingSharpness
        {
            get => crouchingSharpness;
            set => crouchingSharpness = value;
        }

        public float FootstepSfxFrequency
        {
            get => footstepSfxFrequency;
            set => footstepSfxFrequency = value;
        }

        public float FootstepSfxFrequencyWhileSprinting
        {
            get => footstepSfxFrequencyWhileSprinting;
            set => footstepSfxFrequencyWhileSprinting = value;
        }

        public AudioClip FootstepSfx
        {
            get => footstepSfx;
            set => footstepSfx = value;
        }

        Health m_Health;
        PlayerInputHandler m_InputHandler;
        CharacterController m_CharacterController;

        private Vector3 m_MoveDirection;
        private Vector3 m_DesiredMoveDirection;
        private float m_CurrentSpeed;
        private float m_CrouchingSpeed;
        private float m_OriginalHeight;
        private Vector3 m_OriginalCenter;

        private bool m_WasCrouching;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_InputHandler = GetComponent<PlayerInputHandler>();
            m_Health = GetComponent<Health>();
        }

        private void Start()
        {
            m_OriginalHeight = m_CharacterController.height;
            m_OriginalCenter = m_CharacterController.center;
            m_CrouchingSpeed = maxSpeedOnGround * maxSpeedCrouchedRatio;
            SetCrouchingState(false);  // Start in standing position
        }

        private void Update()
        {
            if (IsDead)
                return;

            HandleMovementInput();
            HandleCrouchInput();
            HandleDodgeInput();
            MovePlayer();
        }

        private void HandleMovementInput()
        {
            Vector3 input = m_InputHandler.GetMoveInput();
            Vector3 moveInput = new Vector3(input.x, 0, input.z).normalized;
            
            

            float speed = maxSpeedOnGround;
            if (m_InputHandler.GetSprintInputDown())
                speed *= sprintSpeedModifier;

            m_CurrentSpeed = Mathf.Lerp(m_CurrentSpeed, speed * (IsCrouching ? maxSpeedCrouchedRatio : 1), Time.deltaTime);

            m_DesiredMoveDirection = playerCamera.transform.TransformDirection(moveInput);
            m_DesiredMoveDirection.y = 0;
            
            
            // Play footstep sound
            if (moveInput.magnitude > 0f)
            {
                if (Time.time >= nextFootstepTime)
                {
                    PlayAudioClip(footstepSfx);
                    nextFootstepTime = Time.time + (1f / (IsCrouching ? footstepSfxFrequency * 0.5f : footstepSfxFrequency));
                }
            }
        }

        private void HandleCrouchInput()
        {
            if (m_InputHandler.GetCrouchInputDown() && !IsCrouching)
                SetCrouchingState(true);
            else if (!m_InputHandler.GetCrouchInputReleased() && IsCrouching)
                SetCrouchingState(false);
        }
        
        

        private void SetCrouchingState(bool crouching)
        {
            if (crouching != IsCrouching)
            {
                IsCrouching = crouching;
                m_WasCrouching = crouching;
                OnStanceChanged?.Invoke(crouching);

                StartCoroutine(AdjustHeightCoroutine(crouching));
            }
        }

        private IEnumerator AdjustHeightCoroutine(bool crouching)
        {
            float elapsedTime = 0f;
            float duration = 0.25f; // Transition duration in seconds
            float targetHeight = crouching ? capsuleHeightCrouching : capsuleHeightStanding;
            Vector3 targetCenter = crouching ? new Vector3(0, capsuleHeightCrouching / 2, 0) : new Vector3(0, capsuleHeightStanding / 2, 0);

            float originalHeight = m_CharacterController.height;
            Vector3 originalCenter = m_CharacterController.center;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                m_CharacterController.height = Mathf.Lerp(originalHeight, targetHeight, t);
                m_CharacterController.center = Vector3.Lerp(originalCenter, targetCenter, t);
                yield return null;
            }
        }

        private void HandleDodgeInput()
        {
            // Implement dodge handling here
        }

        private void MovePlayer()
        {
            Vector3 movement = m_DesiredMoveDirection * m_CurrentSpeed * Time.deltaTime;
            m_CharacterController.Move(movement);
            
        }

        private void PlayAudioClip(AudioClip clip)
        {
            if (audioSource && clip)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
