using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        [System.Serializable]
        public class SettingsConfig
        {
            // ======== [[ SERIALIZED FIELDS ]] ============================================================ >>>>
            [SerializeField, ShowOnly] Vector2 _cellDimensions = new Vector2(1, 1);
            [SerializeField, ShowOnly] Vector2 _cellSpacing = new Vector2(1, 1);
            [SerializeField, ShowOnly] Vector2 _cellBonding = new Vector2(0, 0);

            // ======== [[ PROPERTIES ]] ============================================================ >>>>
            public Vector2 CellDimensions => _cellDimensions;
            public Vector2 CellSpacing => _cellSpacing;
            public Vector2 CellBonding => _cellBonding;

            // ======== [[ CONSTRUCTORS ]] ============================================================ >>>>
            public SettingsConfig() { }
            public SettingsConfig(SettingsConfig originConfig)
            {
                _cellDimensions = originConfig._cellDimensions;
                _cellSpacing = originConfig._cellSpacing;
                _cellBonding = originConfig._cellBonding;
            }

            // ======== [[ METHODS ]] ============================================================ >>>>
            public void SetCellDimensions(Vector2 cellDimensions) => _cellDimensions = cellDimensions;
            public void SetCellSpacing(Vector2 cellSpacing) => _cellSpacing = cellSpacing;
            public void SetCellBonding(Vector2 cellBonding) => _cellBonding = cellBonding;
        }
    }
}