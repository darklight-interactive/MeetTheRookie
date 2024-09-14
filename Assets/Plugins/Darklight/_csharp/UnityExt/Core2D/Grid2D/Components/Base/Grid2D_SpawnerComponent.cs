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
        Dictionary<Vector2Int, CellAnchorData> _cellAnchorData = new Dictionary<Vector2Int, CellAnchorData>();

        [System.Serializable]
        public class CellAnchorData
        {
            [ShowOnly] public Vector2Int cellKey;
            public Spatial2D.AnchorPoint originPoint;
            public Spatial2D.AnchorPoint anchorPoint;
            public CellAnchorData(Vector2Int key, Spatial2D.AnchorPoint anchor)
            {
                cellKey = key;
                anchorPoint = anchor;
            }
        }

        public bool showGizmos = true;
        public bool inheritWidth = true;
        public bool inheritHeight = true;
        public bool inheritNormal = true;


        [SerializeField]
        public List<CellAnchorData> cellAnchorData;


        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.SPAWNER,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
                return true;
            });

        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.SPAWNER,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();

                // Update the cell anchor data dictionary to match the serialized data
                if (cellAnchorData != null)
                {
                    foreach (CellAnchorData data in cellAnchorData)
                    {
                        if (!_cellAnchorData.ContainsKey(data.cellKey))
                            _cellAnchorData.Add(data.cellKey, data);
                        else
                            _cellAnchorData[data.cellKey] = data;
                    }
                }

                if (_cellAnchorData == null || _cellAnchorData.ContainsKey(cell.Key) == false)
                {
                    //Debug.LogError($"No cell anchor data found for cell: {cell.Key}");
                    return false;
                }
                spawnerComponent.SetCellOrigin(_cellAnchorData[cell.Key].originPoint);
                spawnerComponent.SetCellAnchor(_cellAnchorData[cell.Key].anchorPoint);
                spawnerComponent.OnUpdate();

                cellAnchorData = new List<CellAnchorData>(_cellAnchorData.Values);
                return true;
            });

        public override void DrawGizmos()
        {
            if (!showGizmos) return;
            base.DrawGizmos();
        }


        public void InstantiateObjectAtCell(GameObject obj, Cell2D cell)
        {
            if (cell == null)
            {
                Debug.LogError("No best cell found");
                return;
            }

            // Instantiate the object at the best cell
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            cellSpawner.InstantiateObject(obj);
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
            cellSpawner.AdjustTransformToCellValues(transform, anchorPoint, inheritWidth, inheritHeight, inheritNormal);
        }

        public Spatial2D.AnchorPoint GetOriginPointFromCell(Cell2D cell)
        {
            if (_cellAnchorData.ContainsKey(cell.Key))
                return _cellAnchorData[cell.Key].originPoint;
            return Spatial2D.AnchorPoint.CENTER;
        }

        public Spatial2D.AnchorPoint GetAnchorPointFromCell(Cell2D cell)
        {
            if (_cellAnchorData.ContainsKey(cell.Key))
                return _cellAnchorData[cell.Key].anchorPoint;
            return Spatial2D.AnchorPoint.CENTER;
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

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

    }
}