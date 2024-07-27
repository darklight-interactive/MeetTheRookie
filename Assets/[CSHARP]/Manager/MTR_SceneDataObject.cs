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
    public void Initialize(string[] buildScenePaths)
    {
        this.buildScenePaths = buildScenePaths;
        Initialize();
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