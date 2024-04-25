using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    [ExecuteAlways]
    public class CameraRig3D : MonoBehaviour
    {
        private Vector3 _cameraPosition;
        private Vector3 _lookTargetPosition;
        private Quaternion _cameraRotation;
        [SerializeField] private CameraSettings _settings;

        [Header("Cameras")]
        [SerializeField] private UnityEngine.Camera[] _cameras = new UnityEngine.Camera[0];

        [Header("Look Target")]
        [SerializeField] private Transform _lookTarget;
        [SerializeField] private Vector3 _lookTargetOffset;

        public void Update()
        {
            if (!_settings || !_lookTarget) return;
            if (_cameras.Length == 0) return;

            // Set the rig's position to the look target
            this.transform.position = _lookTarget.position;

            // Set each camera to the settings
            foreach (UnityEngine.Camera camera in _cameras)
            {
                if (!camera) continue;
                UpdateCamera(camera, _settings);
            }
        }

        void UpdateCamera(UnityEngine.Camera camera, CameraSettings settings)
        {
            float followSpeed = settings.FollowSpeed;
            float rotateSpeed = settings.RotateSpeed;
            _lookTargetPosition = _lookTarget.position + _lookTargetOffset;

            // << CALCULATE LERP POSITION >>
            _cameraPosition = transform.position + settings.LocalCameraPosition;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                _cameraPosition,
                followSpeed * Time.deltaTime
            );

            // << CALCULATE SLERP ROTATION >>
            _cameraRotation = GetLookRotation(camera.transform.position, _lookTargetPosition);
            camera.transform.rotation = Quaternion.Slerp(
                camera.transform.rotation,
                _cameraRotation,
                rotateSpeed * Time.deltaTime
            );

            // << UPDATE THE CAMERA FOV >>
            camera.orthographic = !_settings.IsPerspective;
            if (_settings.IsPerspective)
                camera.fieldOfView = _settings.PerspectiveFOV;
            else
                camera.orthographicSize = _settings.OrthographicSize;

        }

        Quaternion GetLookRotation(Vector3 originPosition, Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - originPosition).normalized;
            if (direction == Vector3.zero) return Quaternion.identity;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraRig3D))]
    public class CameraRig3DEditor : Editor
    {
        CameraRig3D cameraScript;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            cameraScript = (CameraRig3D)target;

            // Detect changes to the serialized properties
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            // If something changed, apply the changes and update the camera position
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
