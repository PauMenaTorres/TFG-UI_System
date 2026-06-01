using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace ModularUIRuntime
{
    public class StandardDesktopInput : MonoBehaviour, IUIInputProvider
    {
        public event Action<int> OnHotbarSlotPressed;
        public event Action OnCancelPressed;
        public event Action OnMenuTogglePressed;

        private bool isEnabled = true;

        public void EnableInput()
        {
            isEnabled = true;
        }

        public void DisableInput()
        {
            isEnabled = false;
        }

        private void Update()
        {
            if (!isEnabled)
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                for (int i = 0; i < 9; i++)
                {
                    Key targetKey = (Key)((int)Key.Digit1 + i);
                    if (keyboard[targetKey].wasPressedThisFrame)
                    {
                        OnHotbarSlotPressed?.Invoke(i);
                    }
                }

                if (keyboard.escapeKey.wasPressedThisFrame)
                {
                    OnCancelPressed?.Invoke();
                }

                if (keyboard.tabKey.wasPressedThisFrame || keyboard.iKey.wasPressedThisFrame)
                {
                    OnMenuTogglePressed?.Invoke();
                }
            }
        }
    }
}