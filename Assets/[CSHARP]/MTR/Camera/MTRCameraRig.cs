using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using Darklight.UnityExt.Utility;

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

    Vector3 _targetPosition;
    Quaternion _targetRotation;
    float _targetFOV;

    [SerializeField] bool _showGizmos;
    [SerializeField] bool _lerpInEditor;
    [SerializeField] Camera _mainCamera;
    [SerializeField, Expandable] MTRCameraRigSettings _settings;
    [SerializeField, Expandable] MTRCameraRigBounds _bounds;

    public float CameraZPos => Mathf.Abs(_settings.zPosOffset);
    public float CameraFOV => _settings.fov;
    public float CameraAspect => _mainCamera.aspect;

    public Vector3 BoundsCenter => _bounds.center;


    public float HalfWidth
    {
        get
        {
            // Calculate the half width of the camera frustum at the target depth
            float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * CameraZPos * CameraAspect;
            return Mathf.Abs(halfWidth); // Return the absolute value
        }
    }
    public float HalfHeight
    {
        get
        {
            // Calculate the half-height of the frustum at the given distance offset
            float HalfHeight = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * CameraZPos;
            return Mathf.Abs(HalfHeight); // Return the absolute value
        }
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
            _settings = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRCameraRigSettings>(SETTINGS_PATH, "DefaultCameraSettings");
        }

        if (_bounds == null)
        {
            _bounds = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRCameraRigBounds>(BOUNDS_PATH, "DefaultCameraBounds");
        }

        if (_mainCamera == null)
        {
            _mainCamera = GetComponentInChildren<Camera>();
            if (_mainCamera == null)
                _mainCamera = new GameObject("MTR Main Camera").AddComponent<Camera>();
        }
        _mainCamera.transform.SetParent(transform);


    }

    public void Update()
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
        Vector3 origin = BoundsCenter;
        Vector3 offset = new Vector3(
            _settings.xPosOffset,
            _settings.yPosOffset,
            _settings.zPosOffset
        );

        Vector3 newPosition = origin + offset;
        if (_bounds != null)
        {
            newPosition = EnforceBounds(newPosition);
        }
        return newPosition;
    }

    Vector3 EnforceBounds(Vector3 position)
    {

        _bounds.xAxisBounds.GetWorldValues(_bounds.center, out float minXBound, out float maxXBound);
        _bounds.yAxisBounds.GetWorldValues(_bounds.center, out float minYBound, out float maxYBound);

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

    Quaternion CalculateLookRotation(Vector3 originPosition, Vector3 cameraPosition)
    {
        Vector3 direction = (originPosition - cameraPosition);
        Quaternion lookRotation = Quaternion.LookRotation(direction);


        return lookRotation;
    }

    /// <summary>
    /// Calculate the frustum corners of the camera based on the given parameters.
    /// </summary>
    Vector3[] CalculateFrustumCorners(Vector3 position, Quaternion rotation)
    {
        Vector3[] frustumCorners = new Vector3[4];

        // Define the corners in local space (relative to the camera's orientation)
        Vector3 topLeft = new Vector3(-HalfWidth, HalfHeight, CameraZPos);
        Vector3 topRight = new Vector3(HalfWidth, HalfHeight, CameraZPos);
        Vector3 bottomLeft = new Vector3(-HalfWidth, -HalfHeight, CameraZPos);
        Vector3 bottomRight = new Vector3(HalfWidth, -HalfHeight, CameraZPos);

        // Transform the corners to world space
        frustumCorners[0] = position + rotation * topLeft;
        frustumCorners[1] = position + rotation * topRight;
        frustumCorners[2] = position + rotation * bottomLeft;
        frustumCorners[3] = position + rotation * bottomRight;

        return frustumCorners;
    }
    #endregion



    void UpdateCameraRig(bool useLerp)
    {

        // << CALCULATE TARGET VALUES >> -------------------------------------
        _targetPosition = CalculateTargetPosition();
        _targetRotation = Quaternion.identity;
        _targetFOV = _settings.fov;

        // << UPDATE CAMERA VALUES >> -------------------------------------
        if (useLerp)
        {
            // ( Lerp Camera Position ) ---------------------------------------
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _targetPosition, _settings.positionLerpSpeed * Time.deltaTime);

            // ( Slerp Camera Rotation ) ---------------------------------------
            _mainCamera.transform.rotation = Quaternion.Slerp(_mainCamera.transform.rotation, _targetRotation, _settings.rotationLerpSpeed * Time.deltaTime);

            // ( Lerp Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFOV, _settings.fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            // ( Set Camera Position ) ---------------------------------------
            _mainCamera.transform.position = _targetPosition;

            // ( Set Camera Rotation ) ---------------------------------------
            _mainCamera.transform.rotation = _targetRotation;

            // ( Set Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = _targetFOV;
        }
    }




    #region ( GIZMOS ) <PRIVATE_METHODS> ================================================
    void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // Draw the bounds
        if (_bounds != null)
            _bounds.DrawInScene();


    }

    void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

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
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), CameraZPos, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

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
        _mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), CameraZPos, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
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

        void OnSceneGUI()
        {
            _script.Update();
        }
    }
#endif
}

