using System;
using Darklight.UnityExt.Editor;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "MeetTheRookie/MTRCameraPreset")]
public class MTRCameraSettingPreset : ScriptableObject
{
    const int DEFAULT_RANGE_VALUE = 25;
    static Vector2 DistRange = new Vector2(-DEFAULT_RANGE_VALUE * 2, 0);
    static Vector2 HorzRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    static Vector2 VertRange = new Vector2(-DEFAULT_RANGE_VALUE, DEFAULT_RANGE_VALUE);
    readonly Vector2 rotationRange = new Vector2(-45, 45);

    [Header("Position")]
    [DynamicRange("HorzRange")] public float xPosOffset = 0f;
    [DynamicRange("VertRange")] public float yPosOffset = 0f;
    [DynamicRange("DistRange")] public float zPosOffset = -10f;

    [Header("Rotation")]
    public bool lookAtFollowTarget;
    [DynamicRange("rotationRange"), ShowIf("lookAtFollowTarget")] public float orbitRotation = 0f;


    [Header("Field of View")]
    [SerializeField, Range(0.1f, 190)] public float fov = 5f;
}