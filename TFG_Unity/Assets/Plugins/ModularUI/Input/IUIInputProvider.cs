using System;

namespace ModularUIRuntime
{
    public interface IUIInputProvider
    {
        event Action<int> OnHotbarSlotPressed;
        event Action OnCancelPressed;
        event Action OnMenuTogglePressed;

        void EnableInput();
        void DisableInput();
    }
}