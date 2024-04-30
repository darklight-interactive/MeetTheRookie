using UnityEngine;
using System.Collections;
using Darklight.Game;
using System.Collections.Generic;
using Darklight.Game.Utility;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    [ExecuteAlways]
    public class CameraRig3D : MonoBehaviour
    {

        // ----- SERIALIZED VARIABLES ----- >>
        [Header("State Machine")]
        CameraStateMachine _stateMachine;
        public CameraStateMachine StateMachine => _stateMachine;

        [Header("Settings")]
        [SerializeField, ShowOnly] CameraSettings _activeSettings;
        [SerializeField] CameraSettings _defaultSettings;
        [SerializeField] CameraSettings _followSettings;
        [SerializeField] CameraSettings _closeUpSettings;

        [Header("Cameras")]
        [SerializeField] UnityEngine.Camera[] _cameras = new UnityEngine.Camera[0];

        [Header("Look Target")]
        [SerializeField] Transform _lookTarget;
        [SerializeField] Vector3 _lookTargetOffset;

        // ----- PRIVATE VARIABLES ----- >>
        private Vector3 _cameraTargetPosition;
        private Vector3 _lookTargetPosition;
        private Quaternion _cameraTargetRotation;

        public CameraSettings ActiveSettings
        {
            get => _activeSettings;
            set => _activeSettings = value;
        }

        public void SetLookTarget(Transform target, Vector3 offset)
        {
            _lookTarget = target;
            _lookTargetOffset = offset;
        }

        #region UNITY METHODS =================================== >>

        public void Awake()
        {
            _stateMachine = new CameraStateMachine(CameraState.FOLLOW_TARGET,
            new Dictionary<CameraState, IState<CameraState>>(){
                    { CameraState.DEFAULT, new CameraDefaultState(this, ref _defaultSettings) },
                    { CameraState.FOLLOW_TARGET, new CameraDefaultState(this, ref _followSettings) },
                    { CameraState.CLOSE_UP, new CameraDefaultState(this, ref _closeUpSettings)}
                    }, this.gameObject);
        }

        void Update()
        {
            if (!_activeSettings || !_lookTarget) return;
            if (_cameras.Length == 0) return;

            // Update the state machine
            _stateMachine.Step();

            // Set the rig's position to the look target
            this.transform.position = _lookTarget.position;

            // Set each camera to the settings
            foreach (UnityEngine.Camera camera in _cameras)
            {
                if (!camera) continue;
                UpdateCamera(camera, _activeSettings);
            }
        }

        void UpdateCamera(UnityEngine.Camera camera, CameraSettings settings)
        {
            float followSpeed = settings.FollowSpeed;
            float rotateSpeed = settings.RotateSpeed;
            _lookTargetPosition = _lookTarget.position + _lookTargetOffset;

            // << CALCULATE LERP POSITION >>
            _cameraTargetPosition = transform.position + settings.LocalCameraPosition;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                _cameraTargetPosition,
                followSpeed * Time.deltaTime
            );

            // << CALCULATE SLERP ROTATION >>
            _cameraTargetRotation = GetLookRotation(camera.transform.position, _lookTargetPosition);
            camera.transform.rotation = Quaternion.Slerp(
                camera.transform.rotation,
                _cameraTargetRotation,
                rotateSpeed * Time.deltaTime
            );

            // << UPDATE THE CAMERA FOV >>
            camera.orthographic = !_activeSettings.IsPerspective;
            if (_activeSettings.IsPerspective)
                camera.fieldOfView = _activeSettings.PerspectiveFOV;
            else
                camera.orthographicSize = _activeSettings.OrthographicSize;

        }

        Quaternion GetLookRotation(Vector3 originPosition, Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - originPosition).normalized;
            if (direction == Vector3.zero) return Quaternion.identity;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraRig3D))]
    public class CameraRig3DEditor : Editor
    {
        CameraRig3D _rig3D;
        int stateIndex = 0;

        void OnEnable()
        {
            _rig3D = (CameraRig3D)target;
            _rig3D.Awake();
        }

        public override void OnInspectorGUI()
        {
            CameraRig3D cameraScript = (CameraRig3D)target;
            CameraStateMachine stateMachine = cameraScript.StateMachine;

            base.OnInspectorGUI();
        }
    }
#endif

}
