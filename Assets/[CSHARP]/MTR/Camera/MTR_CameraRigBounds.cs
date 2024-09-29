using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/MTR_CameraRigBounds")]
public class MTR_CameraRigBounds : ScriptableObject
{
    const int DEFAULT_RANGE_VALUE = 20;
    static Vector2 BOUND_RANGE = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 BOUND_HALF_RANGE = new Vector2(-DEFAULT_RANGE_VALUE / 2, DEFAULT_RANGE_VALUE / 2);

    [Header("Bounds")]
    public SingleAxisBounds xAxisBounds = new SingleAxisBounds(Axis.X, BOUND_RANGE);
    public SingleAxisBounds yAxisBounds = new SingleAxisBounds(Axis.Y, BOUND_RANGE);

    public void DrawBounds(Vector3 center)
    {
        // Draw X Axis Bounds
        Gizmos.color = Color.red;
        xAxisBounds.DrawGizmos(center, yAxisBounds.Max);

        // Draw Y Axis Bounds
        Gizmos.color = Color.green;
        yAxisBounds.DrawGizmos(center + new Vector3(xAxisBounds.Min, 0, 0), xAxisBounds.Distance);

    }
}