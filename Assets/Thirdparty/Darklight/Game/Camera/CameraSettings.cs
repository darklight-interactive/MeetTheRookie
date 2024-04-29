using UnityEngine;

namespace Darklight.Game.Camera
{
    [CreateAssetMenu(menuName = "Darklight/CameraSettings")]
    public class CameraSettings : ScriptableObject
    {
        // >> ----------------- Local Camera Offset ----------------- <<
        [Header("Local Camera Position")]
        [SerializeField, Range(-10f, 10f)]
        private float _xLocalPos = 0;
        [SerializeField, Range(-10f, 10f)]
        private float _yLocalPos = 0;
        [SerializeField, Range(-100f, 100f)]
        private float _zLocalPos = 0;

        // >> ----------------- Camera Movement ----------------- <<
        [Header("Camera Movement Speeds")]
        [SerializeField, Range(0, 10f)]
        private int _followSpeed = 2;

        [SerializeField, Range(0, 10f)]
        private int _rotateSpeed = 2;

        // >> ----------------- Camera Projection ----------------- <<
        [Header("Camera Projection")]
        [SerializeField] private bool _isPerspective = true; // Default to perspective mode
        [SerializeField, Range(0, 100f)] private float _perspectiveFOV = 20f;
        [SerializeField] private float _orthographicSize = 5f;


        // >> ----------------- Getters ----------------- <<
        public Vector3 LocalCameraPosition => new Vector3(_xLocalPos, _yLocalPos, _zLocalPos);
        public float FollowSpeed => _followSpeed;
        public float RotateSpeed => _rotateSpeed;
        public bool IsPerspective => _isPerspective;
        public float PerspectiveFOV => _perspectiveFOV;
        public float OrthographicSize => _orthographicSize;
    }
}