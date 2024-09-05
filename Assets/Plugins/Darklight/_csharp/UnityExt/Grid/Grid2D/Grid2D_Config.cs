using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{
    public partial class Grid2D
    {
        [System.Serializable]
        public class Config
        {
            // ======== [[ SERIALIZED FIELDS ]] ============================================================ >>>>
            [SerializeField, ShowOnly] bool _lockToTransform = true;
            [SerializeField, ShowOnly] Alignment _gridAlignment = Alignment.Center;
            [SerializeField, ShowOnly] Vector3 _gridPosition = new Vector3(0, 0, 0);
            [SerializeField, ShowOnly] Vector3 _gridNormal = Vector3.forward;
            [SerializeField, ShowOnly] Vector2Int _gridDimensions = new Vector2Int(3, 3);
            [SerializeField] Cell2D.SettingsConfig _cellConfig;

            // ======== [[ PROPERTIES ]] ============================================================ >>>>
            public bool LockToTransform => _lockToTransform;
            public Grid2D.Alignment GridAlignment => _gridAlignment;
            public Vector3 GridPosition => _gridPosition;
            public Vector3 GridNormal => _gridNormal;
            public Vector2Int GridDimensions => _gridDimensions;
            public Cell2D.SettingsConfig CellConfig => _cellConfig;

            // ======== [[ CONSTRUCTORS ]] ============================================================ >>>>
            public Config() { }
            public Config(Config originConfig)
            {
                _lockToTransform = originConfig._lockToTransform;
                _gridAlignment = originConfig._gridAlignment;
                _gridPosition = originConfig._gridPosition;
                _gridNormal = originConfig._gridNormal;
                _gridDimensions = originConfig._gridDimensions;
                _cellConfig = new Cell2D.SettingsConfig(originConfig._cellConfig);
            }

            // ======== [[ METHODS ]] ============================================================ >>>>
            public void SetLockToTransform(bool lockToTransform) => _lockToTransform = lockToTransform;
            public void SetGridAlignment(Alignment gridAlignment) => _gridAlignment = gridAlignment;
            public void SetGridPosition(Vector3 originPosition) => _gridPosition = originPosition;
            public void SetGridNormal(Vector3 gridNormal) => _gridNormal = gridNormal;
            public void SetGridDimensions(Vector2Int gridDimensions) => _gridDimensions = gridDimensions;
            public void SetCellConfig(Cell2D.SettingsConfig cellConfig) => _cellConfig = cellConfig;
        }
    }
}