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

    void GetAxisVector(out Vector3 direction)
    {
        direction = Vector3.zero;
        switch (_axis)
        {
            case Axis.X:
                direction = Vector3.right;
                break;
            case Axis.Y:
                direction = Vector3.up;
                break;
            case Axis.Z:
                direction = Vector3.forward;
                break;
        }
    }

    void GetAxisColor(out Color color)
    {
        color = Color.white;
        switch (_axis)
        {
            case Axis.X:
                color = Color.red;
                break;
            case Axis.Y:
                color = Color.green;
                break;
            case Axis.Z:
                color = Color.blue;
                break;
        }
    }

    public void GetWorldValues(Vector3 center, out float min, out float max)
    {
        min = center.x;
        max = center.x;

        switch (_axis)
        {
            case Axis.X:
                min = center.x + Min;
                max = center.x + Max;
                break;
            case Axis.Y:
                min = center.y + Min;
                max = center.y + Max;
                break;
            case Axis.Z:
                min = center.z + Min;
                max = center.z + Max;
                break;
        }
    }

    public void DrawGizmos(Vector3 origin, float length)
    {
        GetAxisVector(out Vector3 direction);
        GetAxisColor(out Color color);

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
}