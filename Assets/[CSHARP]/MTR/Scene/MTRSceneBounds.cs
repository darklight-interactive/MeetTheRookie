using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRSceneBounds
{
    const float BOUND_RANGE = 25f;
    const float SCENE_HEIGHT = 2f;
    [SerializeField] Vector2 _center = new Vector2(0, 1);
    [Header("X Axis Bounds")]
    [SerializeField, Range(-BOUND_RANGE, 0)] float _minX;
    [SerializeField, Range(0, BOUND_RANGE)] float _maxX;

    public Vector2 Center => _center;
    public Vector2 XAxisValues
    {
        get => new Vector2(_minX, _maxX);
        set
        {
            _minX = value.x;
            _maxX = value.y;
        }
    }
    public float Left { get => _center.x + _minX; }
    public float Right { get => _center.x + _maxX; }

    public bool Contains(float xValue)
    {
        return xValue > Left && xValue < Right;
    }

    public void DrawGizmos()
    {
        CustomGizmos.DrawLabel("SceneCenter", Center, new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = Color.white }
        });

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(Left, SCENE_HEIGHT, 0), new Vector3(Left, 0, 0));
        Gizmos.DrawLine(new Vector3(Right, SCENE_HEIGHT, 0), new Vector3(Right, 0, 0));
    }
}
