using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.CameraController
{
    [ExecuteAlways]
    public class CameraRig3D : MonoBehaviour
    {
        #region [[ PRIVATE VARIABLES ]]
        private Camera _mainCamera;
        private Camera[] _childCameras;

        private Vector3 _cameraLocalPosOffset => new Vector3(_xPosOffset, _yPosOffset, _zPosOffset);
        #endregion

        #region [[ PUBLIC ACCESSORS ]]
        public bool Initialized { get; private set; } = false;
        #endregion

        #region [[ Camera Settings ]]
        [Header("Camera Target Offset")]
        [SerializeField] private Transform _lookTarget;
        [SerializeField] private Vector3 _lookTargetOffset = new Vector3(0, 0, 0);

        [Header("CameraPositionOffsets")]
        [SerializeField, Range(-10f, 10f)]
        private float _xPosOffset = 0;

        [SerializeField, Range(0f, 10f)]
        private float _yPosOffset = 0;

        [SerializeField, Range(-10f, 10f)]
        private float _zPosOffset = 0;



        [SerializeField, Range(0, 10f)]
        private int _followSpeed = 2;

        [SerializeField, Range(0, 10f)]
        private int _rotateSpeed = 2;

        [Header("Camera Projection")]
        [SerializeField] private bool isPerspective = true; // Default to perspective mode
        [SerializeField, Range(0, 100f)] private float perspectiveFOV = 20f;
        [SerializeField] private float orthographicSize = 5f;
        #endregion

        // ========================================== //
        private void Awake()
        {
            // Get all cameras in the hierarchy
            _childCameras = GetComponentsInChildren<Camera>();

            if (_childCameras.Length > 0)
            {
                _mainCamera = _childCameras[0];
            }
            else
            {
                _mainCamera = CreateCameraChild();
            }
        }

        public void ResetToDefaults()
        {
            _xPosOffset = 0;
            _yPosOffset = 5;
            _zPosOffset = -5;
            _followSpeed = 2;
            _rotateSpeed = 2;

            Initialized = false;
        }

        Camera CreateCameraChild()
        {
            Camera newCamera = new GameObject("default-CameraRig3D").AddComponent<Camera>();
            newCamera.transform.parent = this.transform;
            newCamera.transform.localPosition = Vector3.zero;
            newCamera.transform.localRotation = Quaternion.identity;

            return newCamera;
        }

        void UpdateAllCamerasToEditorValues()
        {
            foreach (Camera cam in _childCameras)
            {
                cam.transform.localPosition = _mainCamera.transform.localPosition;
                cam.transform.localRotation = _mainCamera.transform.localRotation;
            }
        }

        public void SetToEditorValues()
        {
            /*
            if (!_mainCamera || !_pivotHandle.gameObject)
            {
                return;
            }

            // Calculate and override position
            _mainCamera.transform.localPosition = GetCameraFollowPosition(
                focusTarget.position,
                _cameraLocalPosOffset
            );

            // Calculate and override rotation
            _mainCamera.transform.rotation = GetCameraLookRotation(
                focusTarget.position
            );
            */
        }

        // Update the camera's projection based on the current mode
        private void UpdateCameraProjection(Camera cam)
        {
            if (_mainCamera != null)
            {
                _mainCamera.orthographic = !isPerspective;

                if (isPerspective)
                {
                    _mainCamera.fieldOfView = perspectiveFOV;
                }
                else
                {
                    _mainCamera.orthographicSize = orthographicSize;
                }
            }
        }
        public void Update()
        {
            if (_lookTarget == null) return;

            Vector3 lookTargetPosition = _lookTarget.position + _lookTargetOffset;

            // << UPDATE CAMERA TO FOLLOW TARGET >>
            Vector3 currentFollowPosition = GetCameraFollowPosition(lookTargetPosition, _cameraLocalPosOffset);
            _mainCamera.transform.localPosition = Vector3.Lerp
            (
                _mainCamera.transform.localPosition,
                currentFollowPosition,
                _followSpeed * Time.deltaTime
            );

            // Calculate and Slerp rotation
            Quaternion targetRotation = GetCameraLookRotation(lookTargetPosition);
            _mainCamera.transform.localRotation = Quaternion.Slerp(
                _mainCamera.transform.localRotation,
                targetRotation,
                _rotateSpeed * Time.deltaTime
            );
        }

        #region << GETTER FUNCTIONS >>

        Vector3 GetCameraFollowPosition(Vector3 followTargetPosition, Vector3 positionOffset)
        {
            return followTargetPosition + positionOffset;
        }

        Quaternion GetCameraLookRotation(Vector3 focusTargetPosition)
        {
            Vector3 direction = (focusTargetPosition - _mainCamera.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraRig3D))]
    public class CameraRig3DEditor : Editor
    {
        CameraRig3D cameraScript;
        public void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            cameraScript = (CameraRig3D)target;

            // Detect changes to the serialized properties
            EditorGUI.BeginChangeCheck();

            /*
            if (GUILayout.Button("Set Values"))
            {
                cameraScript.UpdateValues();
            }
            else if (cameraScript.Initialized && GUILayout.Button("Reset to Defaults"))
            {
                cameraScript.ResetToDefaults();
                cameraScript.UpdateValues();
            }

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                // If something changed, apply the changes and update the camera position
                serializedObject.ApplyModifiedProperties();
                cameraScript.UpdateValues();
            }
            */
        }
    }
#endif

}
