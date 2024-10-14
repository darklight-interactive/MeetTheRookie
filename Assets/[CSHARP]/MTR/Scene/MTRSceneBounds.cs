using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRSceneBounds
{
    [SerializeField] float _centerX = 0;
    [SerializeField] Vector2 _xAxisBounds = new Vector2(-10, 10);

    public Vector3 Center { get => Vector3.right * _centerX; }
    public float Left { get => _xAxisBounds.x; set => _xAxisBounds.x = value; }
    public float Right { get => _xAxisBounds.y; set => _xAxisBounds.y = value; }

    public MTRSceneBounds() { }
    public MTRSceneBounds(MTRSceneBounds bounds)
    {
        _centerX = bounds.Center.x;
        _xAxisBounds = bounds._xAxisBounds;
    }

    public void DrawGizmos()
    {
        Handles.color = Color.white;
        CustomGizmos.DrawLabel("SceneCenter", Center, new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = Color.white }
        });

        Vector3 leftBound = Center + new Vector3(Left, 0, 0);
        Vector3 rightBound = Center + new Vector3(Right, 0, 0);

        // X Bounds
        Handles.DrawLine(leftBound, leftBound + new Vector3(0, 5, 0));
        Handles.DrawLine(rightBound, rightBound + new Vector3(0, 5, 0));
    }
}
