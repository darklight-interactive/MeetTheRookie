using System;
using Darklight.UnityExt.Utility;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent
        {
            bool _active;
            Spatial2D.AnchorPoint _cellOrigin = Spatial2D.AnchorPoint.CENTER;

            // ======== [[ FIELDS ]] ================================== >>>>

            // ======== [[ PROPERTIES ]] ================================== >>>>

            public Vector3 CellOriginPosition
            {
                get
                {
                    return Spatial2D.GetAnchorPointPosition(BaseCell.Position, BaseCell.Dimensions, _cellOrigin);
                }
            }

            // ======== [[ METHODS ]] ================================== >>>>
            // ---- (( VIRUTAL METHODS )) ---- >>
            public override void DrawGizmos()
            {
                Gizmos.color = _active ? Color.green : Color.red;
                Gizmos.DrawCube(CellOriginPosition, 0.025f * Vector3.one);
            }

            // ---- (( PUBLIC METHODS )) ---- >>
            public void SetCellOrigin(Spatial2D.AnchorPoint cellOrigin)
            {
                _cellOrigin = cellOrigin;
            }

            public void InstantiateObject(GameObject prefab, Transform parent = null)
            {
                GameObject obj = GameObject.Instantiate(prefab, parent);
                AdjustTransformToCellValues(obj.transform);
                _active = true;
            }

            public void AdjustTransformToCellValues(Transform transform, Spatial2D.AnchorPoint transformOrigin = Spatial2D.AnchorPoint.CENTER, bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
            {
                Debug.Log($"Adjusting transform to cell values :: inheritWidth: {inheritWidth}, inheritHeight: {inheritHeight}, inheritNormal: {inheritNormal}", transform);

                _active = true;

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
                Spatial2D.SetTransformValues_WithOffset(transform, CellOriginPosition, newDimensions, transformOrigin);
            }

            public SpawnerComponent(Cell2D cell) : base(cell) { }
        }
    }
}