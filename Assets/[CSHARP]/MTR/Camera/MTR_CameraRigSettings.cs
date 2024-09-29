using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "MeetTheRookie/MTRCameraPreset")]
public class MTR_CameraRigSettings : ScriptableObject
{
    const int DEFAULT_RANGE_VALUE = 25;
    static Vector2 DistRange = new Vector2(-DEFAULT_RANGE_VALUE * 2, 0);
    static Vector2 HorzRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 VertRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 SpeedRange = new Vector2(0, 5);


    [Header("Speed")]
    [DynamicRange("SpeedRange")] public float positionLerpSpeed = 10f;
    [DynamicRange("SpeedRange")] public float rotationLerpSpeed = 10f;
    [DynamicRange("SpeedRange")] public float fovLerpSpeed = 10f;

    [Header("Depth")]
    [Tooltip("The depth of the camera from the origin target. This is the Z axis.")]
    [DynamicRange("DistRange")] public float distanceOffset = -10f;
    [Header("Position Offsets")]
    [Tooltip("The horizontal offset of the camera from the origin target. This is the X axis.")]
    [DynamicRange("HorzRange")] public float horizontalOffset = 0f;
    [Tooltip("The vertical offset of the camera from the origin target. This is the Y axis.")]
    [DynamicRange("VertRange")] public float verticalOffset = 0f;

    [Header("Field of View")]
    [SerializeField, Range(0.1f, 190)] public float fov = 5f;
}