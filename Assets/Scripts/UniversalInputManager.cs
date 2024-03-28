namespace Darklight.UnityExt.Input
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class UniversalInputManager : MonoBehaviour
    {
        public enum InputType
        {
            NULL,
            TOUCH,
            KEYBOARD,
            GAMEPAD
        }

        public static UniversalInputManager Instance { get; private set; }
        public static string Prefix = "<< UNIVERSAL INPUT MANAGER >> ";
        public static InputType DeviceInputType = InputType.NULL;
        public static InputAction PrimaryInteractAction { get; private set; }
        public static InputAction SecondaryInteractAction { get; private set; }
        public static InputAction MoveInputAction { get; private set; }


        [Header("Input Action Map")]
        public InputActionAsset DefaultUniversalInputActions;

        // Action Map
        private InputActionMap DefaultTouchActionMap;
        private InputActionMap DefaultKeyboardActionMap;
        private InputActionMap DefaultControllerActionMap;

        // Input Actions


        private void OnEnable()
        {
            DefaultUniversalInputActions.Enable();
        }

        private void OnDisable()
        {
            DefaultUniversalInputActions.Disable();
        }

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            DefaultTouchActionMap = DefaultUniversalInputActions.FindActionMap("BasicTouch");
            DefaultKeyboardActionMap = DefaultUniversalInputActions.FindActionMap("BasicKeyboard");
            DefaultControllerActionMap = DefaultUniversalInputActions.FindActionMap("BasicController");

            bool deviceFound = DetectAndEnableInputDevice();
            if (deviceFound)
            {
                Debug.Log(Prefix + $"Found Input : {DeviceInputType}");
            }
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
