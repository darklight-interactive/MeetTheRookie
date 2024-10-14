using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using Darklight.UnityExt.Utility;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This Camera Rig is the main Monobehaviour reference for the full Camera System. 
/// It should be set as the parent object for all cameras in the scene.
/// </summary>
[ExecuteAlways]
public class MTRCameraRig : MonoBehaviour
{
    const string DATA_PATH = "Assets/Resources/MeetTheRookie/Camera/";
    const string SETTINGS_PATH = DATA_PATH + "Settings/";
    const string BOUNDS_PATH = DATA_PATH + "Bounds/";

    static Vector2 SpeedRange = new Vector2(0, 10);


    [SerializeField, ShowOnly] Vector3 _origin;
    [SerializeField, ShowOnly] Vector3 _targetPosition;
    [SerializeField, ShowOnly] Quaternion _targetRotation;
    [SerializeField, ShowOnly] float _targetFOV;

    [Header("Debug")]
    [SerializeField] bool _showGizmos;
    [SerializeField] bool _lerpInEditor;

    [Header("Data")]
    [SerializeField] Transform _followTarget;
    [SerializeField] Camera _mainCamera;
    [SerializeField, ShowOnly] List<Camera> _overlayCameras = new List<Camera>();

    [Header("Speed")]
    [SerializeField, DynamicRange("SpeedRange")] float _positionLerpSpeed = 10f;
    [SerializeField, DynamicRange("SpeedRange")] float _rotationLerpSpeed = 10f;
    [SerializeField, DynamicRange("SpeedRange")] float _fovLerpSpeed = 10f;

    [SerializeField, ReadOnly] MTRCameraSettingPreset _settings;
    [SerializeField] MTRCameraBounds _bounds;

    // << PROPERTIES >> -------------------------------------------------

    public Transform FollowTarget => _followTarget;
    public Vector3 Origin
    {
        get
        {
            _origin = BoundsCenter;
            if (_followTarget != null)
                _origin = _followTarget.position;
            return _origin;
        }
    }
    public float CameraZOffset => Mathf.Abs(_settings.zPosOffset);
    public float CameraFOV => _settings.fov;
    public float CameraAspect => _mainCamera.aspect;
    public Vector3 BoundsCenter => _bounds.Center;

    public float HalfWidth
    {
        get
        {
            // Calculate the half width of the camera frustum at the target depth
            float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * CameraZOffset * CameraAspect;
            return Mathf.Abs(halfWidth); // Return the absolute value
        }
    }
    public float HalfHeight
    {
        get
        {
            // Calculate the half-height of the frustum at the given distance offset
            float HalfHeight = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * CameraZOffset;
            return Mathf.Abs(HalfHeight); // Return the absolute value
        }
    }

    public MTRCameraSettingPreset Settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRCameraSettingPreset>(SETTINGS_PATH, "DefaultCameraSettings");
            }
            return _settings;
        }
        set => _settings = value;
    }

    public MTRCameraBounds Bounds
    {
        get
        {
            if (_bounds == null)
            {
                _bounds = MTRSceneManager.ActiveSceneData.CameraRigBounds;
            }
            return _bounds;
        }
        set => _bounds = value;
    }

    #region ( EDITOR UPDATE ) <PRIVATE_METHODS> ================================================
    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

    void EditorUpdate()
    {
        // This ensures smooth updates in the editor
        if (!Application.isPlaying)
        {
            UpdateCameraRig(_lerpInEditor);
        }
    }
    #endregion

    #region ( UNITY_RUNTIME ) <PRIVATE_METHODS> ================================================
    private void Awake()
    {
        if (_settings == null)
        {
            _settings = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRCameraSettingPreset>(SETTINGS_PATH, "DefaultCameraSettings");
        }

        _bounds = MTRSceneManager.ActiveSceneData.CameraRigBounds;

        if (_mainCamera == null)
        {
            _mainCamera = GetComponentInChildren<Camera>();
            if (_mainCamera == null)
                _mainCamera = new GameObject("MTR Main Camera").AddComponent<Camera>();
        }
        _mainCamera.transform.SetParent(transform);
    }

    public void FixedUpdate()
    {
        if (Application.isPlaying)
            UpdateCameraRig(true);
    }
    #endregion

    #region ( CALCULATIONS ) <PRIVATE_METHODS> ================================================
    /// <summary>
    /// Calculate the target position of the camera based on the preset values.
    /// </summary>
    /// <returns></returns>
    Vector3 CalculateTargetPosition()
    {
        Vector3 offset = new Vector3(
            _settings.xPosOffset,
            _settings.yPosOffset,
            _settings.zPosOffset
        );

        Vector3 adjustedPosition = Origin + offset;
        if (_bounds != null)
            adjustedPosition = EnforceBounds(adjustedPosition);

        if (Mathf.Abs(_settings.orbitRotation) > 0)
        {
            // Calculate the orbit position based on the radius and current offset (angle in degrees)
            float orbitRadians = (_settings.orbitRotation + 90) * Mathf.Deg2Rad; // Convert degrees to radians

            // Set the radius to match the z offset 
            float orbitRadius = _settings.zPosOffset;

            // Calculate orbit based off of enforced bounds 
            Vector3 orbitPosition = new Vector3(
                adjustedPosition.x + Mathf.Cos(orbitRadians) * orbitRadius,
                adjustedPosition.y, // Keep the camera at the desired height
                Origin.z + Mathf.Sin(orbitRadians) * orbitRadius
            );
            adjustedPosition = orbitPosition;

        }

        return adjustedPosition;
    }

    Quaternion CalculateTargetRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

        if (_settings.lookAtFollowTarget)
        {
            Vector3 camPosition = _targetPosition;
            camPosition.y = Origin.y;
            targetRotation = Quaternion.LookRotation(Origin - camPosition);
        }


        return targetRotation;
    }

    Vector3 EnforceBounds(Vector3 position)
    {
        float minXBound = _bounds.Left;
        float maxXBound = _bounds.Right;
        float minYBound = _bounds.Bottom;
        float maxYBound = _bounds.Top;

        // << CALCULATE POSITION >> ------------------------------
        Vector3 adjustedPosition = position;

        // ( Check the adjusted position against the X bounds )
        if (adjustedPosition.x < minXBound)
            adjustedPosition.x = minXBound + HalfWidth; // Add half width to align left edge
        else if (adjustedPosition.x > maxXBound)
            adjustedPosition.x = maxXBound - HalfWidth; // Subtract half width to align right edge

        // ( Check the adjusted position against the Y bounds )
        if (adjustedPosition.y < minYBound)
            adjustedPosition.y = minYBound + HalfHeight;
        else if (adjustedPosition.y > maxYBound)
            adjustedPosition.y = maxYBound - HalfHeight;

        // << CALCULATE FRUSTRUM OFFSET >> ------------------------------
        Vector3 frustrumOffset = Vector3.zero;
        Vector3[] frustumCorners = CalculateFrustumCorners(adjustedPosition, _mainCamera.transform.rotation);
        for (int i = 0; i < frustumCorners.Length; i++)
        {
            Vector3 corner = frustumCorners[i];

            // ( X Axis Bounds ) ------------------------------------------------------
            // If the corner is outside the bounds, adjust the offset
            // If the offset is larger than the difference between the corner and the bound, 
            //   keep the larger offset value
            if (corner.x < minXBound)
                frustrumOffset.x = Mathf.Max(frustrumOffset.x, minXBound - corner.x);
            else if (corner.x > maxXBound)
                frustrumOffset.x = Mathf.Min(frustrumOffset.x, maxXBound - corner.x);

            // ( Y Axis Bounds ) ------------------------------------------------------
            if (corner.y < minYBound)
                frustrumOffset.y = Mathf.Max(frustrumOffset.y, minYBound - corner.y);
            else if (corner.y > maxYBound)
                frustrumOffset.y = Mathf.Min(frustrumOffset.y, maxYBound - corner.y);
        }
        return adjustedPosition + frustrumOffset;
    }



    /// <summary>
    /// Calculate the frustum corners of the camera based on the given parameters.
    /// </summary>
    Vector3[] CalculateFrustumCorners(Vector3 position, Quaternion rotation)
    {
        Vector3[] frustumCorners = new Vector3[4];

        // Define the corners in local space (relative to the camera's orientation)
        Vector3 topLeft = new Vector3(-HalfWidth, HalfHeight, CameraZOffset);
        Vector3 topRight = new Vector3(HalfWidth, HalfHeight, CameraZOffset);
        Vector3 bottomLeft = new Vector3(-HalfWidth, -HalfHeight, CameraZOffset);
        Vector3 bottomRight = new Vector3(HalfWidth, -HalfHeight, CameraZOffset);

        // Transform the corners to world space
        frustumCorners[0] = position + rotation * topLeft;
        frustumCorners[1] = position + rotation * topRight;
        frustumCorners[2] = position + rotation * bottomLeft;
        frustumCorners[3] = position + rotation * bottomRight;

        return frustumCorners;
    }
    #endregion

    #region ( INTERNAL_UPDATE ) <PRIVATE_METHODS> ================================================
    void UpdateCameraRig(bool useLerp)
    {
        // Update the main camera
        UpdateMainCamera(_mainCamera, useLerp);

        // Find all overlay cameras in the main camera's hierarchy
        _overlayCameras = _mainCamera.GetComponentsInChildren<Camera>().ToList();
        UpdateOverlayCameras(_overlayCameras);
    }

    void UpdateMainCamera(Camera cam, bool useLerp)
    {
        // << CALCULATE TARGET VALUES >> -------------------------------------
        _targetPosition = CalculateTargetPosition();
        _targetRotation = CalculateTargetRotation();
        _targetFOV = _settings.fov;

        // << UPDATE CAMERA VALUES >> -------------------------------------
        if (useLerp)
        {
            // ( Lerp Camera Position ) ---------------------------------------
            cam.transform.position = Vector3.Lerp(_mainCamera.transform.position, _targetPosition, _positionLerpSpeed * Time.deltaTime);

            // ( Slerp Camera Rotation ) ---------------------------------------
            cam.transform.rotation = Quaternion.Slerp(_mainCamera.transform.rotation, _targetRotation, _rotationLerpSpeed * Time.deltaTime);

            // ( Lerp Camera Field of View ) ---------------------------------
            cam.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFOV, _fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            // ( Set Camera Position ) ---------------------------------------
            cam.transform.position = _targetPosition;

            // ( Set Camera Rotation ) ---------------------------------------
            cam.transform.rotation = _targetRotation;

            // ( Set Camera Field of View ) ---------------------------------
            cam.fieldOfView = _targetFOV;
        }
    }

    void UpdateOverlayCameras(List<Camera> cameras)
    {
        foreach (Camera camera in cameras)
        {
            if (camera == _mainCamera) continue;
            if (camera.transform.parent != _mainCamera.transform)
                camera.transform.SetParent(_mainCamera.transform);

            // Reset the local position and rotation of the camera
            camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            camera.fieldOfView = _mainCamera.fieldOfView;
        }
    }

    #endregion

    #region ( HANDLERS ) <PUBLIC_METHODS> ================================================
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    public void SetBounds(MTRCameraBounds bounds)
    {
        _bounds = bounds;
    }
    #endregion

    #region ( GIZMOS ) <PRIVATE_METHODS> ================================================

    void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;
        _bounds.DrawGizmos();

        // Draw the camera frustum
        Gizmos.color = Color.yellow;
        DrawCameraFrustum(_mainCamera);

        // Draw the camera view
        Gizmos.color = Color.cyan;
        DrawCameraView();
    }

    void DrawCameraFrustum(Camera cam)
    {
        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), CameraZOffset, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // Transform the corners to world space considering the entire transform hierarchy
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = cam.transform.TransformPoint(frustumCorners[i]);
        }

        // Draw the frustum edges
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(frustumCorners[0], frustumCorners[1]); // Bottom-left to Bottom-right
        Gizmos.DrawLine(frustumCorners[1], frustumCorners[2]); // Bottom-right to Top-right
        Gizmos.DrawLine(frustumCorners[2], frustumCorners[3]); // Top-right to Top-left
        Gizmos.DrawLine(frustumCorners[3], frustumCorners[0]); // Top-left to Bottom-left

        // Draw lines from the camera position to each corner
        Vector3 camPosition = cam.transform.position;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(camPosition, frustumCorners[i]);
        }
    }

    void DrawCameraView()
    {
        Vector3[] frustumCorners = new Vector3[4];
        _mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), CameraZOffset, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = _mainCamera.transform.TransformPoint(frustumCorners[i]);
        }

        Vector3 worldMin = Vector3.Min(frustumCorners[0], frustumCorners[2]);
        Vector3 worldMax = Vector3.Max(frustumCorners[1], frustumCorners[3]);

        Gizmos.DrawLine(frustumCorners[0], frustumCorners[1]);
        Gizmos.DrawLine(frustumCorners[1], frustumCorners[2]);
        Gizmos.DrawLine(frustumCorners[2], frustumCorners[3]);
        Gizmos.DrawLine(frustumCorners[3], frustumCorners[0]);
    }
    #endregion



#if UNITY_EDITOR
    [CustomEditor(typeof(MTRCameraRig), true)]
    public class CameraRigCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRCameraRig _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRCameraRig)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // Manually draw the bounds field
            //EditorGUILayout.PropertyField(_serializedObject.FindProperty("_bounds"));

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();

                // Update the camera rig when properties change
                if (!_script._lerpInEditor)
                    _script.UpdateCameraRig(false);

                EditorUtility.SetDirty(target);
                SceneView.RepaintAll(); // Ensure the Scene view is refreshed
            }
        }
    }
#endif
}

