using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.SceneManagement;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class MTR_SceneData : BuildSceneData
{
    private InkyStoryObject _globalStoryObject;
    private List<string> _knotNames = new List<string> { "default" };

    [SerializeField, ShowOnly] private string _savedKnotData = "default";

    [Dropdown("_knotNames")]
    public string knot;
    public EventReference backgroundMusicEvent;

    public override void InitializeData(string path)
    {
        base.InitializeData(path);

        if (InkyStoryManager.Instance != null)
        {
            _globalStoryObject = InkyStoryManager.GlobalStoryObject;
            _knotNames = _globalStoryObject.KnotNameList;
            _savedKnotData = knot;
        }
    }
}