using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "MeetTheRookie/MTRCameraPreset")]
public class MTR_CameraRigPreset : ScriptableObject
{
    const int DEFAULT_RANGE = 25;
    static Vector2 HorzRange = new Vector2(-DEFAULT_RANGE, DEFAULT_RANGE);
    static Vector2 VertRange = new Vector2(0, DEFAULT_RANGE);
    static Vector2 DepthRange = new Vector2(-DEFAULT_RANGE, 0);

    [Header("Bounds")]
    public bool useBounds = true;
    public SingleAxisBounds xAxisBounds = new SingleAxisBounds(Axis.X, HorzRange);
    public SingleAxisBounds yAxisBounds = new SingleAxisBounds(Axis.Y, VertRange);
    public SingleAxisBounds zAxisBounds = new SingleAxisBounds(Axis.Z, DepthRange);

    [Header("Speed")]
    [Range(0, 10)] public float lerpSpeed = 10f;

    [Header("Depth")]
    [Tooltip("The depth be")]
    [DynamicRange("DepthRange")] public float depth = 50f;

    [Header("Position Offsets")]
    [DynamicRange("HorzRange")] public float horizontalOffset = 0f;
    [DynamicRange("VertRange")] public float verticalOffset = 0f;

    [Header("Field of View")]
    [SerializeField, Range(0.1f, 190)] public float fov = 5f;

    void OnValidate()
    {
        xAxisBounds.Axis = Axis.X;
        yAxisBounds.Axis = Axis.Y;
        zAxisBounds.Axis = Axis.Z;
    }
}