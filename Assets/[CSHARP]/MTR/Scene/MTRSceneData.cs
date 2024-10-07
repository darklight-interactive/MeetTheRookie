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

    [ShowOnly] public string Knot;
    [ShowOnly] public MTRCameraRigSettings CameraRigSettings;
    [ShowOnly] public MTRCameraRigBounds CameraRigBounds;
    public MTRSceneSpawnInfo SpawnInfo;

    public bool FoundSceneKnot => _foundSceneKnot;

    public MTRSceneData() : base()
    {
        TrySetSceneKnot();
    }

    public override void Refresh()
    {
        base.Refresh();

        if (!_foundSceneKnot)
            TrySetSceneKnot();
    }


    void TrySetSceneKnot()
    {
        // << GET SCENE KNOT LIST >>
        List<string> sceneKnotList = InkyStoryManager.Instance.SceneKnotList;
        if (sceneKnotList == null || sceneKnotList.Count == 0)
            return;

        // << PARSE SCENE NAME >>
        string sceneName = Name.ToLower();
        sceneName = sceneName.Replace(" ", ""); // Get the scene name and remove spaces
        sceneName = sceneName.Replace("-", "_"); // Replace hyphens with underscores

        // << FIND RLEATED KNOT >>
        List<string> sceneNameParts = sceneName.Split('_').ToList();
        if (sceneNameParts.Contains("scene"))
        {
            string sceneIndex = sceneNameParts[1];
            string sectionIndex = sceneNameParts[2];

            // Check if the scene knot exists
            if (sceneKnotList.Contains($"scene{sceneIndex}_{sectionIndex}"))
            {
                Knot = $"scene{sceneIndex}_{sectionIndex}";
                _foundSceneKnot = true;

                Debug.Log($"< MTRSceneData > >> Found SceneKnot for {Name} >> ({Knot})");
                return;
            }
        }
    }
}


