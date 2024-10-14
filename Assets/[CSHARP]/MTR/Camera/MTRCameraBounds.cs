using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class MTRCameraBounds
{
    [ReadOnly, AllowNesting] public Vector2 center = Vector2.zero;
    public SingleAxisBounds xAxisBounds = new SingleAxisBounds(Axis.X, new Vector2(-20, 20));
    public SingleAxisBounds yAxisBounds = new SingleAxisBounds(Axis.Y, new Vector2(0, 2));

    public void DrawGizmos()
    {
        // Draw X Axis Bounds
        Gizmos.color = Color.red;
        xAxisBounds.DrawGizmos(center, yAxisBounds.Max);

        // Draw Y Axis Bounds
        Gizmos.color = Color.green;
        yAxisBounds.DrawGizmos((Vector3)center + new Vector3(xAxisBounds.Min, 0, 0), xAxisBounds.Distance);
    }
}