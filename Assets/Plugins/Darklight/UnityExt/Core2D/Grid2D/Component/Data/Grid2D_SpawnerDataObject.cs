using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Core2D;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    [CreateAssetMenu(menuName = "Darklight/Grid2D/SpawnerDataObject")]
    public class Grid2D_SpawnerDataObject : ScriptableObject
    {
        [Header("Flags")]
        [SerializeField]
        bool _inheritCellWidth = true;

        [SerializeField]
        bool _inheritCellHeight = true;

        [SerializeField]
        bool _inheritCellNormal = true;

        [Header("Anchor Points")]
        [SerializeField]
        Spatial2D.AnchorPoint _default_OriginAnchorPoint = Spatial2D.AnchorPoint.CENTER;

        [SerializeField]
        Spatial2D.AnchorPoint _default_TargetAnchorPoint = Spatial2D.AnchorPoint.CENTER;

        [Header("Serialized Data")]
        /// <summary>
        /// Serialized Data modified by user in the inspector. This data is used to update the data map.
        /// </summary>
        [SerializeField, NonReorderable]
        List<Cell2D.SpawnerComponent.InternalData> _serializedCellSpawnData =
            new List<Cell2D.SpawnerComponent.InternalData>();

        public bool InheritCellWidth => _inheritCellWidth;
        public bool InheritCellHeight => _inheritCellHeight;
        public bool InheritCellNormal => _inheritCellNormal;
        public Spatial2D.AnchorPoint DefaultOriginAnchor => _default_OriginAnchorPoint;
        public Spatial2D.AnchorPoint DefaultTargetAnchor => _default_TargetAnchorPoint;
        public IEnumerable<Cell2D.SpawnerComponent.InternalData> SerializedSpawnData =>
            _serializedCellSpawnData;

        public void GetData(Vector2Int key, out Cell2D.SpawnerComponent.InternalData data)
        {
            data = _serializedCellSpawnData.FirstOrDefault(x => x.CellKey == key);
        }

        public void SortSerializedData()
        {
            // Sort the _serializedSpawnData by CellKey in ascending order
            _serializedCellSpawnData.Sort(
                (data1, data2) =>
                {
                    int xComparison = data1.CellKey.x.CompareTo(data2.CellKey.x);
                    if (xComparison == 0)
                    {
                        // If x values are the same, compare y values
                        return data1.CellKey.y.CompareTo(data2.CellKey.y);
                    }
                    return xComparison;
                }
            );
        }

        public void UpdateData(Grid2D grid)
        {
            // Get all the valid keys from the grid
            HashSet<Vector2Int> validKeys = new HashSet<Vector2Int>(grid.CellKeys);
            List<Vector2Int> keysToRemove = new List<Vector2Int>();

            // If the grid is null
            if (grid == null)
                return;
            if (
                _serializedCellSpawnData.Count == validKeys.Count
                && _serializedCellSpawnData.All(data => validKeys.Contains(data.CellKey))
            )
                return;

            // Dangerous debug - be wary this will print a lot of logs
            //Debug.Log($"{this.name} - Valid keys: {string.Join(", ", validKeys.ToList())}", this);

            // Create a dictionary to track unique entries by CellKey
            Dictionary<Vector2Int, Cell2D.SpawnerComponent.InternalData> uniqueEntries =
                new Dictionary<Vector2Int, Cell2D.SpawnerComponent.InternalData>();

            // First, process existing entries to keep the most recent one for each key
            foreach (Cell2D.SpawnerComponent.InternalData data in _serializedCellSpawnData)
            {
                if (!validKeys.Contains(data.CellKey))
                {
                    keysToRemove.Add(data.CellKey);
                    continue;
                }

                // Update the data properties
                data.InheritCellWidth = InheritCellWidth;
                data.InheritCellHeight = InheritCellHeight;
                data.InheritCellNormal = InheritCellNormal;

                // If we haven't seen this key before, add it to our unique entries
                if (!uniqueEntries.ContainsKey(data.CellKey))
                {
                    uniqueEntries[data.CellKey] = data;
                }
            }

            // Add any missing valid keys
            foreach (Vector2Int key in validKeys)
            {
                if (!uniqueEntries.ContainsKey(key))
                {
                    // Find existing data for this key if it exists
                    Cell2D.SpawnerComponent.InternalData existingData =
                        _serializedCellSpawnData.FirstOrDefault(x => x.CellKey == key);

                    var newData = new Cell2D.SpawnerComponent.InternalData(key)
                    {
                        InheritCellWidth = InheritCellWidth,
                        InheritCellHeight = InheritCellHeight,
                        InheritCellNormal = InheritCellNormal,
                        // Use existing anchor values if available, otherwise use defaults
                        OriginAnchor = existingData?.OriginAnchor ?? _default_OriginAnchorPoint,
                        TargetAnchor = existingData?.TargetAnchor ?? _default_TargetAnchorPoint
                    };
                    uniqueEntries[key] = newData;
                    Debug.Log(
                        $"{this.name} - Added key: {key} to serialized data",
                        grid.gameObject
                    );
                }
            }

            // Update the serialized data list with unique entries
            _serializedCellSpawnData.Clear();
            _serializedCellSpawnData.AddRange(uniqueEntries.Values);

            // Sort the data to maintain consistent order
            SortSerializedData();
        }
    }
}
