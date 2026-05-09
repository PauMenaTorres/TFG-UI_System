using UnityEngine;
using System;
using UnityEngine.XR;

namespace ModularUIRuntime
{
    public class VRControllerInput : MonoBehaviour, IUIInputProvider
    {
        public event Action<int> OnHotbarSlotPressed;
        public event Action OnCancelPressed;
        public event Action OnMenuTogglePressed;

        private bool isEnabled = true;
        private InputDevice rightController;
        private InputDevice leftController;

        private bool previousCancelState = false;
        private bool previousMenuState = false;

        private void Start()
        {
            InitializeControllers();
        }

        public void EnableInput()
        {
            isEnabled = true;
        }

        public void DisableInput()
        {
            isEnabled = false;
        }

        private void InitializeControllers()
        {
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        private void Update()
        {
            if (!isEnabled)
            {
                return;
            }

            if (!rightController.isValid || !leftController.isValid)
            {
                InitializeControllers();
            }

            HandleButtonState(rightController, CommonUsages.secondaryButton, ref previousCancelState, OnCancelPressed);
            HandleButtonState(leftController, CommonUsages.primaryButton, ref previousMenuState, OnMenuTogglePressed);
        }

        private void HandleButtonState(InputDevice device, InputFeatureUsage<bool> buttonUsage, ref bool previousState, Action action)
        {
            if (device.TryGetFeatureValue(buttonUsage, out bool currentState))
            {
                if (currentState && !previousState)
                {
                    action?.Invoke();
                }
                previousState = currentState;
            }
        }

        public void SimulateHotbarPressFromRaycast(int slotIndex)
        {
            if (isEnabled)
            {
                OnHotbarSlotPressed?.Invoke(slotIndex);
            }
        }
    }
}