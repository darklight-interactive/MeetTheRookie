using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using Darklight.UnityExt.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTR_SceneDataObject : BuildSceneDataObject<MTR_SceneData>
{
    public override void Initialize(string[] buildScenePaths)
    {
        for (int i = 0; i < buildScenePaths.Length; i++)
        {
            string scenePath = buildScenePaths[i];

            // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
            if (this.buildSceneData.Count <= i || this.buildSceneData[i].Path != scenePath)
            {
                MTR_SceneData sceneData = new MTR_SceneData();
                this.buildSceneData.Add(sceneData);
            }

            this.buildSceneData[i].InitializeData(scenePath);
        }
        Debug.Log("MTR_SceneDataObject Initialized");

    }

    public List<MTR_SceneData> GetBuildSceneData()
    {
        return buildSceneData.ToList();
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetBuildSceneData().Find(x => x.knot == knot);
    }
}