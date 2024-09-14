using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public static class SpatialUtils2D
    {
        // Enum to specify origin points
        public enum OriginPointTag
        {
            TOP_LEFT, TOP_CENTER, TOP_RIGHT,
            CENTER_LEFT, CENTER, CENTER_RIGHT,
            BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT
        }

        /// <summary>
        /// Calculates a 2D object's center position based on a given spawn position and origin point.
        /// </summary>
        /// <param name="initialPosition">The  position of the object.</param>
        /// <param name="dimensions">The dimensions of the cell.</param>
        /// <param name="originTag">The origin point that was used for spawning.</param>
        /// <returns>Original center position of the cell.</returns>
        public static Vector3 CalculateOriginOffsetPosition(Vector3 initialPosition, Vector2 dimensions, OriginPointTag originTag)
        {
            Vector3 offset = Vector3.zero;

            switch (originTag)
            {
                case OriginPointTag.TOP_LEFT:
                    offset = new Vector3(-dimensions.x / 2, dimensions.y / 2, 0);
                    break;
                case OriginPointTag.TOP_CENTER:
                    offset = new Vector3(0, dimensions.y / 2, 0);
                    break;
                case OriginPointTag.TOP_RIGHT:
                    offset = new Vector3(dimensions.x / 2, dimensions.y / 2, 0);
                    break;
                case OriginPointTag.CENTER_LEFT:
                    offset = new Vector3(-dimensions.x / 2, 0, 0);
                    break;
                case OriginPointTag.CENTER:
                    // Center does not require an offset
                    offset = Vector3.zero;
                    break;
                case OriginPointTag.CENTER_RIGHT:
                    offset = new Vector3(dimensions.x / 2, 0, 0);
                    break;
                case OriginPointTag.BOTTOM_LEFT:
                    offset = new Vector3(-dimensions.x / 2, -dimensions.y / 2, 0);
                    break;
                case OriginPointTag.BOTTOM_CENTER:
                    offset = new Vector3(0, -dimensions.y / 2, 0);
                    break;
                case OriginPointTag.BOTTOM_RIGHT:
                    offset = new Vector3(-dimensions.x / 2, -dimensions.y / 2, 0);
                    break;
            }

            return initialPosition + offset;
        }

        public static void AlignTransformsByOrigin(Transform source, Transform target, OriginPointTag originPoint = OriginPointTag.CENTER)
        {
            // All values begin as the source's values
            GetTransformValues(source, out Vector3 position, out Vector2 dimensions, out Vector3 normal);

            // << GET POSITION >>
            Vector3 originPosition = CalculateOriginOffsetPosition(position, dimensions, originPoint);

            // << ADJUST TRANSFORM >>
            SetTransformValues(target, originPosition, dimensions, normal);
        }

        public static void GetTransformValues(Transform transform, out Vector3 position, out Vector2 dimensions, out Vector3 normal)
        {
            position = transform.position;
            dimensions = new Vector2(transform.localScale.x, transform.localScale.y);
            normal = transform.forward;
        }

        // ======== [[ TRANSFORM UTILITIES ]] ================================== >>>>
        public static void SetTransformToDefaultValues(Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        public static void SetTransformValues(Transform transform, Vector3 position)
        {
            transform.position = position;
        }

        public static void SetTransformValues(Transform transform, Vector3 position, Vector2 dimensions)
        {
            transform.position = position;
            transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
        }

        public static void SetTransformValues(Transform transform, Vector3 position, Vector2 dimensions, Vector3 normal)
        {
            transform.position = position;
            transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
            transform.rotation = Quaternion.LookRotation(normal, Vector3.up);
        }

        public static void SetTransformScale_ToDimensions(Transform transform, Vector2 dimensions)
        {
            transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
        }

        public static void SetTransformScale_ToSquare(Transform transform, float size)
        {
            transform.localScale = new Vector3(size, size, 1);
        }
    }
}
