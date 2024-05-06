using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Darklight.Game.Utility;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.Input
{
    /// <summary>
    /// A MonoBehaviour singleton class that manages the input device type and the input actions for the current device.
    /// </summary>
    public class UniversalInputManager : MonoBehaviourSingleton<UniversalInputManager>
    {
        private bool _moveStarted;

        // -------------- [[ STATIC INPUT TYPE ]] -------------- >>
        public enum InputType { NULL, KEYBOARD, TOUCH, GAMEPAD }
        public static InputType DeviceInputType
        {
            get => Instance._deviceInputType;
            private set => Instance._deviceInputType = value;
        }

        // -------------- [[ SERIALIZED FIELDS ]] -------------- >>
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField, ShowOnly] private InputType _deviceInputType;
        [SerializeField, ShowOnly] private Vector2 _moveInput;
        [SerializeField, ShowOnly] private bool _primaryInteract;
        [SerializeField, ShowOnly] private bool _secondaryInteract;

        // -------------- [[ INPUT ACTION MAPS ]] -------------- >>
        InputActionMap _activeActionMap;
        InputActionMap _keyboardActionMap;
        InputActionMap _gamepadActionMap;
        InputActionMap _touchActionMap;

        // -------------- [[ INPUT ACTIONS ]] -------------- >>
        InputAction _move => _activeActionMap.FindAction("MoveInput");
        InputAction _primary => _activeActionMap.FindAction("PrimaryInteract");
        InputAction _secondary => _activeActionMap.FindAction("SecondaryInteract");

        // -------------- [[ INPUT EVENTS ]] -------------- >>
        public delegate void OnInput_Trigger();
        public delegate void OnInput_Vec2(Vector2 moveInput);

        /// <summary> Event for the move input from the active device. </summary>
        public static event OnInput_Vec2 OnMoveInput;
        public static event OnInput_Vec2 OnMoveInputStarted;
        public static event OnInput_Trigger OnMoveInputCanceled;

        /// <summary> Event for the primary interaction input from the active device. </summary>
        public static event OnInput_Trigger OnPrimaryInteract;
        public static event OnInput_Trigger OnPrimaryInteractCanceled;

        /// <summary> Event for the secondary interaction input from the active device. </summary>
        public static event OnInput_Trigger OnSecondaryInteract;
        public static event OnInput_Trigger OnSecondaryInteractCanceled;

        public override void Awake()
        {
            base.Awake(); // Initialize the singleton instance
            if (DetectAndEnableInputDevice())
            {
                Debug.Log($"{Prefix}Found Input: {DeviceInputType}");
            }
            _inputActionAsset.Enable();
        }

        #region ---- [[ DEVICE INPUT DETECTION ]] ---->>
        bool DetectAndEnableInputDevice()
        {
            DisableAllActionMaps();
            return EnableDeviceBasedActionMap();
        }

        void DisableAllActionMaps()
        {
            if (_keyboardActionMap != null) { _keyboardActionMap.Disable(); }
            if (_gamepadActionMap != null) { _gamepadActionMap.Disable(); }
            if (_touchActionMap != null) { _touchActionMap.Disable(); }
        }

        bool EnableDeviceBasedActionMap()
        {
            // Detect the device and enable the action map
            switch (InputSystem.devices[0])
            {
                case Keyboard:
                    _keyboardActionMap = _inputActionAsset.FindActionMap("DefaultKeyboard");
                    return EnableActionMap(_keyboardActionMap, InputType.KEYBOARD);
                case Gamepad:
                    _gamepadActionMap = _inputActionAsset.FindActionMap("DefaultGamepad");
                    return EnableActionMap(_gamepadActionMap, InputType.GAMEPAD);
                case Touchscreen:
                    _touchActionMap = _inputActionAsset.FindActionMap("DefaultTouch");
                    return EnableActionMap(_touchActionMap, InputType.TOUCH);
                default:
                    Debug.LogError($"{Prefix}Could not find Input Type");
                    return false;
            }
        }

        bool EnableActionMap(InputActionMap map, InputType type)
        {
            DisableAllActionMaps();
            if (map == null)
            {
                Debug.LogError($"{Prefix} Could not find Action Map for {type}");
                return false;
            }

            // Set the active action map
            _activeActionMap = map;
            _activeActionMap.Enable();

            // Set the device input type
            DeviceInputType = type;

            // Enable the actions
            _move.Enable();
            _primary.Enable();
            _secondary.Enable();

            // << -- Set the input events -- >>
            _move.started += ctx =>
            {
                if (!_moveStarted)
                {
                    Vector2 input = ctx.ReadValue<Vector2>();
                    OnMoveInputStarted?.Invoke(input);
                    _moveStarted = true;
                }
            };

            _move.performed += (ctx) =>
            {
                _moveInput = ctx.ReadValue<Vector2>();
                OnMoveInput?.Invoke(_moveInput);
            };

            _move.canceled += (ctx) =>
            {
                _moveStarted = false;
                _moveInput = Vector2.zero;
                OnMoveInputCanceled?.Invoke();
            };

            _primary.performed += (ctx) =>
            {
                _primaryInteract = true;
                OnPrimaryInteract?.Invoke();
            };

            _primary.canceled += (ctx) =>
            {
                _primaryInteract = false;
                OnPrimaryInteractCanceled?.Invoke();
            };

            _secondary.performed += (ctx) =>
            {
                _secondaryInteract = true;
                OnSecondaryInteract?.Invoke();
            };

            _secondary.canceled += (ctx) =>
            {
                _secondaryInteract = false;
                OnSecondaryInteractCanceled?.Invoke();
            };

            return true;
        }
        #endregion
    }
}