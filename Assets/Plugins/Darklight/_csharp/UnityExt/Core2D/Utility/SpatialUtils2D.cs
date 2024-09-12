using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public static class SpatialUtils2D
    {
        // Enum to specify origin points
        public enum OriginPoint
        {
            TOP_LEFT,
            TOP_CENTER,
            TOP_RIGHT,
            CENTER_LEFT,
            CENTER,
            CENTER_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_CENTER,
            BOTTOM_RIGHT
        }

        /// <summary>
        /// Calculates a 2D object's center position based on a given spawn position and origin point.
        /// </summary>
        /// <param name="spawnPosition">The actual spawn position of the object.</param>
        /// <param name="dimensions">The dimensions of the cell.</param>
        /// <param name="originTag">The origin point that was used for spawning.</param>
        /// <returns>Original center position of the cell.</returns>
        public static Vector3 CalculatePositionWithOriginOffset(Vector3 spawnPosition, Vector2 dimensions, OriginPoint originTag)
        {
            Vector3 offset = Vector3.zero;

            switch (originTag)
            {
                case OriginPoint.TOP_LEFT:
                    offset = new Vector3(dimensions.x / 2, -dimensions.y / 2, 0);
                    break;
                case OriginPoint.TOP_CENTER:
                    offset = new Vector3(0, -dimensions.y / 2, 0);
                    break;
                case OriginPoint.TOP_RIGHT:
                    offset = new Vector3(-dimensions.x / 2, -dimensions.y / 2, 0);
                    break;
                case OriginPoint.CENTER_LEFT:
                    offset = new Vector3(dimensions.x / 2, 0, 0);
                    break;
                case OriginPoint.CENTER:
                    // Center does not require an offset
                    offset = Vector3.zero;
                    break;
                case OriginPoint.CENTER_RIGHT:
                    offset = new Vector3(-dimensions.x / 2, 0, 0);
                    break;
                case OriginPoint.BOTTOM_LEFT:
                    offset = new Vector3(dimensions.x / 2, dimensions.y / 2, 0);
                    break;
                case OriginPoint.BOTTOM_CENTER:
                    offset = new Vector3(0, dimensions.y / 2, 0);
                    break;
                case OriginPoint.BOTTOM_RIGHT:
                    offset = new Vector3(-dimensions.x / 2, dimensions.y / 2, 0);
                    break;
            }

            return spawnPosition + offset;
        }

    }
}
