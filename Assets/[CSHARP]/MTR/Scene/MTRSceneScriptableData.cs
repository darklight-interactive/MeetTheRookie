using System.Collections.Generic;
using System.Linq;

using Darklight.UnityExt.BuildScene;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;

using EasyButtons;

using FMODUnity;

using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;



#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTRSceneScriptableData : BuildSceneScriptableData<MTRSceneData>
{
    List<string> _knotList => InkyStoryManager.KnotList;
    [SerializeField, Dropdown("_knotList")] string _knot = "Default";

    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;
}

