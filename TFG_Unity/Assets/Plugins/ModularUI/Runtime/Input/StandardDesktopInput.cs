using UnityEngine;
using System;

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

            for (int i = 0; i < 9; i++)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    OnHotbarSlotPressed?.Invoke(i);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelPressed?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab) || UnityEngine.Input.GetKeyDown(KeyCode.I))
            {
                OnMenuTogglePressed?.Invoke();
            }
        }
    }
}