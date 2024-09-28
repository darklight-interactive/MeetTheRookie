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

    [SerializeField] Camera _mainCamera;
    [SerializeField, Expandable] MTR_CameraRigPreset _preset;

    protected Vector3 originPosition => this.transform.position;
    protected Vector3 cameraPosition
    {
        get
        {
            Vector3 origin = originPosition;
            Vector3 offset = new Vector3(
                _preset.horizontalOffset,
                _preset.verticalOffset,
                _preset.depth
            );
            return origin + offset;
        }
    }

    protected float cameraFOV => _preset.fov;
    protected Bounds cameraBound
    {
        get
        {
            /*
            float width = _preset.xAxisBounds.y - _preset.xAxisBounds.x;
            float height = _preset.yAxisBounds.y - _preset.yAxisBounds.x;
            return new Bounds(originPosition, new Vector3(width, height, 0));
            */
            return new Bounds(originPosition, Vector3.one);
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
            _mainCamera = new GameObject("Main Camera").AddComponent<Camera>();
        }
        _mainCamera.transform.SetParent(transform);

        /*
        CameraBounds[] bounds = FindObjectsByType<CameraBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (bounds.Length > 0)
        {
            cameraBounds = bounds[0];
        }
        else
        {
            cameraBounds = null;
        }
        */

    }

    public virtual void Update()
    {
        _mainCamera.transform.position = cameraPosition;
        _mainCamera.fieldOfView = cameraFOV;

        /*
        // set the offsets
        _offsetPosition = new Vector3(_distanceX, _distanceY, _distanceZ);
        _focusTargetPositionOffset = new Vector3(0, _focusOffsetY, 0);

        // set the position
        Vector3 newPosition = _focusTargetPosition + _offsetPosition;
        Vector3 offsetDirection = (newPosition - transform.position).normalized;
        float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * GetCurrentFOV()) * _distanceZ * _camerasInChildren[0].aspect;

        transform.position = Vector3.Lerp(transform.position, newPosition, _positionLerpSpeed * Time.deltaTime);
        */

        /*
        if (cameraBounds)
        {
            if ((transform.position.x - halfWidth > cameraBounds.leftBound && offsetDirection.x < 0) || (transform.position.x + halfWidth < cameraBounds.rightBound && offsetDirection.x > 0))
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, _positionLerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, _positionLerpSpeed * Time.deltaTime);
        }
        */

        /*
        // set the rotation
        Quaternion newRotation = GetLookRotation(newPosition, _focusTargetPosition + _focusTargetPositionOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, _rotationLerpSpeed * Time.deltaTime);

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

    void OnDrawGizmos()
    {
        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;
        SingleAxisBounds zAxisBounds = _preset.zAxisBounds;
        // Draw X Axis Bounds
        Gizmos.color = Color.red;
        xAxisBounds.DrawGizmos(originPosition, yAxisBounds.Max);

        // Draw Y Axis Bounds
        Gizmos.color = Color.green;
        yAxisBounds.DrawGizmos(originPosition + new Vector3(xAxisBounds.Min, 0, 0), xAxisBounds.Distance);

        // Draw Z Axis Bounds
        Gizmos.color = Color.blue;
        zAxisBounds.DrawGizmos(originPosition, yAxisBounds.Max);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(originPosition, 0.1f);
        Gizmos.DrawSphere(cameraPosition, 0.1f);
        Gizmos.DrawLine(originPosition, cameraPosition);

        // << ORIGIN -> OFFSET_Z >> ---------------------------------------
        Handles.color = Color.blue;
        Vector3 offsetZ = originPosition + new Vector3(0, 0, _preset.depth);
        Handles.DrawLine(originPosition, offsetZ);

        // << OFFSET_Z -> OFFSET_Y >> -------------------------------------
        Handles.color = Color.green;
        Vector3 offsetYZ = originPosition + offsetZ + new Vector3(0, _preset.verticalOffset, 0);
        Handles.DrawLine(offsetZ, offsetYZ);

        // << OFFSET_Y -> OFFSET_X >> -------------------------------------
        Handles.color = Color.red;
        Vector3 offsetXYZ = originPosition + offsetYZ + new Vector3(_preset.horizontalOffset, 0, 0);
        Handles.DrawLine(offsetYZ, offsetXYZ);
    }


    public void SetOffsetRotation(Transform mainTarget, Transform secondTarget)
    {
        float mainX = mainTarget.position.x;
        float secondX = secondTarget.position.x;
        float middleX = (secondX - mainX) / 2;
    }

    Quaternion GetLookRotation(Vector3 originPosition, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - originPosition).normalized;
        if (direction == Vector3.zero) return Quaternion.identity;
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
            }
        }

        void OnSceneGUI()
        {
            _script.Update();
        }
    }
#endif
}

