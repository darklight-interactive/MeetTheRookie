using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.BuildScene;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class MTRSceneData : BuildSceneData
{
    // private access to knots for dropdown
    private List<string> _knotNames
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }
    [SerializeField, ShowOnly] private string _savedKnotData = "default";

    [Dropdown("_knotNames")]
    public string knot;


    public MTRSceneData() : base("") { }
    public MTRSceneData(string path) : base(path)
    {
        _savedKnotData = knot;
    }
}