using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRSceneBounds
{
    bool _disableEdit = false;

    [SerializeField, DisableIf("_disableEdit"), AllowNesting] Vector2 _center = Vector2.zero;
    [SerializeField, DisableIf("_disableEdit"), AllowNesting] Vector2 _xAxisBounds = new Vector2(-10, 10);
    [SerializeField, DisableIf("_disableEdit"), AllowNesting] Vector2 _yAxisBounds = new Vector2(0, 2);

    public bool DisableEdit { get => _disableEdit; set => _disableEdit = value; }
    public Vector3 Center { get => _center; set => _center = value; }
    public float Left { get => _xAxisBounds.x; set => _xAxisBounds.x = value; }
    public float Right { get => _xAxisBounds.y; set => _xAxisBounds.y = value; }
    public float Top { get => _yAxisBounds.y; set => _yAxisBounds.y = value; }
    public float Bottom { get => _yAxisBounds.x; set => _yAxisBounds.x = value; }

    public MTRSceneBounds() { }
    public MTRSceneBounds(MTRSceneBounds bounds, bool disableEdit)
    {
        _center = bounds.Center;
        _xAxisBounds = bounds._xAxisBounds;
        _yAxisBounds = bounds._yAxisBounds;
        _disableEdit = disableEdit;
    }

    public void DrawGizmos()
    {
        Handles.color = Color.white;

        Vector3 leftBound = Center + new Vector3(Left, 0, 0);
        Vector3 rightBound = Center + new Vector3(Right, 0, 0);
        Vector3 topBound = Center + new Vector3(0, Top, 0);
        Vector3 bottomBound = Center + new Vector3(0, Bottom, 0);

        // X Bounds
        Handles.DrawLine(leftBound + new Vector3(0, Top, 0), leftBound + new Vector3(0, Bottom, 0));
        Handles.DrawLine(rightBound + new Vector3(0, Top, 0), rightBound + new Vector3(0, Bottom, 0));

        // Y Bounds
        Handles.DrawLine(leftBound + new Vector3(0, Top, 0), rightBound + new Vector3(0, Top, 0));
        Handles.DrawLine(leftBound + new Vector3(0, Bottom, 0), rightBound + new Vector3(0, Bottom, 0));

    }
}
