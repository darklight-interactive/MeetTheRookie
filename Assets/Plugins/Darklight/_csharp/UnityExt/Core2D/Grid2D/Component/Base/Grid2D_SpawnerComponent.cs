using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Object = UnityEngine.Object;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.ObjectLibrary;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_SpawnerComponent : Grid2D.BaseComponent
    {

        // ======== [[ FIELDS ]] ================================== >>>>
        Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData> _dataMap = new Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData>();


        [SerializeField] bool _showGizmos = true;
        [SerializeField, Expandable] Grid2D_SpawnerDataObject _dataObject;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        Grid2D_SpawnerDataObject DataObject
        {
            get
            {
                if (_dataObject == null)
                    _dataObject = CreateOrLoadDataObject();
                return _dataObject;
            }
        }

        List<Cell2D.SpawnerComponent.SpawnData> SerializedData { get => DataObject.SerializedSpawnData; set => DataObject.SerializedSpawnData = value; }

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
        public override void OnInitialize(Grid2D grid)
        {
            CreateOrLoadDataObject();
            base.OnInitialize(grid);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateData();
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
            if (_dataObject == null) return;

            // << CELL SPAWNER DATA >> ------------------------------------ >>
            // Initialize the cell spawner data if it is null
            if (cellSpawner.Data == null)
                cellSpawner.Data = new Cell2D.SpawnerComponent.SpawnData(cell.Key, _dataObject.DefaultOriginAnchor, _dataObject.DefaultTargetAnchor, _dataObject.DefaultObjectToSpawn);

            // << DATA MAP >> --------------------------------------------- >>
            // Initialize the data map if it is null
            if (_dataMap == null)
                _dataMap = new Dictionary<Vector2Int, Cell2D.SpawnerComponent.SpawnData>();
            // Else If the cell is not in the data map, add it
            else if (!_dataMap.ContainsKey(cell.Key))
                _dataMap.Add(cell.Key, cellSpawner.Data);

            // << SERIALIZED DATA >> -------------------------------------- >>
            // Initialize the serialized data if it is null
            if (SerializedData == null)
                SerializedData = new List<Cell2D.SpawnerComponent.SpawnData>();
            // Else If the serialized data is empty, add the cell spawner data
            else if (SerializedData.Count == 0)
            {
                SerializedData.Add(cellSpawner.Data);
                return;
            }
            // Else If the data key is not found in the serialized data, add it
            else if (!SerializedData.Exists(x => x.CellKey == cellSpawner.Data.CellKey))
                SerializedData.Add(cellSpawner.Data);
            // Else Update the data map from the serialized data
            else
            {
                Cell2D.SpawnerComponent.SpawnData savedSerializedData = SerializedData.Find(x => x.CellKey == cellSpawner.Data.CellKey);
                _dataMap[cellSpawner.Data.CellKey] = savedSerializedData;
                cellSpawner.Data = new Cell2D.SpawnerComponent.SpawnData(savedSerializedData);
            }
        }

        void UpdateData()
        {
            // Get all the valid keys from the grid
            HashSet<Vector2Int> validKeys = new HashSet<Vector2Int>(BaseGrid.CellKeys);
            List<Vector2Int> keysToRemove = new List<Vector2Int>();

            // << DATA MAP ITERATOR >>
            foreach (Vector2Int key in _dataMap.Keys)
            {
                if (!validKeys.Contains(key))
                    keysToRemove.Add(key);
            }

            // Remove invalid keys from the data map
            foreach (Vector2Int key in keysToRemove)
                _dataMap.Remove(key);

            // << SERIALIZED DATA ITERATOR >>
            keysToRemove.Clear();
            foreach (Cell2D.SpawnerComponent.SpawnData data in SerializedData)
            {
                if (!validKeys.Contains(data.CellKey))
                    keysToRemove.Add(data.CellKey);
            }

            // Remove invalid keys from the serialized data
            foreach (Vector2Int key in keysToRemove)
                SerializedData.RemoveAll(x => x.CellKey == key);

            // Sort the _serializedSpawnData by CellKey in ascending order
            SerializedData.Sort((data1, data2) =>
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

        public void AssignTransformToCell(Transform transform, Cell2D cell)
        {
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            cellSpawner.AttachTransformToCell(transform, _dataObject.InheritCellWidth, _dataObject.InheritCellHeight, _dataObject.InheritCellNormal);
        }

        public Spatial2D.AnchorPoint GetOriginAnchor(Cell2D cell)
        {
            if (_dataMap.ContainsKey(cell.Key))
                return _dataMap[cell.Key].OriginAnchor;
            return Spatial2D.AnchorPoint.CENTER;
        }

        public Spatial2D.AnchorPoint GetTargetAnchor(Cell2D cell)
        {
            if (_dataMap.ContainsKey(cell.Key))
                return _dataMap[cell.Key].TargetAnchor;
            return Spatial2D.AnchorPoint.CENTER;
        }

        public void SetAllCellsToDefault()
        {
            foreach (Cell2D.SpawnerComponent.SpawnData data in SerializedData)
            {
                data.OriginAnchor = _dataObject.DefaultOriginAnchor;
                data.TargetAnchor = _dataObject.DefaultTargetAnchor;
            }
        }

        Grid2D_SpawnerDataObject CreateOrLoadDataObject()
        {
            if (_dataObject != null) return _dataObject;

            // Create or load the data object
            string name = this.name;
            _dataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<Grid2D_SpawnerDataObject>(Grid2D.DataObjectRegistry.DATA_PATH, name);
            _dataObject.name = name;
            return _dataObject;
        }


        // ======== [[ NESTED TYPES ]] ================================== >>>>

        [System.Serializable]
        public class ObjectAnchorPair
        {
            [ShowOnly] public Spatial2D.AnchorPoint targetAnchor;
            [ShowAssetPreview] public Object obj;
        }




#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Grid2D_SpawnerComponent))]
        public class CustomEditor : UnityEditor.Editor
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

                if (_script._dataObject == null && GUILayout.Button("Create Data Object"))
                {
                    _script.CreateOrLoadDataObject();
                }

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