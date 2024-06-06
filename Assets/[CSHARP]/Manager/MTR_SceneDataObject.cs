using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTR_SceneDataObject : ScriptableObject
{
    public MTR_SceneData[] buildSceneData = new MTR_SceneData[0];
    public void Initialize(string[] buildScenePaths)
    {
        for (int i = 0; i < buildScenePaths.Length; i++)
        {
            string scenePath = buildScenePaths[i];

            // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
            if (this.buildSceneData.Length <= i || this.buildSceneData[i].Path != scenePath)
            {
                MTR_SceneData sceneData = new MTR_SceneData();
                this.buildSceneData.ToList().Add(sceneData);
            }

            this.buildSceneData[i].InitializeData(scenePath);
        }
        Debug.Log("MTR_SceneDataObject Initialized");

    }

    public List<MTR_SceneData> GetData()
    {
        return buildSceneData.ToList();
    }

    public MTR_SceneData GetActiveSceneData()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return buildSceneData.ToList().Find(x => x.Name == activeSceneName);
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetData().Find(x => x.knot == knot);
    }

    public EventReference GetActiveBackgroundMusicEvent()
    {
        MTR_SceneData data = GetActiveSceneData();
        return data.backgroundMusicEvent;
    }
}