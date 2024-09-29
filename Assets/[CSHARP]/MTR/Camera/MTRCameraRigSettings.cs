using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "MeetTheRookie/MTRCameraPreset")]
public class MTRCameraRigSettings : ScriptableObject
{
    const int DEFAULT_RANGE_VALUE = 25;
    static Vector2 DistRange = new Vector2(-DEFAULT_RANGE_VALUE * 2, 0);
    static Vector2 HorzRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 VertRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 SpeedRange = new Vector2(0, 5);
    readonly Vector2 rotationRange = new Vector2(-45, 45);


    [Header("Speed")]
    [DynamicRange("SpeedRange")] public float positionLerpSpeed = 10f;
    [DynamicRange("SpeedRange")] public float rotationLerpSpeed = 10f;
    [DynamicRange("SpeedRange")] public float fovLerpSpeed = 10f;

    [Header("Position")]
    [DynamicRange("HorzRange")] public float xPosOffset = 0f;
    [DynamicRange("VertRange")] public float yPosOffset = 0f;
    [DynamicRange("DistRange")] public float zPosOffset = -10f;

    [Header("Rotation")]
    [DynamicRange("rotationRange")] public float xRotOffset = 0f;


    [Header("Field of View")]
    [SerializeField, Range(0.1f, 190)] public float fov = 5f;
}