using System;
using Darklight.UnityExt.Utility;
using UnityEngine;
using static Darklight.UnityExt.Core2D.SpatialUtils2D;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        public class SpawnerComponent : BaseComponent
        {
            public void SpawnObject(GameObject prefab, OriginPoint originPoint = OriginPoint.CENTER,
                bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
            {
                GameObject instance = GameObject.Instantiate(prefab, BaseCell.Data.Position, Quaternion.identity);
                AdjustTransform(instance.transform, originPoint, inheritWidth, inheritHeight, inheritNormal);
            }

            public void AdjustTransform(Transform transform, OriginPoint originPoint = OriginPoint.CENTER,
                bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
            {
                Vector3 position = BaseCell.Data.Position;
                Vector3 normal = transform.forward;
                Vector2 dimensions = transform.localScale;

                // << GET DIMENSIONS >>
                if (inheritWidth) dimensions.x = BaseCell.Data.Dimensions.x;
                if (inheritHeight) dimensions.y = BaseCell.Data.Dimensions.y;

                // << GET NORMAL >>
                if (inheritNormal) normal = BaseCell.Data.Normal;

                // << GET POSITION >>
                position = SpatialUtils2D.CalculatePositionWithOriginOffset(position, dimensions, originPoint);

                // << ADJUST TRANSFORM >>
                transform.position = position;
                transform.rotation = Quaternion.LookRotation(normal, Vector3.up);
                transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
            }

            public void AdjustTransformToSquareFromWidth(Transform transform, OriginPoint originPoint = OriginPoint.CENTER, bool inheritNormal = true)
            {
                Vector3 position = BaseCell.Data.Position;
                Vector3 normal = transform.forward;
                Vector2 dimensions = transform.localScale;

                // << GET DIMENSIONS >>
                dimensions = new Vector2(BaseCell.Data.Dimensions.x, BaseCell.Data.Dimensions.x);

                // << GET NORMAL >>
                if (inheritNormal) normal = BaseCell.Data.Normal;

                // << GET POSITION >>
                position = SpatialUtils2D.CalculatePositionWithOriginOffset(position, dimensions, originPoint);

                // << ADJUST TRANSFORM >>
                transform.position = position;
                transform.rotation = Quaternion.LookRotation(normal, Vector3.up);
                transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
            }

            public SpawnerComponent(Cell2D cell) : base(cell) { }
        }
    }
}