using UnityEngine;
using System;

namespace ModularUIRuntime  
{
    public class MobileTouchInput : MonoBehaviour, IUIInputProvider
    {
        [Header("Settings")]
        public float joystickDeadzone = 0.1f;
        public event Action<Vector2> OnMove;
        public event Action OnJump;
        public event Action<int> OnHotbarSlotPressed;
        public event Action OnCancelPressed;
        public event Action OnMenuTogglePressed;

        private bool isEnabled = true;
        private Vector2 movementInput;

        public void EnableInput()
        {
            isEnabled = true;
        }

        public void DisableInput()
        {
            isEnabled = false;
            movementInput = Vector2.zero;
        }

        public void UpdateMovement(Vector2 input)
        {
            if (!isEnabled) return;
            
            if (input.magnitude < joystickDeadzone)
                movementInput = Vector2.zero;
            else
                movementInput = input;

            OnMove?.Invoke(movementInput);
        }

        public void TriggerJump()
        {
            if (isEnabled) OnJump?.Invoke();
        }

        public void TriggerHotbar(int index)
        {
            if (isEnabled) OnHotbarSlotPressed?.Invoke(index);
        }

        public void TriggerCancel()
        {
            if (isEnabled) OnCancelPressed?.Invoke();
        }

        public void TriggerMenu()
        {
            if (isEnabled) OnMenuTogglePressed?.Invoke();
        }

        public Vector2 GetMovementInput() => movementInput;
    }
}