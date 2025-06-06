using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_SpawnerComponent : Grid2D.BaseComponent
    {
        // ======== [[ FIELDS ]] ================================== >>>>
        Dictionary<Vector2Int, Cell2D.SpawnerComponent.InternalData> _dataMap =
            new Dictionary<Vector2Int, Cell2D.SpawnerComponent.InternalData>();

        [SerializeField, ReadOnly]
        List<Cell2D.SpawnerComponent.InternalData> _dataMapReadOnly =
            new List<Cell2D.SpawnerComponent.InternalData>();

        [SerializeField, Expandable]
        Grid2D_SpawnerDataObject _dataObject;

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

        protected override Cell2D.ComponentVisitor CellComponent_InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(
                ComponentTypeKey.SPAWNER,
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Cell2D.SpawnerComponent spawnerComponent =
                        cell.GetComponent<Cell2D.SpawnerComponent>();
                    return true;
                }
            );

        protected override Cell2D.ComponentVisitor CellComponent_UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(
                ComponentTypeKey.SPAWNER,
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Cell2D.SpawnerComponent spawnerComponent =
                        cell.GetComponent<Cell2D.SpawnerComponent>();
                    VisitCellSpawner(spawnerComponent);
                    spawnerComponent.OnUpdate();
                    return true;
                }
            );

        // ======== [[ METHODS ]] ================================== >>>>
        // ---- (( HANDLE DATA )) <PRIVATE_METHODS> ---- >>
        void VisitCellSpawner(Cell2D.SpawnerComponent cellSpawner)
        {
            Cell2D cell = cellSpawner.BaseCell;
            if (_dataObject == null)
                return;

            _dataObject.GetData(cell.Key, out Cell2D.SpawnerComponent.InternalData data);
            _dataMap[cell.Key] = data;
            cellSpawner.Data = data;
        }

        void UpdateData()
        {
            DataObject.UpdateData(BaseGrid);
            _dataMapReadOnly = _dataMap.Values.ToList();
        }

        // ---- (( INTERFACE )) ---- >>
        public override void OnInitialize(Grid2D grid)
        {
            CreateOrLoadDataObject();
            base.OnInitialize(grid);

            // Initialize the data map if it is null
            if (_dataMap == null)
                _dataMap = new Dictionary<Vector2Int, Cell2D.SpawnerComponent.InternalData>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateData();
        }

        public void AssignTransformToCell(Transform transform, Cell2D cell)
        {
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            cellSpawner.AttachTransformToCell(transform);
        }

        public List<Transform> GetAttachedTransformsAtCell(Cell2D cell)
        {
            if (_dataMap.ContainsKey(cell.Key))
            {
                Cell2D.SpawnerComponent.InternalData data = _dataMap[cell.Key];
                return data.AttachedTransforms;
            }
            return new List<Transform>();
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
            foreach (Cell2D.SpawnerComponent.InternalData data in DataObject.SerializedSpawnData)
            {
                data.OriginAnchor = _dataObject.DefaultOriginAnchor;
                data.TargetAnchor = _dataObject.DefaultTargetAnchor;
            }
        }

        Grid2D_SpawnerDataObject CreateOrLoadDataObject()
        {
            if (_dataObject != null)
                return _dataObject;

#if UNITY_EDITOR
            // Create or load the data object
            _dataObject =
                ScriptableObjectUtility.CreateOrLoadScriptableObject<Grid2D_SpawnerDataObject>(
                    Grid2D.DataObjectRegistry.DATA_PATH,
                    "DefaultSpawnerDataObject"
                );
#endif
            return _dataObject;
        }

        // ======== [[ NESTED TYPES ]] ================================== >>>>

        [System.Serializable]
        public class ObjectAnchorPair
        {
            [ShowOnly]
            public Spatial2D.AnchorPoint targetAnchor;

            [ShowAssetPreview]
            public Object obj;
        }
    }
}
