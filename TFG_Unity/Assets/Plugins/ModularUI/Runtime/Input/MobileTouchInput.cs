using UnityEngine;

namespace ModularUIRuntime  
{
    public class MobileTouchInput : MonoBehaviour, IUIInputProvider
    {
        public event System.Action<int> OnHotbarSlotPressed;
        public event System.Action OnCancelPressed;
        public event System.Action OnMenuTogglePressed;

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