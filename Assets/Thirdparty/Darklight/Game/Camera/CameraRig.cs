using UnityEngine;
using System.Collections;
using Darklight.Game;
using System.Collections.Generic;
using Darklight.Game.Utility;
using Darklight.UnityExt.Editor;
using static Darklight.UnityExt.Editor.CustomInspectorGUI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    /// <summary>
    /// This Camera Rig is the main Monobehaviour reference for the full Camera System. 
    /// It should be set as the parent object for all cameras in the scene.
    /// </summary>
    [ExecuteAlways]
    public class CameraRig : MonoBehaviour
    {
        [SerializeField] UnityEngine.Camera[] _cameras = new UnityEngine.Camera[0];
        [SerializeField, ShowOnly] private Transform _focusTarget;
        public void SetFocusTarget(Transform target) => _focusTarget = target;

        [SerializeField, ShowOnly] private Vector3 _offsetPosition;
        [SerializeField, ShowOnly] private Vector3 _offsetRotation;


        [Header("Lerp Speed")]
        [SerializeField, Range(0, 10)] private float _positionLerpSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _rotationLerpSpeed = 5f;
        [SerializeField, Range(0, 10)] private float _fovLerpSpeed = 5f;

        [Space(10), Header("Distance")]
        [SerializeField, Range(1, 100)] private float _distanceZ = 10f; // distance from the targeton the Z axis

        [Header("Field of View")]
        [SerializeField, Range(0.1f, 45)] private float _baseFOV = 5f;
        [SerializeField, ShowOnly] private float _offsetFOV;
        public void SetOffsetFOV(float value) => _offsetFOV = value;
        [SerializeField, ShowOnly] private float _currentFOV;
        public float GetCurrentFOV()
        {
            _currentFOV = _baseFOV + _offsetFOV;
            return _currentFOV;
        }

        public virtual void Update()
        {
            // set the offsets
            _offsetPosition = new Vector3(0, 0, -_distanceZ);
            _offsetRotation = Vector3.zero;

            // << UPDATE THE RIG TRANSFORM >>
            if (_focusTarget)
            {
                // set the position
                Vector3 rigPosition = _focusTarget.position + _offsetPosition;
                transform.position = Vector3.Lerp(transform.position, rigPosition, _positionLerpSpeed * Time.deltaTime);

                // set the rotation
                Quaternion rigRotation = GetLookRotation(rigPosition, _focusTarget.position + _offsetRotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, rigRotation, _rotationLerpSpeed * Time.deltaTime);
            }

            // << UPDATE ALL CAMERAS >>
            foreach (UnityEngine.Camera camera in _cameras)
            {
                if (camera != null)
                {
                    // Reset the local position and rotation of the camera
                    camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    // Lerp the field of view
                    camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, GetCurrentFOV(), _fovLerpSpeed * Time.deltaTime);
                }
            }
        }

        Quaternion GetLookRotation(Vector3 originPosition, Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - originPosition).normalized;
            if (direction == Vector3.zero) return Quaternion.identity;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            return lookRotation;
        }
    }
}
