using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Darklight.UnityExt.Core2D.Spatial2D;

namespace Darklight.UnityExt.Core2D
{
    /// <summary>
    /// Interface for 2D Spatial Objects.
    /// </summary>
    public interface ISpatial2D
    {
        Vector3 Position { get; }
        Vector2 Dimensions { get; }
        Vector3 Normal { get; }
    }

    /// <summary>
    /// Custom Static Utility Class for 2D Spatial Calculations.
    /// </summary>
    public static class Spatial2D
    {
        public enum AnchorPoint
        {
            TOP_LEFT, TOP_CENTER, TOP_RIGHT,
            CENTER_LEFT, CENTER, CENTER_RIGHT,
            BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT
        }

        public static readonly Dictionary<AnchorPoint, Vector2> anchorPointOffsets = new Dictionary<AnchorPoint, Vector2>
        {
            { AnchorPoint.TOP_LEFT, new Vector2(-0.5f, 0.5f) },
            { AnchorPoint.TOP_CENTER, new Vector2(0, 0.5f) },
            { AnchorPoint.TOP_RIGHT, new Vector2(0.5f, 0.5f) },
            { AnchorPoint.CENTER_LEFT, new Vector2(-0.5f, 0) },
            { AnchorPoint.CENTER, new Vector2(0, 0) },
            { AnchorPoint.CENTER_RIGHT, new Vector2(0.5f, 0) },
            { AnchorPoint.BOTTOM_LEFT, new Vector2(-0.5f, -0.5f) },
            { AnchorPoint.BOTTOM_CENTER, new Vector2(0, -0.5f) },
            { AnchorPoint.BOTTOM_RIGHT, new Vector2(0.5f, -0.5f) }
        };

        /// <summary>
        /// Calculates the position offset of the anchor point.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="anchorTag"></param>
        /// <returns></returns>
        public static Vector3 CalculateAnchorPointOffset(Vector2 dimensions, AnchorPoint anchorTag)
        {
            return dimensions * anchorPointOffsets[anchorTag];
        }

        /// <summary>
        /// Calculates the worlf position of the anchor point based on a given center.
        /// </summary>
        /// <param name="center">
        ///     The center position of the object.
        /// </param>
        /// <param name="dimensions">
        ///     The dimensions of the object.
        /// </param>
        /// <param name="anchorTag"></param>
        /// <returns></returns>
        static Vector3 CalculateAnchorPointPosition(Vector3 center, Vector2 dimensions, AnchorPoint anchorTag)
        {
            return center + CalculateAnchorPointOffset(dimensions, anchorTag);
        }

        // ======== [[ TRANSFORM UTILITIES ]] ================================== >>>>
        // ---- (( HANDLERS )) ---- >>


        // ---- (( GETTERS )) ---- >>
        public static Vector3 GetAnchorPointOffset(Vector2 dimensions, AnchorPoint anchorTag)
        {
            return CalculateAnchorPointOffset(dimensions, anchorTag);
        }

        public static Vector3 GetAnchorPointPosition(Vector3 center, Vector2 dimensions, AnchorPoint anchorTag)
        {
            return CalculateAnchorPointPosition(center, dimensions, anchorTag);
        }


        // ---- (( SETTERS )) ---- >>
        public static void SetTransformToDefaultValues(Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
        }

        public static void SetTransformValues(Transform transform, Vector3 position, Vector2 dimensions, Vector3 normal)
        {
            transform.position = position;
            SetTransformScale_ToDimensions(transform, dimensions);
            SetTransformRotation_ToNormal(transform, normal);
        }

        public static void SetTransformPos_ToAnchor(Transform transform, Vector3 position, Vector2 dimensions, AnchorPoint anchorTag)
        {
            Vector3 positionOffset = CalculateAnchorPointOffset(dimensions, anchorTag);
            transform.position = position - positionOffset;
        }

        public static void SetTransformScale_ToDimensions(Transform transform, Vector2 dimensions)
        {
            transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
        }

        public static void SetTransformScale_ToSquareRatio(Transform transform, float size)
        {
            transform.localScale = new Vector3(size, size, 1);
        }

        public static void SetTransformRotation_ToNormal(Transform transform, Vector3 normal)
        {
            transform.localRotation = Quaternion.LookRotation(normal, Vector3.up);
        }
    }
}
