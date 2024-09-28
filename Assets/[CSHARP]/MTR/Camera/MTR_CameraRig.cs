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
            float depth = Mathf.Abs(_preset.distanceOffset);

            // Calculate the half width of the camera frustum at the target depth
            float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * depth * _mainCamera.aspect;
            return Mathf.Abs(halfWidth); // Return the absolute value
        }
    }

    public float HalfHeight
    {
        get
        {
            float depth = Mathf.Abs(_preset.distanceOffset);

            // Calculate the half-height of the frustum at the given distance offset
            float halfHeight = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * depth;
            return Mathf.Abs(halfHeight); // Return the absolute value
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

    void OnValidate()
    {
        UpdateCameraRig(false);
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
        if (_preset.useBounds)
        {
            // Ensure that the camera stays within the bounds
            EnforceBounds(useLerp);

            /*
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
            */
        }

        // << UPDATE CAMERA ROTATION >> -----------------------------------
        /*
        Quaternion newRotation = Quaternion.identity;
        if (_preset.lookAtOriginX)
        {
            Vector3 excludeYValue = new Vector3(OriginPosition.x, _cameraTargetPosition.y, OriginPosition.z);
            newRotation = GetLookRotation(excludeYValue, _cameraTargetPosition);
        }
        */

        //Quaternion newRotation = AdjustRotationToStayInBounds();
        if (useLerp)
        {
            // ( Lerp Camera Position ) ---------------------------------------
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _cameraTargetPosition, _preset.positionLerpSpeed * Time.deltaTime);

            /*
            // ( Slerp Camera Rotation ) ---------------------------------------
            _mainCamera.transform.rotation = Quaternion.Slerp(_mainCamera.transform.rotation, newRotation, _preset.rotationLerpSpeed * Time.deltaTime);


            // ( Lerp Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFOV, _preset.fovLerpSpeed * Time.deltaTime);
            */
        }
        else
        {
            // ( Set Camera Position ) ---------------------------------------
            _mainCamera.transform.position = _cameraTargetPosition;
            //Debug.Log($"Setting Camera Position: {_cameraTargetPosition}");

            // ( Set Camera Rotation ) ---------------------------------------
            //_mainCamera.transform.rotation = newRotation;

            // ( Set Camera Field of View ) ---------------------------------
            _mainCamera.fieldOfView = _targetFOV;
        }
    }


    public static Vector3[] CalculateFrustumCorners(Vector3 position, Quaternion rotation, float fov, float aspectRatio, float depth)
    {
        Vector3[] frustumCorners = new Vector3[4];

        /*
            float halfWidth = Mathf.Tan(0.5f * Mathf.Deg2Rad * _targetFOV) * _preset.distanceOffset * _mainCamera.aspect;
            return Mathf.Abs(halfWidth); // Return the absolute value
        */

        // Calculate the height and width of the frustum at the specified depth
        float halfHeight = Mathf.Tan(0.5f * Mathf.Deg2Rad * fov) * depth;
        float halfWidth = halfHeight * aspectRatio;

        // Define the corners in local space (relative to the camera's orientation)
        Vector3 topLeft = new Vector3(-halfWidth, halfHeight, depth);
        Vector3 topRight = new Vector3(halfWidth, halfHeight, depth);
        Vector3 bottomLeft = new Vector3(-halfWidth, -halfHeight, depth);
        Vector3 bottomRight = new Vector3(halfWidth, -halfHeight, depth);

        // Transform the corners to world space
        frustumCorners[0] = position + rotation * topLeft;
        frustumCorners[1] = position + rotation * topRight;
        frustumCorners[2] = position + rotation * bottomLeft;
        frustumCorners[3] = position + rotation * bottomRight;

        return frustumCorners;
    }

    public Vector3 GetClosestValidPosition(Vector3 position, Quaternion rotation, float fov, float aspectRatio, float depth)
    {
        Vector3 adjustedPosition = position;
        Vector3[] frustumCorners = CalculateFrustumCorners(adjustedPosition, rotation, fov, aspectRatio, depth);

        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;

        xAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minXBound, out Vector3 maxXBound);
        yAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minYBound, out Vector3 maxYBound);

        for (int i = 0; i < frustumCorners.Length; i++)
        {
            Vector3 corner = frustumCorners[i];

            // ( Check X bounds ) -------------------------------------------
            if (corner.x < minXBound.x)
            {
                adjustedPosition.x = minXBound.x + HalfWidth; // Adjust to min X using HalfWidth
                //adjustedPosition.x = bounds.min.x; // Adjust to min X
            }
            else if (corner.x > maxXBound.x)
            {
                adjustedPosition.x = maxXBound.x - HalfWidth; // Adjust to max X using HalfWidth
                //adjustedPosition.x = bounds.max.x; // Adjust to max X
            }

            // ( Check Y bounds ) -------------------------------------------
            if (corner.y < minYBound.y)
            {
                //adjustedPosition.y = bounds.min.y + HalfHeight;  // Adjust to min Y using HalfHeight
            }
            else if (corner.y > maxYBound.y)
            {
                //adjustedPosition.y = bounds.max.y - HalfHeight;  // Adjust to max Y using HalfHeight
            }

            /*
            // ( Check Z bounds ) -------------------------------------------
            if (corner.z < bounds.min.z)
            {
                adjustedPosition.z = bounds.min.z;  // Adjust to min Z
            }
            else if (corner.z > bounds.max.z)
            {
                adjustedPosition.z = bounds.max.z;  // Adjust to max Z
            }
            */

            // Clamp the adjusted position to stay within the bounds
            //adjustedPosition.x = Mathf.Clamp(adjustedPosition.x, bounds.min.x, bounds.max.x);
            //adjustedPosition.y = Mathf.Clamp(adjustedPosition.y, bounds.min.y, bounds.max.y);
            //adjustedPosition.z = Mathf.Clamp(adjustedPosition.z, bounds.min.z, bounds.max.z);
        }
        return adjustedPosition;
    }



    private void EnforceBounds(bool useLerp)
    {
        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;
        SingleAxisBounds zAxisBounds = _preset.zAxisBounds;

        float depth = Mathf.Abs(_preset.distanceOffset);
        Vector3 newPosition = CalculateCameraTargetPosition();

        _cameraTargetPosition = GetClosestValidPosition(newPosition, _mainCamera.transform.rotation, _preset.fov, _mainCamera.aspect, depth);

        /*
        // Get the corners of the camera frustum at the target depth
        Vector3[] frustumCorners = CalculateFrustumCorners(newPosition, _mainCamera.transform.rotation, _preset.fov, _mainCamera.aspect, depth);

        // Transform the corners to world space
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = _mainCamera.transform.TransformPoint(frustumCorners[i]);
        }

        // Calculate the bounds and enforce them
        Vector3 worldMin = Vector3.Min(frustumCorners[0], frustumCorners[2]);
        Vector3 worldMax = Vector3.Max(frustumCorners[1], frustumCorners[3]);
        Debug.Log($"Frustum World Min: {worldMin} :: World Max: {worldMax}");

        // << ENFORCE X AXIS BOUNDS >> -------------------------------------
        xAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minXBound, out Vector3 maxXBound);
        // (( Enforce Camera Position ))
        if (newPosition.x < minXBound.x)
        {
            //newPosition.x += HalfWidth;
            Debug.Log($"Camera Min X Bound: POSITION {newPosition.x} < {minXBound.x}", this);
        }
        // (( Enforce Frustum Corners ))
        if (worldMin.x < minXBound.x && Mathf.Abs(minXBound.x - worldMin.x) > 0.1f)
        {
            newPosition.x = minXBound.x - worldMin.x;
            Debug.Log($"Camera Min X Bound: WORLD_MIN {worldMin.x} < {minXBound.x} :: Set X to {newPosition.x}", this);
        }


        if (newPosition.x > maxXBound.x)
        {
            //newPosition.x = maxXBound.x - HalfWidth;
            //Debug.Log($"Camera Max X Bound: POSITION {newPosition.x} > {maxXBound.x}", this);
        }
        if (worldMax.x > maxXBound.x)
        {
            newPosition.x = maxXBound.x - worldMax.x;
            Debug.Log($"Camera Max X Bound: WORLD_MAX {worldMax.x} > {maxXBound.x} :: Set X to {newPosition.x}", this);
        }
        */

        /*
        // << ENFORCE Y AXIS BOUNDS >> -------------------------------------
        yAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minYBound, out Vector3 maxYBound);
        if (newPosition.y < minYBound.y || worldMin.y < minYBound.y)
        {
            newPosition.y = minYBound.y + HalfWidth;
            //Debug.Log($"Camera Min Y Bound: {worldMin.y} < {minYBound.y}", this);
        }
        if (newPosition.y > maxYBound.y || worldMax.y > maxYBound.y)
        {
            newPosition.y = maxYBound.y - HalfWidth;
            //Debug.Log($"Camera Max Y Bound: {worldMax.y} > {maxYBound.y}", this);
        }

        // << ENFORCE Z AXIS BOUNDS >> -------------------------------------
        zAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minZBound, out Vector3 maxZBound);
        if (newPosition.z < minZBound.z || worldMin.z < minZBound.z)
        {
            newPosition.z = minZBound.z + depth;
            //Debug.Log($"Camera Min Z Bound: {worldMin.z} < {minZBound.z}", this);
        }
        if (newPosition.z > maxZBound.z || worldMax.z > maxZBound.z)
        {
            newPosition.z = maxZBound.z - depth;
            //Debug.Log($"Camera Max Z Bound: {worldMax.z} > {maxZBound.z}", this);
        }
        */

        if (!useLerp)
        {
            // ( Set Camera Position ) ---------------------------------------
            //_mainCamera.transform.position = newPosition;
            //Debug.Log($"Setting Camera Position: {newPosition}");
        }
    }


    Quaternion AdjustRotationToStayInBounds()
    {
        float depth = Mathf.Abs(_preset.distanceOffset);

        // Get the corners of the camera frustum at the target depth
        Vector3[] frustumCorners = new Vector3[4];
        _mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), depth, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // Transform the corners to world space
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = _mainCamera.transform.TransformPoint(frustumCorners[i]);
        }

        Vector3 worldMin = Vector3.Min(frustumCorners[0], frustumCorners[2]);
        Vector3 worldMax = Vector3.Max(frustumCorners[1], frustumCorners[3]);

        // Store the current rotation
        Quaternion initialRotation = _mainCamera.transform.rotation;

        // Adjust rotation if necessary
        bool adjusted = false;

        /*
        // Adjust Yaw (Horizontal Rotation)
        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        xAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minXBound, out Vector3 maxXBound);

        if (worldMin.x < minXBound.x || worldMax.x > maxXBound.x)
        {
            float yawCorrection = 0;

            if (worldMin.x < minXBound.x)
                yawCorrection += Mathf.Abs(minXBound.x - worldMin.x);
            if (worldMax.x > maxXBound.x)
                yawCorrection -= Mathf.Abs(worldMax.x - maxXBound.x);

            _mainCamera.transform.Rotate(Vector3.up, yawCorrection);
            adjusted = true;
        }
        */

        // Adjust Pitch (Vertical Rotation)
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;
        yAxisBounds.GetBoundWorldPositions(OriginPosition, out Vector3 minYBound, out Vector3 maxYBound);

        if (worldMin.y < minYBound.y || worldMax.y > maxYBound.y)
        {
            float pitchCorrection = 0;

            if (worldMin.y < minYBound.y)
                pitchCorrection += Mathf.Abs(minYBound.y - worldMin.y);
            if (worldMax.y > maxYBound.y)
                pitchCorrection -= Mathf.Abs(worldMax.y - maxYBound.y);

            //_mainCamera.transform.Rotate(Vector3.right, pitchCorrection);
            adjusted = true;
        }

        // If adjustments were made, return the new rotation
        if (adjusted)
        {
            return _mainCamera.transform.rotation;
        }

        // If no adjustments were needed, return the original rotation
        return initialRotation;
    }

    Vector3 CalculateCameraTargetPosition()
    {
        Vector3 origin = OriginPosition;
        Vector3 offset = new Vector3(
            _preset.horizontalOffset,
            _preset.verticalOffset,
            _preset.distanceOffset
        );
        return origin + offset;
    }

    void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // Draw the bounds
        SingleAxisBounds xAxisBounds = _preset.xAxisBounds;
        SingleAxisBounds yAxisBounds = _preset.yAxisBounds;
        SingleAxisBounds zAxisBounds = _preset.zAxisBounds;

        float depth = Mathf.Abs(_preset.distanceOffset);


        // Draw X Axis Bounds
        Gizmos.color = Color.red;
        xAxisBounds.DrawGizmos(OriginPosition, yAxisBounds.Max);

        // Draw Y Axis Bounds
        Gizmos.color = Color.green;
        yAxisBounds.DrawGizmos(OriginPosition + new Vector3(xAxisBounds.Min, 0, 0), xAxisBounds.Distance);

        // Draw Z Axis Bounds
        Gizmos.color = Color.blue;
        zAxisBounds.DrawGizmos(OriginPosition, yAxisBounds.Max);

        // Draw the camera frustum
        Gizmos.color = Color.yellow;
        DrawCameraFrustum(_mainCamera, depth);

        // Draw the bounds of the camera frustum in world space
        Gizmos.color = Color.cyan;
        Vector3[] frustumCorners = new Vector3[4];
        _mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), depth, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
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

        // Visualize the correction that would be applied to keep the camera in bounds
        if (worldMin.x < xAxisBounds.Min)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMin, new Vector3(xAxisBounds.Min, worldMin.y, worldMin.z));
        }
        if (worldMax.x > xAxisBounds.Max)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMax, new Vector3(xAxisBounds.Max, worldMax.y, worldMax.z));
        }
        if (worldMin.y < yAxisBounds.Min)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMin, new Vector3(worldMin.x, yAxisBounds.Min, worldMin.z));
        }
        if (worldMax.y > yAxisBounds.Max)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMax, new Vector3(worldMax.x, yAxisBounds.Max, worldMax.z));
        }
        if (worldMin.z < zAxisBounds.Min)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMin, new Vector3(worldMin.x, worldMin.y, zAxisBounds.Min));
        }
        if (worldMax.z > zAxisBounds.Max)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldMax, new Vector3(worldMax.x, worldMax.y, zAxisBounds.Max));
        }
    }

    void DrawCameraFrustum(Camera cam, float depth)
    {
        Vector3[] frustumCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), depth, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // Transform the corners to world space considering the entire transform hierarchy
        for (int i = 0; i < 4; i++)
        {
            frustumCorners[i] = cam.transform.TransformPoint(frustumCorners[i]);
        }

        // Check if the camera's forward direction needs adjustment due to parent transforms
        Vector3 cameraForward = cam.transform.forward;
        if (Vector3.Dot(cameraForward, Vector3.forward) < 0)
        {
            // If the camera is effectively pointing in the negative Z direction, flip the frustum
            Vector3 temp = frustumCorners[0];
            frustumCorners[0] = frustumCorners[3];
            frustumCorners[3] = temp;

            temp = frustumCorners[1];
            frustumCorners[1] = frustumCorners[2];
            frustumCorners[2] = temp;
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






    void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        /*
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
        */
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

