using System;
using Darklight.UnityExt.Utility;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent
        {

            // ======== [[ FIELDS ]] ================================== >>>>
            SpatialUtils2D.OriginPointTag _originTag = SpatialUtils2D.OriginPointTag.TOP_LEFT;

            // ======== [[ PROPERTIES ]] ================================== >>>>
            Vector3 originPosition => SpatialUtils2D.CalculateOriginOffsetPosition(BaseCell.Data.Position, BaseCell.Data.Dimensions, _originTag);


            // ======== [[ METHODS ]] ================================== >>>>
            // ---- (( VIRUTAL METHODS )) ---- >>
            public override void DrawGizmos()
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(originPosition, 0.025f * Vector3.one);
            }

            // ---- (( PUBLIC METHODS )) ---- >>
            public void SetOriginPointTag(SpatialUtils2D.OriginPointTag originPointTag)
            {
                _originTag = originPointTag;
            }

            public void InstantiateObject(GameObject prefab, Transform parent = null)
            {
                GameObject obj = GameObject.Instantiate(prefab, parent);
                AdjustTransformToCellValues(obj.transform);
            }

            public void AdjustTransformToCellValues(Transform transform,
                bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
            {
                Debug.Log($"Adjusting transform to cell values :: inheritWidth: {inheritWidth}, inheritHeight: {inheritHeight}, inheritNormal: {inheritNormal}", transform);

                // << GET CELL VALUES >>
                BaseCell.Data.GetWorldSpaceValues(out Vector3 cellPosition, out Vector2 cellDimensions, out Vector3 cellNormal);
                Vector3 newPosition = SpatialUtils2D.CalculateOriginOffsetPosition(cellPosition, cellDimensions, _originTag);
                Vector2 newDimensions = transform.localScale;
                Vector3 newNormal = transform.forward;

                // << GET DIMENSIONS FROM CELL >>

                // If both are inherited, set the scale to the dimensions
                if (inheritWidth && inheritHeight)
                {
                    Debug.Log($"Setting scale to dimensions {cellDimensions}", transform);
                    newDimensions = cellDimensions;
                }
                // If just inherit width, set the scale to be a square with the width value
                else if (inheritWidth)
                {
                    Debug.Log($"Setting scale to square {cellDimensions.x}", transform);
                    newDimensions = new Vector2(cellDimensions.x, cellDimensions.x);
                }
                // If just inherit height, set the scale to be a square with the height value
                else if (inheritHeight)
                {
                    Debug.Log($"Setting scale to square {cellDimensions.y}", transform);
                    newDimensions = new Vector2(cellDimensions.y, cellDimensions.y);
                }

                // << GET NORMAL >>
                if (inheritNormal) newNormal = cellNormal;

                // << ADJUST TRANSFORM >>
                SpatialUtils2D.SetTransformValues(transform, newPosition, newDimensions, newNormal);
            }

            public SpawnerComponent(Cell2D cell) : base(cell) { }
        }
    }
}