using System;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent
        {
            // ======== [[ FIELDS ]] ================================== >>>>
            SpawnData _data;


            // ======== [[ PROPERTIES ]] ================================== >>>>
            public SpawnData Data { get => _data; set => _data = value; }
            public Spatial2D.AnchorPoint OriginAnchorPoint { get => _data.originPoint; set => _data.originPoint = value; }
            public Vector3 OriginAnchorPosition
            {
                get
                {
                    return Spatial2D.GetAnchorPointPosition(BaseCell.Position, BaseCell.Dimensions, OriginAnchorPoint);
                }
            }

            public Spatial2D.AnchorPoint TargetAnchorPoint { get => _data.anchorPoint; set => _data.anchorPoint = value; }
            public Vector3 TargetAnchorPosition
            {
                get
                {
                    return Spatial2D.GetAnchorPointPosition(BaseCell.Position, BaseCell.Dimensions, TargetAnchorPoint);
                }
            }

            // ======== [[ METHODS ]] ================================== >>>>
            // ---- (( INTERFACE )) ---- >>
            public override void OnInitialize(Cell2D cell)
            {
                base.OnInitialize(cell);
                _data = new SpawnData(cell.Key);
            }

            public override void DrawGizmos()
            {
                base.DrawGizmos();

                Gizmos.color = Color.grey;
                Gizmos.DrawCube(OriginAnchorPosition, 0.025f * Vector3.one);

                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(TargetAnchorPosition, 0.025f * Vector3.one);
            }

            // ---- (( PUBLIC METHODS )) ---- >>
            public void SetCellOrigin(Spatial2D.AnchorPoint origin)
            {
                OriginAnchorPoint = origin;
            }

            public void SetCellAnchor(Spatial2D.AnchorPoint anchor)
            {
                TargetAnchorPoint = anchor;
            }

            public void InstantiateObject(GameObject prefab, Transform parent = null)
            {
                GameObject obj = GameObject.Instantiate(prefab, parent);
                AdjustTransformToCellValues(obj.transform);
            }

            public void AdjustTransformToCellValues(Transform transform, Spatial2D.AnchorPoint transformOrigin = Spatial2D.AnchorPoint.CENTER, bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
            {
                Debug.Log($"Adjusting transform to cell values :: inheritWidth: {inheritWidth}, inheritHeight: {inheritHeight}, inheritNormal: {inheritNormal}", transform);

                // << GET BASE ELL VALUES >>
                BaseCell.Data.GetWorldSpaceValues(out Vector3 cellPosition, out Vector2 cellDimensions, out Vector3 cellNormal);
                Vector3 newPosition = transform.position;
                Vector2 newDimensions = transform.localScale;
                Vector3 newNormal = transform.forward;

                // << CALCULATE NEW DIMENSIONS >>
                // If both are inherited, set the scale to the dimensions
                if (inheritWidth && inheritHeight)
                {
                    newDimensions = cellDimensions;
                }
                // If just inherit width, set the scale to be a square with the width value
                else if (inheritWidth)
                {
                    newDimensions = new Vector2(cellDimensions.x, cellDimensions.x);
                }
                // If just inherit height, set the scale to be a square with the height value
                else if (inheritHeight)
                {
                    newDimensions = new Vector2(cellDimensions.y, cellDimensions.y);
                }

                // << GET NORMAL >>
                if (inheritNormal)
                    Spatial2D.SetTransformRotation_ToNormal(transform, cellNormal);

                // << CALCULATE NEW POSITION >>
                Spatial2D.SetTransformValues_WithOffset(transform, OriginAnchorPosition, newDimensions, OriginAnchorPoint);
            }

            // ======== [[ CONSTRUCTORS ]] ================================== >>>>
            public SpawnerComponent(Cell2D cell) : base(cell) { }


            // ======== [[ NESTED CLASSES ]] ================================== >>>>
            [System.Serializable]
            public class SpawnData
            {
                [SerializeField, ShowOnly] Vector2Int _cellKey = Vector2Int.zero;
                [SerializeField, ShowOnly] string _objectTypeName = "";
                [SerializeField, ShowAssetPreview] Object _objectToSpawn = null;
                public Spatial2D.AnchorPoint originPoint = Spatial2D.AnchorPoint.CENTER;
                public Spatial2D.AnchorPoint anchorPoint = Spatial2D.AnchorPoint.CENTER;

                public Vector2Int CellKey { get => _cellKey; set => _cellKey = value; }
                public Object ObjectToSpawn
                {
                    get => _objectToSpawn;
                    set
                    {
                        _objectToSpawn = value;
                        _objectTypeName = value.GetType().Name;
                    }
                }
                public Type ObjectType
                {
                    get
                    {
                        if (_objectToSpawn == null)
                        {
                            _objectTypeName = "NULL";
                            return null;
                        }

                        Type type = _objectToSpawn.GetType();
                        _objectTypeName = type.Name;
                        return type;
                    }
                }

                public SpawnData(Vector2Int key)
                {
                    _cellKey = key;
                }

                public SpawnData(Vector2Int key, Spatial2D.AnchorPoint origin, Spatial2D.AnchorPoint anchor, Object obj)
                {
                    _cellKey = key;
                    originPoint = origin;
                    anchorPoint = anchor;
                    _objectToSpawn = obj;

                    if (_objectToSpawn != null)
                        _objectTypeName = _objectToSpawn.GetType().Name;
                    else
                        _objectTypeName = "NULL";
                }

                public SpawnData(SpawnData data)
                {
                    _cellKey = data.CellKey;
                    _objectToSpawn = data.ObjectToSpawn;
                    originPoint = data.originPoint;
                    anchorPoint = data.anchorPoint;

                    if (_objectToSpawn != null)
                        _objectTypeName = _objectToSpawn.GetType().Name;
                    else
                        _objectTypeName = "NULL";
                }
            }
        }


    }
}