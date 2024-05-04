using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Darklight.Game.Utility;


namespace Darklight.UnityExt.Input
{
    /// <summary>
    /// A MonoBehaviour singleton class that manages the input device type and the input actions for the current device.
    /// </summary>
    public class UniversalInputManager : MonoBehaviourSingleton<UniversalInputManager>
    {
        public enum InputType { NULL, KEYBOARD, TOUCH, GAMEPAD }
        public static InputType DeviceInputType = InputType.NULL;

        [Header("Input Action Map")]
        [SerializeField] private InputActionAsset _inputActionAsset;
        public InputActionMap KeyboardActionMap => _inputActionAsset.FindActionMap("DefaultKeyboard");
        public InputActionMap GamepadActionMap => _inputActionAsset.FindActionMap("DefaultGamepad");
        public InputActionMap TouchActionMap => _inputActionAsset.FindActionMap("DefaultTouch");

        public static InputAction MoveInputAction { get; private set; }
        public static InputAction PrimaryInteractAction { get; private set; }
        public static InputAction SecondaryInteractAction { get; private set; }

        public override void Awake()
        {
            base.Awake();
            if (DetectAndEnableInputDevice())
            {
                Debug.Log($"{Prefix}Found Input: {DeviceInputType}");
            }
            _inputActionAsset.Enable();
        }

        bool DetectAndEnableInputDevice()
        {
            DisableAllActionMaps();
            return EnableDeviceBasedActionMap();
        }

        private void DisableAllActionMaps()
        {
            KeyboardActionMap.Disable();
            GamepadActionMap.Disable();
            TouchActionMap.Disable();
        }

        private bool EnableDeviceBasedActionMap()
        {
            // Enable the action map based on the device
            bool EnableActionMap(InputActionMap map, InputType type)
            {
                map.Enable();
                SetInputActions(map);
                DeviceInputType = type;
                return true;
            }

            // Detect the device and enable the action map
            switch (InputSystem.devices[0])
            {
                case Keyboard:
                    return EnableActionMap(KeyboardActionMap, InputType.KEYBOARD);
                case Gamepad:
                    return EnableActionMap(GamepadActionMap, InputType.GAMEPAD);
                case Touchscreen:
                    return EnableActionMap(TouchActionMap, InputType.TOUCH);
                default:
                    Debug.LogError($"{Prefix}Could not find Input Type");
                    return false;
            }
        }



        private void SetInputActions(InputActionMap map)
        {
            PrimaryInteractAction = map.FindAction("PrimaryInteract");
            SecondaryInteractAction = map.FindAction("SecondaryInteract");
            MoveInputAction = map.FindAction("MoveInput");
        }
    }
}