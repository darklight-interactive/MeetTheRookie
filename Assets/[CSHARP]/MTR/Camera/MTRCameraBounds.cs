using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRCameraBounds
{
    [ShowOnly]
    public Vector2 Center = new Vector2(0, 1);

    [ShowOnly]
    public Vector2 XAxisValues = new Vector2(-5, 5);

    [ShowOnly]
    public Vector2 YAxisValues = new Vector2(-1, 1);

    public float Left
    {
        get => Center.x + XAxisValues.x;
    }
    public float Right
    {
        get => Center.x + XAxisValues.y;
    }
    public float Top
    {
        get => Center.y + YAxisValues.y;
    }
    public float Bottom
    {
        get => Center.y + YAxisValues.x;
    }

    public void SetBounds(Vector2 center, Vector2 xAxisBounds, Vector2 yAxisBounds)
    {
        Center = center;
        XAxisValues = xAxisBounds;
        YAxisValues = yAxisBounds;
    }

    public void DrawGizmos()
    {
        Handles.color = Color.cyan;
        Handles.DrawLine(new Vector3(Left, Top, 0), new Vector3(Left, Bottom, 0));
        Handles.DrawLine(new Vector3(Right, Top, 0), new Vector3(Right, Bottom, 0));
        Handles.DrawLine(new Vector3(Left, Top, 0), new Vector3(Right, Top, 0));
        Handles.DrawLine(new Vector3(Left, Bottom, 0), new Vector3(Right, Bottom, 0));
    }
}
