using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.BuildScene;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class MTRSceneData : BuildSceneData
{
    bool _foundSceneKnot;

    [ShowOnly] public string SceneKnot;
    [ShowOnly] public string OnStartInteractionStitch;

    public MTRSceneData() : base() { }
    public override void Refresh()
    {
        base.Refresh();
    }


}


