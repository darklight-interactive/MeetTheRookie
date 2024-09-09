using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{
    public partial class Grid2D
    {
        [System.Serializable]
        public class Config
        {
            // ======== [[ FIELDS ]] ============================================================ >>>>
            Transform _transform;

            [SerializeField, ShowOnly] bool _lockToTransform = true;
            [SerializeField, ShowOnly] Alignment _gridAlignment = Alignment.Center;
            [SerializeField, ShowOnly] Vector3 _gridLocalPosition = new Vector3(0, 0, 0);
            [SerializeField, ShowOnly] Vector3 _gridWorldPosition = new Vector3(0, 0, 0);
            [SerializeField, ShowOnly] Vector3 _gridNormal = Vector3.forward;
            [SerializeField, ShowOnly] Vector2Int _gridDimensions = new Vector2Int(3, 3);
            [SerializeField] Cell2D.SettingsConfig _cellConfig;

            // ======== [[ PROPERTIES ]] ============================================================ >>>>
            public bool LockToTransform { get => _lockToTransform; set => _lockToTransform = value; }
            public Alignment GridAlignment { get => _gridAlignment; set => _gridAlignment = value; }
            public Vector3 GridLocalPosition { get => _gridLocalPosition; set => _gridLocalPosition = value; }
            public Vector3 GridWorldPosition { get => _gridWorldPosition; set => _gridWorldPosition = value; }
            public Vector3 GridNormal { get => _gridNormal; set => _gridNormal = value; }
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

                // Update the grid position based on the transform
                if (!_lockToTransform) return;
                _gridWorldPosition = transform.position + _gridLocalPosition;
                _gridNormal = transform.forward;
            }
        }
    }
}