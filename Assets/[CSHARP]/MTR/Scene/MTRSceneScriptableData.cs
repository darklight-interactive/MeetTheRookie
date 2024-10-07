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
    List<string> _knotList = new List<string> { "Default" };
    List<string> KnotList
    {
        get
        {
            if (InkyStoryManager.Instance.KnotList != null)
                _knotList = InkyStoryManager.Instance.KnotList;
            return _knotList;
        }
    }
    [SerializeField, Dropdown("KnotList")] string _knot = "Default";

    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;

    public MTRCameraRigBounds CameraRigBounds { get => _cameraRigBounds; set => _cameraRigBounds = value; }
}

