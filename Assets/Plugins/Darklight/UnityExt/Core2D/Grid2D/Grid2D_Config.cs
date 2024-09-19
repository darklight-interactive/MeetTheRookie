using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Grid2D
    {
        [System.Serializable]
        public class Config
        {
            // ======== [[ FIELDS ]] ============================================================ >>>>
            Transform _transform;
            Vector3 _gridWorldPosition = new Vector3(0, 0, 0);

            [SerializeField, ShowOnly] bool _lockPosToTransform = true;
            [SerializeField, ShowOnly] bool _lockNormalToTransform = true;
            [SerializeField, ShowOnly] Alignment _gridAlignment = Alignment.Center;
            [SerializeField, ShowOnly] Vector3 _gridLocalPosition = new Vector3(0, 0, 0);
            [SerializeField, ShowOnly] Vector3 _gridNormal = Vector3.up;
            [SerializeField, ShowOnly] Vector2Int _gridDimensions = new Vector2Int(3, 3);
            [SerializeField] Cell2D.SettingsConfig _cellConfig;

            // ======== [[ PROPERTIES ]] ============================================================ >>>>
            public bool LockPosToTransform { get => _lockPosToTransform; set => _lockPosToTransform = value; }
            public bool LockNormalToTransform { get => _lockNormalToTransform; set => _lockNormalToTransform = value; }
            public Alignment GridAlignment { get => _gridAlignment; set => _gridAlignment = value; }
            public Vector3 GridNormal { get => GetGridNormal(); set => _gridNormal = value; }
            public Vector2Int GridDimensions { get => _gridDimensions; set => _gridDimensions = value; }
            public Cell2D.SettingsConfig CellConfig { get => _cellConfig; set => _cellConfig = value; }

            // ======== [[ CONSTRUCTORS ]] ============================================================ >>>>
            public Config() { }
            public Config(Transform transform) => UpdateTransformData(transform);

            // ======== [[ METHODS ]] ============================================================ >>>>
            // ---- (( RUNTIME )) ---- >>
            public void UpdateTransformData(Transform transform)
            {
                // Set the private transform field
                _transform = transform;

                _gridWorldPosition = GetGridWorldPosition();
            }

            // ---- (( SETTERS )) ---- >>
            public void SetGridLocalPosition(Vector3 position)
            {
                _gridLocalPosition = position;
            }

            public Vector3 GetGridNormal()
            {
                Vector3 normal = _gridNormal;
                if (_lockNormalToTransform && _transform != null)
                {
                    normal = _transform.forward + _gridNormal;
                }
                return normal;
            }

            public Vector3 GetGridWorldPosition()
            {
                Vector3 position = _gridWorldPosition;
                if (_lockPosToTransform && _transform != null)
                    position = _transform.position + _gridLocalPosition;
                else
                    position = _gridLocalPosition;
                return position;
            }
        }
    }
}