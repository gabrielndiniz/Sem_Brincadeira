using FPHorror.Gameplay.Player;
using FPHorror.Game;
using UnityEngine;

namespace FPHorror.Gameplay.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float webglLookSensitivityMultiplier = 0.25f;
        [SerializeField] private float triggerAxisThreshold = 0.4f;
        [SerializeField] private bool invertYAxis = false;
        [SerializeField] private bool invertXAxis = false;

        private GameFlowManager gameFlowManager;
        private PlayerCharacterController playerCharacterController;
        private bool actionInputWasHeld;

        void Start()
        {
            playerCharacterController = GetComponent<PlayerCharacterController>();
            gameFlowManager = FindObjectOfType<GameFlowManager>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            actionInputWasHeld = GetActionInputHeld();
        }

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked && !gameFlowManager.GameIsEnding;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                // Constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);
                
                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical);
        }


        public bool GetInteractInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameInteract);
            }

            return false;
        }

        public bool GetDodgeInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameDodge);
            }

            return false;
        }

        public bool GetActionInputDown()
        {
            return GetActionInputHeld() && !actionInputWasHeld;
        }

        public bool GetActionInputReleased()
        {
            return !GetActionInputHeld() && actionInputWasHeld;
        }

        public bool GetActionInputHeld()
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadInteract) != 0f;
                if (isGamepad)
                {
                    return Input.GetAxis(GameConstants.k_ButtonNameGamepadDodge) >= triggerAxisThreshold;
                }
                else
                {
                    return Input.GetButton(GameConstants.k_ButtonNameGamepadCrouch);
                }
            }

            return false;
        }

        public bool GetAimInputHeld()
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadInteract) != 0f;
                bool i = isGamepad
                    ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadDodge) > 0f)
                    : Input.GetButton(GameConstants.k_ButtonNameGamepadCrouch);
                return i;
            }

            return false;
        }


        public bool GetCrouchInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        public bool GetCrouchInputReleased()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }



        public bool GetSprintInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameSprint);
            }

            return false;
        }

        public bool GetSprintInputReleased()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameSprint);
            }

            return false;
        }
        
        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(stickInputName) != 0f;
                float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

                if (invertYAxis)
                    i *= -1f;

                i *= lookSensitivity;

                if (isGamepad)
                {
                    i *= Time.deltaTime;
                }
                else
                {
                    i *= 0.01f;
#if UNITY_WEBGL
                    i *= webglLookSensitivityMultiplier;
#endif
                }

                return i;
            }

            return 0f;
        }
    }
}
