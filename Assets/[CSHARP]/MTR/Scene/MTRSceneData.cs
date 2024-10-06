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
    [SerializeField, ShowOnly] string _knot;
    public string Knot { get => _knot; set => _knot = value; }
}