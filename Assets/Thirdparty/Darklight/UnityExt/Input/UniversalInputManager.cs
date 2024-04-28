namespace Darklight.UnityExt.Input
{
    using System.Collections;
    using System.Collections.Generic;
    using Darklight.Game.Utility;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;


    /// <summary>
    /// The Universal Input Manager is a singleton class that manages the input device type and the input actions for the current device.
    /// </summary>
    public class UniversalInputManager : MonoBehaviourSingleton<UniversalInputManager>
    {
        // Action Map
        private InputActionMap DefaultTouchActionMap;
        private InputActionMap DefaultKeyboardActionMap;
        private InputActionMap DefaultControllerActionMap;

        public enum InputType
        {
            NULL,
            TOUCH,
            KEYBOARD,
            GAMEPAD
        }

        public static InputType DeviceInputType = InputType.NULL;
        public static InputAction PrimaryInteractAction { get; private set; }
        public static InputAction SecondaryInteractAction { get; private set; }
        public static InputAction MoveInputAction { get; private set; }


        [Header("Input Action Map")]
        public InputActionAsset DefaultUniversalInputActions;

        public delegate void InputReady();
        public event InputReady OnInputReady;

        private void OnEnable()
        {
            DefaultUniversalInputActions.Enable();
            OnInputReady?.Invoke();
        }

        private void OnDisable()
        {
            DefaultUniversalInputActions.Disable();
        }

        public override void Awake()
        {
            base.Awake();

            DefaultTouchActionMap = DefaultUniversalInputActions.FindActionMap("DefaultTouch");
            DefaultKeyboardActionMap = DefaultUniversalInputActions.FindActionMap("DefaultKeyboard");
            DefaultControllerActionMap = DefaultUniversalInputActions.FindActionMap("DefaultController");

            bool deviceFound = DetectAndEnableInputDevice();
            if (deviceFound)
            {
                Debug.Log(Prefix + $"Found Input : {DeviceInputType}");
            }

            OnInputReady?.Invoke();
        }

        private bool DetectAndEnableInputDevice()
        {
            // Disable all action maps initially
            DefaultTouchActionMap.Disable();
            DefaultKeyboardActionMap.Disable();
            DefaultControllerActionMap.Disable();

            // Enable the appropriate action map based on the current input device
            if (Touchscreen.current != null)
            {
                DefaultTouchActionMap.Enable();
                PrimaryInteractAction = DefaultTouchActionMap.FindAction("PrimaryInteract");
                SecondaryInteractAction = DefaultTouchActionMap.FindAction("SecondaryInteract");
                MoveInputAction = DefaultTouchActionMap.FindAction("MoveInput");
                DeviceInputType = InputType.TOUCH;
            }
            else if (Gamepad.current != null)
            {
                DefaultControllerActionMap.Enable();
                PrimaryInteractAction = DefaultControllerActionMap.FindAction("PrimaryInteract");
                SecondaryInteractAction = DefaultControllerActionMap.FindAction("SecondaryInteract");
                MoveInputAction = DefaultControllerActionMap.FindAction("MoveInput");
                DeviceInputType = InputType.GAMEPAD;
            }
            else if (Keyboard.current != null)
            {
                DefaultKeyboardActionMap.Enable();
                PrimaryInteractAction = DefaultKeyboardActionMap.FindAction("PrimaryInteract");
                SecondaryInteractAction = DefaultKeyboardActionMap.FindAction("SecondaryInteract");
                MoveInputAction = DefaultKeyboardActionMap.FindAction("MoveInput");
                DeviceInputType = InputType.KEYBOARD;
            }
            else
            {
                Debug.LogError(Prefix + "Could not find Input Type");
                return false;
            }

            return true;
        }
    }
}
