using UnityEngine;
using System;

namespace ModularUIRuntime  
{
    public class MobileTouchInput : MonoBehaviour, IUIInputProvider
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

        public void TriggerHotbar(int index)
        {
            if (isEnabled)
            {
                OnHotbarSlotPressed?.Invoke(index);
            }
        }

        public void TriggerCancel()
        {
            if (isEnabled)
            {
                OnCancelPressed?.Invoke();
            }
        }

        public void TriggerMenu()
        {
            if (isEnabled)
            {
                OnMenuTogglePressed?.Invoke();
            }
        }
    }
}