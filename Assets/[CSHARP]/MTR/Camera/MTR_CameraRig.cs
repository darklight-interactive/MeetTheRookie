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
public class MTR_CameraRig : MonoBehaviour
{
    const string PRESET_PATH = "Assets/Resources/MeetTheRookie/CameraPresets/";

    Vector3 _cameraTargetPosition;
    Vector3 _targetRotation;
    float _targetFOV;

    [SerializeField] bool _showGizmos;
    [SerializeField] bool _lerpInEditor;
    [SerializeField] Camera _mainCamera;
    [SerializeField, Expandable] MTR_CameraRigPreset _preset;

    public Vector3 OriginPosition
    {
        get
        {
            return transform.position;
        }
    }

    public float CameraFOV => _preset.fov;
    public float HalfWidth
    {
        get
        {
            float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * _preset.depth * _mainCamera.aspect;
            return Mathf.Abs(halfWidth); // Return the absolute value
        }
    }

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


    private void Awake()
    {
        if (_preset == null)
        {
            _preset = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTR_CameraRigPreset>(PRESET_PATH + "DefaultCameraPreset");
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

    void UpdateCameraRig(bool useLerp)
    {
        _targetFOV = _preset.fov;


        // << UPDATE CAMERA POSITION >> -----------------------------------
        _cameraTargetPosition = CalculateCameraTargetPosition();
        if (_preset.useBounds)
        {
            SingleAxisBounds xAxisBounds = _preset.xAxisBounds;

            // ( Contain the Camera within the xAxisBounds ) -----------------
            float camLeftFOVBound = _cameraTargetPosition.x - HalfWidth;
            float camRightFOVBound = _cameraTargetPosition.x + HalfWidth;

            xAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minBoundPos, out Vector3 maxBoundPos);
            if (camLeftFOVBound < minBoundPos.x)
            {
                _cameraTargetPosition.x = minBoundPos.x + HalfWidth;
                //Debug.Log($"Camera Left FOV Bound: {camLeftFOVBound} < {minBoundPos.x}");
            }
            else if (camRightFOVBound > maxBoundPos.x)
            {
                _cameraTargetPosition.x = maxBoundPos.x - HalfWidth;
                //Debug.Log($"Camera Right FOV Bound: {camRightFOVBound} > {maxBoundPos.x}");
            }
        }

        // << UPDATE CAMERA ROTATION >> -----------------------------------
        Quaternion newRotation = Quaternion.identity;
        if (_preset.lookAtOriginX)
        {
            Vector3 excludeYValue = new Vector3(OriginPosition.x, _cameraTargetPosition.y, OriginPosition.z);
            newRotation = GetLookRotation(excludeYValue, _cameraTargetPosition);
        }

        if (useLerp)
        {
            // ( Lerp Camera Position ) ---------------------------------------
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _cameraTargetPosition, _preset.positionLerpSpeed * Time.deltaTime);


            // ( Slerp Camera Rotation ) ---------------------------------------
            _mainCamera.transform.rotation = Quaternion.Slerp(_mainCamera.transform.rotation, newRotation, _preset.rotationLerpSpeed * Time.deltaTime);


            // ( Lerp Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFOV, _preset.fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            // ( Set Camera Position ) ---------------------------------------
            _mainCamera.transform.position = _cameraTargetPosition;

            // ( Set Camera Rotation ) ---------------------------------------
            _mainCamera.transform.rotation = newRotation;

            // ( Set Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = _targetFOV;
        }



        /*
        // set the rotation


        // << UPDATE ALL CAMERAS >>
        foreach (UnityEngine.Camera camera in _camerasInChildren)
        {
            if (camera != null)
            {
                // Reset the local position and rotation of the camera
                camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                // Lerp the field of view
                camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, GetCurrentFOV(), _fovLerpSpeed * Time.deltaTime);
            }
        }
        */
    }

    Vector3 CalculateCameraTargetPosition()
    {
        Vector3 origin = OriginPosition;
        Vector3 offset = new Vector3(
            _preset.horizontalOffset,
            _preset.verticalOffset,
            _preset.depth
        );
        return origin + offset;
    }

    void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;
        SingleAxisBounds zAxisBounds = _preset.zAxisBounds;
        // Draw X Axis Bounds
        Gizmos.color = Color.red;
        xAxisBounds.DrawGizmos(OriginPosition, yAxisBounds.Max);

        // Draw Y Axis Bounds
        Gizmos.color = Color.green;
        yAxisBounds.DrawGizmos(OriginPosition + new Vector3(xAxisBounds.Min, 0, 0), xAxisBounds.Distance);

        // Draw Z Axis Bounds
        Gizmos.color = Color.blue;
        zAxisBounds.DrawGizmos(OriginPosition, yAxisBounds.Max);
    }

    void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(OriginPosition, 0.025f);
        Gizmos.DrawSphere(CalculateCameraTargetPosition(), 0.025f);
        Gizmos.DrawLine(OriginPosition, CalculateCameraTargetPosition());

        // << ORIGIN -> OFFSET_Z >> ---------------------------------------
        Handles.color = Color.blue;
        Vector3 offsetZ = OriginPosition + new Vector3(0, 0, _preset.depth);
        Handles.DrawLine(OriginPosition, offsetZ);

        // << OFFSET_Z -> OFFSET_Y >> -------------------------------------
        Handles.color = Color.green;
        Vector3 offsetYZ = OriginPosition + new Vector3(0, _preset.verticalOffset, _preset.depth);
        Handles.DrawLine(offsetZ, offsetYZ);

        // << OFFSET_Y -> OFFSET_X >> -------------------------------------
        Handles.color = Color.red;
        Vector3 offsetXYZ = OriginPosition + new Vector3(_preset.horizontalOffset, _preset.verticalOffset, _preset.depth);
        Handles.DrawLine(offsetYZ, offsetXYZ);

        // << FOV BOUNDS >> ------------------------------------------------
        float camLeftFOVBound = _cameraTargetPosition.x - HalfWidth;
        float camRightFOVBound = _cameraTargetPosition.x + HalfWidth;

        Gizmos.color = Color.yellow;
        _preset.xAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minBound, out Vector3 maxBound);
        Gizmos.DrawRay(minBound, Vector3.back * 20);
        Gizmos.DrawRay(maxBound, Vector3.back * 20);

        Vector3 leftBoundPoint = new Vector3(camLeftFOVBound, OriginPosition.y, OriginPosition.z);
        Vector3 rightBoundPoint = new Vector3(camRightFOVBound, OriginPosition.y, OriginPosition.z);
        Gizmos.DrawLine(leftBoundPoint, rightBoundPoint);
    }

    Quaternion GetLookRotation(Vector3 originPosition, Vector3 cameraPosition)
    {
        Vector3 direction = (originPosition - cameraPosition);
        Quaternion lookRotation = Quaternion.LookRotation(direction);


        return lookRotation;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTR_CameraRig), true)]
    public class CameraRigCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTR_CameraRig _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTR_CameraRig)target;
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
                _script.Update();
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

