using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Darklight.UnityExt.Editor;
using UnityEngine;
public enum Axis { X, Y, Z }

[Serializable]
public class SingleAxisBounds
{
    readonly Vector2 valueRange;

    [SerializeField, ShowOnly] Axis _axis;
    [SerializeField, DynamicRange("valueRange")] float _min;
    [SerializeField, DynamicRange("valueRange")] float _max;

    public Axis Axis { get => _axis; set => _axis = value; }
    public float Min
    {
        get
        {
            return _min = Mathf.Clamp(_min, valueRange.x, valueRange.y);
        }
        set
        {
            _min = Mathf.Clamp(value, valueRange.x, valueRange.y);
        }
    }
    public float Max
    {
        get
        {
            return _max = Mathf.Clamp(_max, valueRange.x, valueRange.y);
        }
        set
        {
            _max = Mathf.Clamp(value, valueRange.x, valueRange.y);
        }
    }
    public float Distance => Mathf.Abs(Max - Min);
    public SingleAxisBounds(Axis axis, Vector2 range)
    {
        _axis = axis;
        valueRange = range;
        Min = range.x;
        Max = range.y;
    }

    public void GetBoundWorldPositions(Vector3 origin, out Vector3 min, out Vector3 max)
    {
        GetAxisValues(out Vector3 direction, out Color color);

        min = origin + direction * Min;
        max = origin + direction * Max;
    }

    public void DrawGizmos(Vector3 origin, float length)
    {
        GetAxisValues(out Vector3 direction, out Color color);
        Gizmos.color = color;

        // Calculate min and max points along the current axis
        Vector3 minPoint = origin + direction * Min;
        Vector3 maxPoint = origin + direction * Max;

        // Determine the perpendicular vector based on the current axis
        Vector3 perpendicular = Vector3.zero;

        if (_axis == Axis.X)
            perpendicular = Vector3.up * length; // Move up for X axis
        else if (_axis == Axis.Y)
            perpendicular = Vector3.right * length; // Move right for Y axis
        else if (_axis == Axis.Z)
            perpendicular = Vector3.up * length; // Move up for Z axis

        // Draw the rays for the min and max bounds
        Gizmos.DrawRay(minPoint, perpendicular);
        Gizmos.DrawRay(maxPoint, perpendicular);
    }

    void GetAxisValues(out Vector3 axisVector, out Color axisColor)
    {
        // ( Vector ) ---------------------
        switch (this.Axis)
        {
            case Axis.X:
                axisVector = Vector3.right;
                axisColor = Color.red;
                break;
            case Axis.Y:
                axisVector = Vector3.up;
                axisColor = Color.green;
                break;
            case Axis.Z:
                axisVector = Vector3.forward;
                axisColor = Color.blue;
                break;
            default:
                axisVector = Vector3.zero;
                axisColor = Color.white;
                break;
        }
    }

}