using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent, IUnityEditorListener
        {

            // ======== [[ FIELDS ]] ================================== >>>>
            InternalData _data;

            // ======== [[ PROPERTIES ]] ================================== >>>>
            public InternalData Data { get => _data; set => _data = value; }
            public Spatial2D.AnchorPoint OriginAnchorPoint
            {
                get
                {
                    if (_data == null)
                        return Spatial2D.AnchorPoint.CENTER;
                    return _data.OriginAnchor;
                }
                set
                {
                    if (_data == null)
                        _data = new InternalData(BaseCell.Key);
                    _data.OriginAnchor = value;
                }
            }
            public Vector3 OriginAnchorPosition
            {
                get
                {
                    if (BaseCell == null)
                        return Vector3.zero;
                    return Spatial2D.GetAnchorPointPosition(BaseCell.Position, BaseCell.Dimensions, OriginAnchorPoint);
                }
            }
            public Spatial2D.AnchorPoint TargetAnchorPoint
            {
                get
                {
                    if (_data == null)
                        return Spatial2D.AnchorPoint.CENTER;
                    return _data.TargetAnchor;
                }
                set
                {
                    if (_data == null)
                        _data = new InternalData(BaseCell.Key);
                    _data.TargetAnchor = value;
                }
            }
            public Vector3 TargetAnchorPosition
            {
                get
                {
                    if (BaseCell == null)
                        return Vector3.zero;
                    return Spatial2D.GetAnchorPointPosition(BaseCell.Position, BaseCell.Dimensions, TargetAnchorPoint);
                }
            }

            // ======== [[ METHODS ]] ================================== >>>>
            // ---- (( INTERFACE )) ---- >>
            public void OnEditorReloaded()
            {
                DestroyAllAttachedTransforms();
            }

            public override void OnInitialize(Cell2D cell)
            {
                base.OnInitialize(cell);
                _data = new InternalData(cell.Key);
            }

            public override void DrawGizmos()
            {
                base.DrawGizmos();
                Gizmos.color = Color.grey;
                Gizmos.DrawCube(OriginAnchorPosition, 0.0125f * Vector3.one);

                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(TargetAnchorPosition, 0.0125f * Vector3.one);
            }

            public override void DrawSelectedGizmos()
            {
                base.DrawSelectedGizmos();


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

            public void DestroyAllAttachedTransforms()
            {
                for (int i = 0; i < _data.AttachedTransforms.Count; i++)
                {
                    ObjectUtility.DestroyAlways(_data.AttachedTransforms[i].gameObject);
                }
                _data.AttachedTransforms.Clear();
            }

            public void AttachTransformToCell(Transform transform)
            {
                if (!_data.AttachedTransforms.Contains(transform))
                    _data.AttachedTransforms.Add(transform);

                // << GET DEFAULT VALUES >>
                Vector3 newPosition = transform.position;
                Vector2 newDimensions = transform.localScale;
                Vector3 newNormal = transform.forward;

                // << GET BASE CELL VALUES >>
                BaseCell.Data.GetWorldSpaceValues(out Vector3 cellPosition, out Vector2 cellDimensions, out Vector3 cellNormal);

                // << SET SCALE >>
                SetTransformToCellDimensions(transform);

                // << SET NORMAL >>
                if (_data.InheritCellNormal)
                    Spatial2D.SetTransformRotation_ToNormal(transform, cellNormal);

                // << SET NEW POSITION >>
                Spatial2D.SetTransformPos_ToAnchor(transform, OriginAnchorPosition, newDimensions, OriginAnchorPoint);
            }

            void SetTransformToCellDimensions(Transform transform)
            {
                Vector2 cellDimensions = BaseCell.Data.Dimensions;
                bool inheritWidth = _data.InheritCellWidth;
                bool inheritHeight = _data.InheritCellHeight;

                // << SET NEW DIMENSIONS >>
                // If both are inherited, set the scale to the dimensions
                if (inheritWidth && inheritHeight)
                {
                    Spatial2D.SetTransformScale_ToDimensions(transform, cellDimensions);
                }
                // If just inherit width, set the scale to be a square with the width value
                else if (inheritWidth)
                {
                    Spatial2D.SetTransformScale_ToSquareRatio(transform, cellDimensions.x);
                }
                // If just inherit height, set the scale to be a square with the height value
                else if (inheritHeight)
                {
                    Spatial2D.SetTransformScale_ToSquareRatio(transform, cellDimensions.y);
                }
            }

            // ======== [[ CONSTRUCTORS ]] ================================== >>>>
            public SpawnerComponent(Cell2D cell) : base(cell) { }


            // ======== [[ NESTED CLASSES ]] ================================== >>>>
            [System.Serializable]
            public class InternalData
            {
                // ======== [[ FIELDS ]] ================================== >>>>
                [SerializeField, ShowOnly] Vector2Int _cellKey = Vector2Int.zero;
                [SerializeField, NonReorderable, ShowOnly] List<Transform> _attachedTransforms = new List<Transform>();

                [Header("Settings")]
                [SerializeField, ShowOnly] bool _inheritCellWidth = true;
                [SerializeField, ShowOnly] bool _inheritCellHeight = true;
                [SerializeField, ShowOnly] bool _inheritCellNormal = true;

                [Header("Anchors")]
                [Tooltip("This determines the origin point of the object to be spawned and the cell anchor point that the object will be placed on")]
                [SerializeField] Spatial2D.AnchorPoint _originAnchor = Spatial2D.AnchorPoint.CENTER;
                [Tooltip("This is an identifier for the cell to be used as a 'direction anchor' to determine properties of the spawned object")]
                [SerializeField] Spatial2D.AnchorPoint _targetAnchor = Spatial2D.AnchorPoint.CENTER;


                // ======== [[ PROPERTIES ]] ================================== >>>>
                public Vector2Int CellKey { get => _cellKey; set => _cellKey = value; }
                public List<Transform> AttachedTransforms { get => _attachedTransforms; set => _attachedTransforms = value; }
                public bool InheritCellWidth { get => _inheritCellWidth; set => _inheritCellWidth = value; }
                public bool InheritCellHeight { get => _inheritCellHeight; set => _inheritCellHeight = value; }
                public bool InheritCellNormal { get => _inheritCellNormal; set => _inheritCellNormal = value; }
                public Spatial2D.AnchorPoint OriginAnchor { get => _originAnchor; set => _originAnchor = value; }
                public Spatial2D.AnchorPoint TargetAnchor { get => _targetAnchor; set => _targetAnchor = value; }

                // ======== [[ CONSTRUCTORS ]] ================================== >>>>
                public InternalData(Vector2Int key)
                {
                    _cellKey = key;
                }

                public InternalData(InternalData data)
                {
                    _cellKey = data.CellKey;
                    _inheritCellWidth = data._inheritCellWidth;
                    _inheritCellHeight = data._inheritCellHeight;
                    _inheritCellNormal = data._inheritCellNormal;
                    _originAnchor = data._originAnchor;
                    _targetAnchor = data._targetAnchor;
                }
            }
        }


    }
}