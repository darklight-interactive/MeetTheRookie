using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.CameraController
{
    public class CameraRig3D : MonoBehaviour
    {
        #region [[ PRIVATE VARIABLES ]]
        private Vector3 _cameraPosOffset => new Vector3(_xPosOffset, _yPosOffset, _zPosOffset);
        private Transform _pivotHandle => this.transform; // Set to self transform
        private Camera _camera;
        #endregion

        #region [[ PUBLIC ACCESSORS ]]
        public bool Initialized { get; private set; } = false;
        #endregion

        #region [[ PUBLIC INSPECTOR VARIABLES ]]
        [Header("References")]
        [SerializeField]
        private GameObject cameraPrefab;

        public Transform focusTarget;

        [Header("CameraPositionOffset"), SerializeField, Range(-10f, 10f)]
        private float _xPosOffset = 0;

        [SerializeField, Range(0f, 10f)]
        private float _yPosOffset = 50;

        [SerializeField, Range(-10f, 0f)]
        private float _zPosOffset = -50;

        [Header("PivotHandleRotation"), SerializeField, Range(-180, 180)]
        public float pivotHandleRotation = 0;

        [Header("Speeds"), SerializeField, Range(0, 10f)]
        private int followSpeed = 2;

        [SerializeField, Range(0, 10f)]
        private int rotateSpeed = 2;

        public bool isPerspective = true; // Default to perspective mode
        public float perspectiveFOV = 60f;
        public float orthographicSize = 5f;
        #endregion

        // ========================================== //
        private void Awake()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            if (!_camera || !_pivotHandle)
            {
                CreateCamera();
            }

            UpdateCameraProjection();
            SetToEditorValues();
            Initialized = true;
        }
        public void ResetToDefaults()
        {
            _xPosOffset = 0;
            _yPosOffset = 5;
            _zPosOffset = -5;
            pivotHandleRotation = 0;
            followSpeed = 2;
            rotateSpeed = 2;

            Initialized = false;
        }

        void CreateCamera()
        {
            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
            {
                // Create pivot handle at the root of the [SelectionBase]
                _pivotHandle.localPosition = Vector3.zero;

                // Prefab is set and no camera is found
                if (cameraPrefab != null)
                {
                    _camera = Instantiate(cameraPrefab, _pivotHandle).GetComponent<Camera>(); // Create camera that is child of pivot
                    _camera.transform.localPosition = Vector3.zero;
                    _camera.transform.localRotation = Quaternion.identity;
                }
                else if (cameraPrefab == null)
                {
                    Debug.Log("Camera Prefab is not Assigned - Creating Default Camera");
                    _camera = new GameObject("default-CameraRig3D").AddComponent<Camera>();
                    _camera.transform.parent = _pivotHandle.transform;
                    _camera.transform.localPosition = Vector3.zero;
                    _camera.transform.localRotation = Quaternion.identity;
                }
            }
        }

        public void SetToEditorValues()
        {
            if (!focusTarget || !_camera || !_pivotHandle.gameObject)
            {
                Debug.Log($"focusTarget :: {focusTarget} _camera :: {_camera} :: _pivotHandle{_pivotHandle}");
                return;
            }

            // Calculate pivot rotation
            _pivotHandle.rotation = GetPivotRotation();

            // Calculate and override position
            _camera.transform.localPosition = GetCameraFollowPosition(
                focusTarget.position,
                _cameraPosOffset
            );

            // Calculate and override rotation
            _camera.transform.rotation = GetCameraLookRotation(
                focusTarget.position
            );
        }
        // Method to switch to perspective mode
        public void SwitchToPerspective()
        {
            isPerspective = true;
            UpdateCameraProjection();
        }

        // Method to switch to orthographic mode
        public void SwitchToOrthographic()
        {
            isPerspective = false;
            UpdateCameraProjection();
        }

        // Update the camera's projection based on the current mode
        private void UpdateCameraProjection()
        {
            if (_camera != null)
            {
                _camera.orthographic = !isPerspective;

                if (isPerspective)
                {
                    _camera.fieldOfView = perspectiveFOV;
                }
                else
                {
                    _camera.orthographicSize = orthographicSize;
                }
            }
        }
        public void Update()
        {
            if (!focusTarget || !_camera || !_pivotHandle)
            {
                return;
            }

            Vector3 targetPosition = GetCameraFollowPosition
            (
                focusTarget.position,
                _cameraPosOffset
            );
            _camera.transform.localPosition = Vector3.Lerp
            (
                _camera.transform.localPosition,
                targetPosition,
                followSpeed * Time.deltaTime
            );

            // Calculate and Slerp rotation
            Quaternion targetRotation = GetCameraLookRotation(
                focusTarget.position);
            _camera.transform.localRotation = Quaternion.Slerp(
                _camera.transform.localRotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );

            // Update Pivot Rotation
            _pivotHandle.rotation = GetPivotRotation();
        }

        #region << GETTER FUNCTIONS >>

        Vector3 GetCameraFollowPosition(Vector3 followTargetPosition, Vector3 positionOffset)
        {
            return followTargetPosition + positionOffset;
        }

        Quaternion GetCameraLookRotation(Vector3 focusTargetPosition)
        {
            Vector3 direction = (focusTargetPosition - _camera.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }

        public Quaternion GetPivotRotation()
        {
            Vector3 yAxisRotation = Vector3.up * pivotHandleRotation;
            return Quaternion.Euler(yAxisRotation);
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
            if (cameraScript != null)
            {
                cameraScript.UpdateValues();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            cameraScript = (CameraRig3D)target;

            // Detect changes to the serialized properties
            EditorGUI.BeginChangeCheck();

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
        }
    }
#endif

}
