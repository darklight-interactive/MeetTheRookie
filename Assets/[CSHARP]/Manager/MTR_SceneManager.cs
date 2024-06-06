using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.SceneManagement;
using FMODUnity;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
public class MTR_SceneManager : BuildSceneDataManager<MTR_SceneData>
{
    public MTR_SceneDataObject sceneDataObject;

    public override void Initialize()
    {
#if UNITY_EDITOR
        sceneDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTR_SceneDataObject>(
            DATA_PATH,
                DATA_FILENAME
            );

        if (sceneDataObject == null)
        {
            Debug.LogError($"{this.name} Failed to create or load build scene data object.");
            return;
        }
        else
        {
            Debug.Log($"{this.name} Build Scene Data Object loaded successfully. {sceneDataObject}");
        }

        base.LoadBuildScenes();
#endif

        sceneDataObject.Initialize(buildScenePaths);

        //SaveBuildSceneData(buildScenePaths);
        InkyStoryManager.Instance.OnStoryInitialized += OnStoryInitialized;
    }

    /// <summary>
    /// Saves the build scene data by updating the paths of the BuildSceneData objects
    /// based on the paths in the EditorBuildSettingsScene array.
    /// </summary>
    public override void SaveBuildSceneData(string[] buildScenePaths)
    {
        this.buildScenePaths = buildScenePaths;
        List<MTR_SceneData> buildSceneData = sceneDataObject.GetAllData();

        for (int i = 0; i < buildScenePaths.Length; i++)
        {
            string scenePath = buildScenePaths[i];

            // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
            if (buildSceneData.Count <= i || buildSceneData[i].Path != scenePath)
            {
                buildSceneData.Add(new MTR_SceneData());
                Debug.Log($"{this.name} -> Added new MTR_SceneData object.");
            }

            // Initialize the scene data.
            buildSceneData[i].InitializeData(scenePath);
            //mtr_SceneDataObject.SaveSceneData(buildSceneData[i]);
        }


        EditorUtility.SetDirty(this);
        Debug.Log($"{this.name} Saved build scene data.");
    }

    public void OnStoryInitialized(Story story)
    {
        Debug.Log($"{Prefix} >> STORY INITIALIZED EVENT: {story}");
        story.BindExternalFunction(
            "ChangeGameScene",
            (string knotName) => ChangeGameScene(knotName)
        );
    }

    /// <summary>
    /// This is the main external function that is called from the Ink story to change the game scene.
    /// </summary>
    /// <param name="args">0 : The name of the sceneKnot</param>
    /// <returns>False if BuildSceneData is null. True if BuildSceneData is valid.</returns>
    object ChangeGameScene(string knotName)
    {
        MTR_SceneData data = sceneDataObject.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        return true;
    }

    public MTR_SceneData GetSceneData(string name)
    {
        return sceneDataObject.GetSceneData(name);
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return this.sceneDataObject.GetSceneDataByKnot(knot);
    }

    public MTR_SceneData GetActiveSceneData()
    {
        if (sceneDataObject == null)
            return new MTR_SceneData();
        return sceneDataObject.GetActiveSceneData();
    }
}

