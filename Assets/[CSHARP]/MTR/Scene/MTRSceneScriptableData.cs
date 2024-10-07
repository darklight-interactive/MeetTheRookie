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
    string _savedKnot;
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

    [Header("MTR STORY SETTINGS")]
    [SerializeField, Dropdown("KnotList")] string _knot = "Default";

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, Expandable] MTRCameraRigSettings _cameraRigSettings;
    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;

    [Header("MTR GAMEPLAY SETTINGS")]
    [SerializeField] List<int> _spawnPoints = new List<int>();

    public MTRCameraRigBounds CameraRigBounds { get => _cameraRigBounds; set => _cameraRigBounds = value; }

    void OnValidate()
    {
        if (_knot != _savedKnot)
        {
            _savedKnot = _knot;
        }
    }

    public override MTRSceneData ToData()
    {
        return new MTRSceneData()
        {
            Path = this.Path,
            Knot = _knot,
        };
    }
}

