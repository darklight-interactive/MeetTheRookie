using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRSceneBounds
{
    const float X_BOUND_RANGE = 25f;
    const float Y_BOUND_RANGE = 5f;

    [SerializeField]
    Vector2 _center = new Vector2(0, 1);

    [Header("X Axis Bounds")]
    [SerializeField, Range(-X_BOUND_RANGE, 0)]
    float _minX;

    [SerializeField, Range(0, X_BOUND_RANGE)]
    float _maxX;

    [Header("Y Axis Bounds")]
    [SerializeField, Range(-Y_BOUND_RANGE, 0)]
    float _minY;

    [SerializeField, Range(0, Y_BOUND_RANGE)]
    float _maxY;

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
    public Vector2 YAxisValues
    {
        get => new Vector2(_minY, _maxY);
        set
        {
            _minY = value.x;
            _maxY = value.y;
        }
    }
    public float Left
    {
        get => _center.x + _minX;
    }
    public float Right
    {
        get => _center.x + _maxX;
    }
    public float Top
    {
        get => _center.y + _maxY;
    }
    public float Bottom
    {
        get => _center.y + _minY;
    }

    public bool ContainsXPos(float xValue)
    {
        return xValue > Left && xValue < Right;
    }

    public bool ContainsYPos(float yValue)
    {
        return yValue > Bottom && yValue < Top;
    }

    public void DrawGizmos()
    {
        CustomGizmos.DrawLabel(
            "SceneCenter",
            Center,
            new GUIStyle() { normal = new GUIStyleState() { textColor = Color.white } }
        );

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(Left, Top, 0), new Vector3(Left, Bottom, 0));
        Gizmos.DrawLine(new Vector3(Right, Top, 0), new Vector3(Right, Bottom, 0));
        Gizmos.DrawLine(new Vector3(Left, Top, 0), new Vector3(Right, Top, 0));
        Gizmos.DrawLine(new Vector3(Left, Bottom, 0), new Vector3(Right, Bottom, 0));
    }
}
