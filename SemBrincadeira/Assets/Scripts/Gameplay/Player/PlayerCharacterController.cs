using System.Collections;
using System.Collections.Generic;
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

        [Header("General")]
        [SerializeField] [Tooltip("Force applied downward when in the air")]
        private float gravityDownForce = 20f;

        [SerializeField] [Tooltip("Physic layers checked to consider the player grounded")]
        private LayerMask groundCheckLayers = -1;

        [SerializeField] [Tooltip("Distance from the bottom of the character controller capsule to test for grounded")]
        private float groundCheckDistance = 0.05f;

        [Header("Movement")]
        [SerializeField] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        private float maxSpeedOnGround = 10f;

        [SerializeField] [Tooltip("Sharpness for the movement when grounded")]
        private float movementSharpnessOnGround = 15;

        [SerializeField] [Tooltip("Max movement speed when crouching")]
        [Range(0, 1)]
        private float maxSpeedCrouchedRatio = 0.5f;

        [SerializeField] [Tooltip("Max movement speed when not grounded")]
        private float maxSpeedInAir = 10f;

        [SerializeField] [Tooltip("Acceleration speed when in the air")]
        private float accelerationSpeedInAir = 25f;

        [SerializeField] [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        private float sprintSpeedModifier = 2f;

        [SerializeField] [Tooltip("Height at which the player dies instantly when falling off the map")]
        private float killHeight = -50f;

        [Header("Rotation")]
        [SerializeField] [Tooltip("Rotation speed for moving the camera")]
        private float rotationSpeed = 200f;

        [SerializeField] [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        private float aimingRotationMultiplier = 0.4f;

        [Header("Jump")]
        [SerializeField] [Tooltip("Force applied upward when jumping")]
        private float jumpForce = 9f;

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

        [SerializeField] [Tooltip("Sound played when jumping")]
        private AudioClip jumpSfx;

        [SerializeField] [Tooltip("Sound played when landing")]
        private AudioClip landSfx;

        [SerializeField] [Tooltip("Sound played when taking damage from a fall")]
        private AudioClip fallDamageSfx;

        [Header("Fall Damage")]
        [SerializeField] [Tooltip("Whether the player will receive damage when hitting the ground at high speed")]
        private bool receivesFallDamage;

        [SerializeField] [Tooltip("Minimum fall speed for receiving fall damage")]
        private float minSpeedForFallDamage = 10f;

        [SerializeField] [Tooltip("Fall speed for receiving the maximum amount of fall damage")]
        private float maxSpeedForFallDamage = 30f;

        [SerializeField] [Tooltip("Damage received when falling at the minimum speed")]
        private float fallDamageAtMinSpeed = 10f;

        [SerializeField] [Tooltip("Damage received when falling at the maximum speed")]
        private float fallDamageAtMaxSpeed = 50f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
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

        public float GravityDownForce
        {
            get => gravityDownForce;
            set => gravityDownForce = value;
        }

        public LayerMask GroundCheckLayers
        {
            get => groundCheckLayers;
            set => groundCheckLayers = value;
        }

        public float GroundCheckDistance
        {
            get => groundCheckDistance;
            set => groundCheckDistance = value;
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

        public float MaxSpeedInAir
        {
            get => maxSpeedInAir;
            set => maxSpeedInAir = value;
        }

        public float AccelerationSpeedInAir
        {
            get => accelerationSpeedInAir;
            set => accelerationSpeedInAir = value;
        }

        public float SprintSpeedModifier
        {
            get => sprintSpeedModifier;
            set => sprintSpeedModifier = value;
        }

        public float KillHeight
        {
            get => killHeight;
            set => killHeight = value;
        }

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }

        public float AimingRotationMultiplier
        {
            get => aimingRotationMultiplier;
            set => aimingRotationMultiplier = value;
        }

        public float JumpForce
        {
            get => jumpForce;
            set => jumpForce = value;
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

        public AudioClip JumpSfx
        {
            get => jumpSfx;
            set => jumpSfx = value;
        }

        public AudioClip LandSfx
        {
            get => landSfx;
            set => landSfx = value;
        }

        public AudioClip FallDamageSfx
        {
            get => fallDamageSfx;
            set => fallDamageSfx = value;
        }

        public bool ReceivesFallDamage
        {
            get => receivesFallDamage;
            set => receivesFallDamage = value;
        }

        public float MinSpeedForFallDamage
        {
            get => minSpeedForFallDamage;
            set => minSpeedForFallDamage = value;
        }

        public float MaxSpeedForFallDamage
        {
            get => maxSpeedForFallDamage;
            set => maxSpeedForFallDamage = value;
        }

        public float FallDamageAtMinSpeed
        {
            get => fallDamageAtMinSpeed;
            set => fallDamageAtMinSpeed = value;
        }

        public float FallDamageAtMaxSpeed
        {
            get => fallDamageAtMaxSpeed;
            set => fallDamageAtMaxSpeed = value;
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

            CheckGroundStatus();
            HandleMovementInput();
            HandleJumpInput();
            HandleCrouchInput();
            HandleDodgeInput();
            ApplyGravity();
            MovePlayer();
        }

        private void CheckGroundStatus()
        {
            IsGrounded = Physics.CheckSphere(transform.position - Vector3.up * m_CharacterController.height * 0.5f, groundCheckDistance, groundCheckLayers);
        }

        private void HandleMovementInput()
        {
            Vector2 input = m_InputHandler.GetMoveInput();
            Vector3 moveInput = new Vector3(input.x, 0, input.y).normalized;

            float speed = IsGrounded ? maxSpeedOnGround : maxSpeedInAir;
            if (m_InputHandler.GetSprintInputDown())
                speed *= sprintSpeedModifier;

            m_CurrentSpeed = Mathf.Lerp(m_CurrentSpeed, speed * (IsCrouching ? maxSpeedCrouchedRatio : 1), Time.deltaTime * (IsGrounded ? movementSharpnessOnGround : accelerationSpeedInAir));

            m_DesiredMoveDirection = playerCamera.transform.TransformDirection(moveInput);
            m_DesiredMoveDirection.y = 0;
        }

        private void HandleJumpInput()
        {
            if (IsGrounded && !HasJumpedThisFrame)
            {
                HasJumpedThisFrame = true;
                m_MoveDirection.y = jumpForce;
                PlayAudioClip(jumpSfx);
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

            m_CharacterController.height = targetHeight;
            m_CharacterController.center = targetCenter;
        }

        private void HandleDodgeInput()
        {
            if (m_InputHandler.GetDodgeInputDown())
            {
                StartCoroutine(DodgeCoroutine());
            }
        }

        private IEnumerator DodgeCoroutine()
        {
            // Play dodge animation or sound if needed
            Vector3 dodgeDirection = -m_InputHandler.GetMoveInput();
            float dodgeDistance = 5f;
            float dodgeDuration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < dodgeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / dodgeDuration);
                transform.position += dodgeDirection * (dodgeDistance * Time.deltaTime / dodgeDuration);
                yield return null;
            }

            // Optional: Add more logic for dodge recovery
        }

        private void ContinueDodge()
        {
            // Add logic to continue dodging if needed
        }

        private void ApplyGravity()
        {
            if (!IsGrounded)
                m_MoveDirection.y -= gravityDownForce * Time.deltaTime;
        }

        private void MovePlayer()
        {
            m_MoveDirection.x = m_DesiredMoveDirection.x * m_CurrentSpeed;
            m_MoveDirection.z = m_DesiredMoveDirection.z * m_CurrentSpeed;

            if (IsGrounded)
                m_MoveDirection.y = Mathf.Max(m_MoveDirection.y, 0); // Ensure no upward movement while grounded

            m_CharacterController.Move(m_MoveDirection * Time.deltaTime);
        }

        

        private void PlayAudioClip(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public void Die()
        {
            IsDead = true;
            OnPlayerDeath.Invoke(this);
            messageDisplay.ShowMessage("MORTE", false);
            // Trigger any death animation or effect
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position - Vector3.up * m_CharacterController.height * 0.5f, groundCheckDistance);
        }
    }

    [System.Serializable]
    public class PlayerDeathEvent : UnityEvent<PlayerCharacterController> { }
}
