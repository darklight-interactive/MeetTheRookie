using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.BuildScene;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

[Serializable]
public class MTRSceneData : BuildSceneData
{
    bool _foundSceneKnot;

    [Header("MTR STORY SETTINGS")]
    [SerializeField, ShowOnly] string _sceneKnot = "None";
    [SerializeField, ShowOnly] string _onEnterStitch = "None";

    [Header("MTR SCENE SETTINGS")]
    [SerializeField]
    MTRSceneBounds _sceneBounds = new MTRSceneBounds()
    {
        DisableEdit = true
    };

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, ShowOnly] MTRCameraRigSettings _cameraRigSettings;
    [SerializeField, ShowOnly] MTRCameraRigBounds _cameraRigBounds;

    public string SceneKnot { get => _sceneKnot; set => _sceneKnot = value; }
    public string OnEnterStitch { get => _onEnterStitch; set => _onEnterStitch = value; }
    public MTRSceneBounds SceneBounds { get => _sceneBounds; set => _sceneBounds = value; }
    public MTRCameraRigSettings CameraRigSettings { get => _cameraRigSettings; set => _cameraRigSettings = value; }
    public MTRCameraRigBounds CameraRigBounds { get => _cameraRigBounds; set => _cameraRigBounds = value; }

    public MTRSceneData() : base() { }

    public void DrawGizmos()
    {
        SceneBounds.DrawGizmos();
    }
}


