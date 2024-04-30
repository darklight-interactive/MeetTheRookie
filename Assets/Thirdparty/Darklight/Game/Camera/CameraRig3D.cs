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

        [Header("Settings")]
        [SerializeField] CameraSettings _activeSettings;

        [Header("Cameras")]
        [SerializeField] UnityEngine.Camera[] _cameras = new UnityEngine.Camera[0];

        // ----- PRIVATE VARIABLES ----- >>
        [SerializeField, ShowOnly] private Vector3 _cameraTargetPosition;
        [SerializeField, ShowOnly] private Quaternion _cameraTargetRotation;
        [SerializeField, ShowOnly] private Transform _lookTarget;
        [SerializeField, ShowOnly] private Vector3 _lookOffset = Vector3.up;
        [SerializeField, ShowOnly] private Vector3 _lookTargetPosition;

        public Vector3 LookTargetPosition
        {
            get
            {
                if (_lookTarget == null) return _lookTargetPosition + _lookOffset;
                return _lookTarget.position + _lookOffset;
            }
            set
            {
                _lookTarget = null;
                _lookOffset = Vector3.zero;
                _lookTargetPosition = value;
            }
        }

        public void SetNewLookTarget(Transform target, Vector3 offset)
        {
            _lookTarget = target;
            _lookOffset = offset;
        }

        public CameraSettings ActiveSettings
        {
            get => _activeSettings;
            set => _activeSettings = value;
        }

        #region UNITY METHODS =================================== >>

        protected virtual void Update()
        {
            if (!_activeSettings) return;
            if (_cameras.Length == 0) return;

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

            // << CALCULATE LERP POSITION >>
            // Calculate the target position of the camera using this transform as a origin point
            _cameraTargetPosition = transform.position + settings.LocalCameraPosition;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                _cameraTargetPosition,
                followSpeed * Time.deltaTime
            );

            // << CALCULATE SLERP ROTATION >>
            _cameraTargetRotation = GetLookRotation(camera.transform.position, LookTargetPosition);
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
}
