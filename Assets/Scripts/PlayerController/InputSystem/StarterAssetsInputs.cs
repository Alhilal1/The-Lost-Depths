using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool crouch;
        public bool prone;
        public bool match;
        public bool flashlight;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        [Header("User Interface Settings")]
        public GameObject UserInterface;

        private float crouchHoldTime = 0.5f;
        private float crouchTimer = 0f;
        private bool crouchPressed = false;
        

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        
        //jika lama pencet < waktu prone, maka akan crouch
        //jika lama pencet >= waktu prone, maka akan prone
        public void OnCrouch(InputValue value)
        {
            if (value.isPressed)
            {
                crouchPressed = true;
                crouchTimer = 0f;
            }
            else
            {
                if (crouchPressed && crouchTimer < crouchHoldTime) 
                {
                    if (prone)
                    {
                        ProneInput(false);
                    }               
                    CrouchInput(!crouch);
                }
                crouchPressed = false;
            }
        }

        public void OnMatch(InputValue value)
        {
            MatchInput(value.isPressed);
        }

        public void OnFlashlight(InputValue value)
        {
            FlashlightInput(value.isPressed);
        }

#endif

        private void Start()
        {
            
        }
        private void Update()
        {
            //menghitung seberapa lama button dipencet
            if (crouchPressed)
            {
                crouchTimer += Time.deltaTime;

                if (crouchTimer >= crouchHoldTime)
                {
                    ProneInput(!prone);
                    crouchPressed = false;
                }
            }
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void CrouchInput(bool newCrouchState)
        {
            if (!prone)
            {
                crouch = newCrouchState;
            }
        }

        public void ProneInput(bool newProneState)
        {
            if (newProneState)
            {
                crouch = false;
                prone = true;
            }
            else
            {
                prone = false;
            }
        }

        public void MatchInput(bool newMatchState)
        {
            if (!flashlight)
            {
                match = !match;
            }
            else
            {
                match = true;
                flashlight = false;
            }
        }

        public void FlashlightInput(bool newFlashlightState)
        {
            if (!match)
            {
                flashlight = !flashlight;
            }
            else
            {
                match = false;
                flashlight = true;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        
    }
}
