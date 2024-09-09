using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Game.Grid
{
    [CreateAssetMenu(menuName = "Darklight/Grid2D/DataObject")]
    public class Grid2D_ConfigDataObject : ScriptableObject
    {
        #region ---- ( CUSTOM EDITOR DATA ) --------- >>
        public DropdownList<Vector3> editor_directions = new DropdownList<Vector3>()
        {
            { "Up", Vector3.up },
            { "Down", Vector3.down },
            { "Left", Vector3.left },
            { "Right", Vector3.right },
            { "Forward", Vector3.forward },
            { "Back", Vector3.back },
        };

        bool _showLocalPosition => _lockPosToTransform;
        bool _showWorldPosition => !_lockPosToTransform;
        bool _showNormal => !_lockNormalToTransform;

        #endregion

        // ======== [[ SERIALIZED FIELDS ]] ======================================================= >>>>
        // (( GRID2D CONFIG )) ---- >>
        [Header("-- GRID2D CONFIG -- >>")]
        [SerializeField] bool _lockPosToTransform = true;
        [SerializeField] bool _lockNormalToTransform = true;
        [SerializeField, ShowIf("_showLocalPosition")] Vector3 _gridLocalPosition = new Vector3(0, 0, 0);
        [Dropdown("editor_directions")]
        [SerializeField, ShowIf("_showNormal")] Vector3 _gridNormal = Vector3.forward;

        [Space(10)]
        [SerializeField] Grid2D.Alignment _gridAlignment = Grid2D.Alignment.Center;
        [SerializeField, Range(1, 100)] int _gridColumns = 3;
        [SerializeField, Range(1, 100)] int _gridRows = 3;

        // (( CELL2D CONFIG )) ---- >>
        [HorizontalLine(4, EColor.Gray)]
        [Header("-- CELL2D CONFIG -- >>")]
        [SerializeField, Range(0.1f, 10)] float _cellWidth = 1;
        [SerializeField, Range(0.1f, 10)] float _cellHeight = 1;

        [Space(10)]
        [SerializeField, Range(0f, 10)] float _cellSpacingX = 1;
        [SerializeField, Range(0f, 10)] float _cellSpacingY = 1;

        [Space(10)]
        [SerializeField, Range(0, 10)] float _cellBondingX = 0;
        [SerializeField, Range(0, 10)] float _cellBondingY = 0;

        // ======== [[ METHODS ]] ======================================================= >>>>
        public Cell2D.SettingsConfig CreateCellConfig()
        {
            Cell2D.SettingsConfig config = new Cell2D.SettingsConfig();
            config.SetCellDimensions(new Vector2(_cellWidth, _cellHeight));
            config.SetCellSpacing(new Vector2(_cellSpacingX, _cellSpacingY));
            config.SetCellBonding(new Vector2(_cellBondingX, _cellBondingY));
            return config;
        }

        public void UpdateConfig(Grid2D.Config config)
        {
            config.LockPosToTransform = _lockPosToTransform;
            config.LockNormalToTransform = _lockNormalToTransform;
            config.GridAlignment = _gridAlignment;
            config.GridDimensions = new Vector2Int(_gridColumns, _gridRows);
            config.GridNormal = _gridNormal;
            config.SetGridLocalPosition(_gridLocalPosition);

            config.CellConfig = CreateCellConfig();
        }
    }
}