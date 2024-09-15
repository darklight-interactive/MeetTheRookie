using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_SpawnerComponent : Grid2D_BaseComponent
    {

        // ======== [[ FIELDS ]] ================================== >>>>
        [SerializeField] bool _showGizmos = true;

        [Header("Cell Spawner Flags")]
        [SerializeField] bool _inheritCellWidth = true;
        [SerializeField] bool _inheritCellHeight = true;
        [SerializeField] bool _inheritCellNormal = true;

        [SerializeField] Spatial2D.AnchorPoint _default_OriginAnchorPoint = Spatial2D.AnchorPoint.CENTER;
        [SerializeField] Spatial2D.AnchorPoint _default_TargetAnchorPoint = Spatial2D.AnchorPoint.CENTER;
        [SerializeField] Object _default_ObjectToSpawn;


        /// <summary>
        /// The main dictionary that references all cell spawner data.
        /// </summary>
        Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData> _dataMap = new Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData>();

        /// <summary>
        /// Serialized Data modified by user in the inspector. This data is used to update the data map.
        /// </summary>
        [SerializeField, NonReorderable] List<Cell2D.SpawnerComponent.SpawnData> _serializedSpawnData;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        protected override Cell2D.ComponentVisitor CellComponent_InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.SPAWNER,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
                return true;
            });

        protected override Cell2D.ComponentVisitor CellComponent_UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.SPAWNER,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
                VisitCellSpawner(spawnerComponent);
                spawnerComponent.OnUpdate();
                return true;
            });

        // ======== [[ METHODS ]] ================================== >>>>
        // ---- (( INTERFACE )) ---- >>
        public override void OnUpdate()
        {
            base.OnUpdate();

            HashSet<Vector2Int> validKeys = new HashSet<Vector2Int>(BaseGrid.CellKeys);
            List<Vector2Int> keysToRemove = new List<Vector2Int>();

            // << REMOVE INVALID DATA MAP KEYS >>
            foreach (Vector2Int key in _dataMap.Keys)
            {
                if (!validKeys.Contains(key))
                    keysToRemove.Add(key);
            }
            foreach (Vector2Int key in keysToRemove)
                _dataMap.Remove(key);

            // << SERIALIZED DATA ITERATOR >>
            keysToRemove.Clear();
            foreach (Cell2D.SpawnerComponent.SpawnData data in _serializedSpawnData)
            {
                if (!validKeys.Contains(data.CellKey))
                    keysToRemove.Add(data.CellKey);
                else
                {
                    // Assign default object if the object to spawn is null
                    if (data.ObjectToSpawn == null && _default_ObjectToSpawn != null)
                        data.ObjectToSpawn = _default_ObjectToSpawn;
                }
            }
            foreach (Vector2Int key in keysToRemove)
                _serializedSpawnData.RemoveAll(x => x.CellKey == key);

            // Sort the _serializedSpawnData by CellKey in ascending order
            _serializedSpawnData.Sort((data1, data2) =>
            {
                int xComparison = data1.CellKey.x.CompareTo(data2.CellKey.x);
                if (xComparison == 0)
                {
                    // If x values are the same, compare y values
                    return data1.CellKey.y.CompareTo(data2.CellKey.y);
                }
                return xComparison;
            });
        }

        public override void DrawGizmos()
        {
            if (!_showGizmos) return;
            base.DrawGizmos();
        }

        // ---- (( HANDLE DATA )) ---- >>
        void VisitCellSpawner(Cell2D.SpawnerComponent cellSpawner)
        {
            Cell2D cell = cellSpawner.BaseCell;

            // << CELL SPAWNER DATA >>
            // Initialize the cell spawner data if it is null
            if (cellSpawner.Data == null)
                cellSpawner.Data = new Cell2D.SpawnerComponent.SpawnData(cell.Key, _default_OriginAnchorPoint, _default_TargetAnchorPoint, _default_ObjectToSpawn);

            // << DATA MAP >>
            // Initialize the data map if it is null
            if (_dataMap == null)
                _dataMap = new Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData>();
            // Else If the cell is not in the data map, add it
            else if (!_dataMap.ContainsKey(cell.Key))
                _dataMap.Add(cell.Key, cellSpawner.Data);

            // << SERIALIZED DATA >>
            // Initialize the serialized data if it is null
            if (_serializedSpawnData == null)
                _serializedSpawnData = new List<Cell2D.SpawnerComponent.SpawnData>();
            // Else If the serialized data is empty, add the cell spawner data
            else if (_serializedSpawnData.Count == 0)
            {
                _serializedSpawnData.Add(cellSpawner.Data);
                return;
            }
            // Else If the data key is not found in the serialized data, add it
            else if (!_serializedSpawnData.Exists(x => x.CellKey == cellSpawner.Data.CellKey))
                _serializedSpawnData.Add(cellSpawner.Data);
            // Else Update the data map from the serialized data
            else
            {
                Cell2D.SpawnerComponent.SpawnData savedSerializedData = _serializedSpawnData.Find(x => x.CellKey == cellSpawner.Data.CellKey);
                _dataMap[cellSpawner.Data.CellKey] = savedSerializedData;
                cellSpawner.Data = new Cell2D.SpawnerComponent.SpawnData(savedSerializedData);
            }
        }

        public void AdjustTransformToCellOrigin(Transform transform, Cell2D cell)
        {
            if (cell == null)
            {
                Debug.LogError("No best cell found");
                return;
            }

            // Adjust the transform to the best cell
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            Spatial2D.AnchorPoint anchorPoint = GetAnchorPointFromCell(cell);
            cellSpawner.AdjustTransformToCellValues(transform, anchorPoint, _inheritCellWidth, _inheritCellHeight, _inheritCellNormal);
        }

        public Spatial2D.AnchorPoint GetOriginPointFromCell(Cell2D cell)
        {
            if (_dataMap.ContainsKey(cell.Key))
                return _dataMap[cell.Key].originPoint;
            return Spatial2D.AnchorPoint.CENTER;
        }

        public Spatial2D.AnchorPoint GetAnchorPointFromCell(Cell2D cell)
        {
            if (_dataMap.ContainsKey(cell.Key))
                return _dataMap[cell.Key].anchorPoint;
            return Spatial2D.AnchorPoint.CENTER;
        }

        public void SetAllCellsToDefault()
        {
            foreach (Cell2D.SpawnerComponent.SpawnData data in _serializedSpawnData)
            {
                data.originPoint = _default_OriginAnchorPoint;
                data.anchorPoint = _default_TargetAnchorPoint;
                data.ObjectToSpawn = _default_ObjectToSpawn;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D_SpawnerComponent))]
        public class Grid2D_SpawnerComponentCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            Grid2D_SpawnerComponent _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D_SpawnerComponent)target;
                _script.Awake();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Set All Cells to Default"))
                {
                    _script.SetAllCellsToDefault();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }
}