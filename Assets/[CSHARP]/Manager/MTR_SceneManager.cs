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

    MTR_SceneDataObject mtr_SceneDataObject;

    public override void Initialize()
    {
#if UNITY_EDITOR
            mtr_SceneDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTR_SceneDataObject>(
                DATA_PATH,
                DATA_FILENAME
            );

            if (mtr_SceneDataObject == null)
            {
                Debug.LogError($"{this.name} Failed to create or load build scene data object.");
                return;
            }
        else
        {
            Debug.Log($"{this.name} Build Scene Data Object loaded successfully. {mtr_SceneDataObject}");
        }

        base.LoadBuildScenes();
#endif

        mtr_SceneDataObject.Initialize(buildScenePaths);

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
        List<MTR_SceneData> buildSceneData = mtr_SceneDataObject.GetData();

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
            buildSceneDataObject.SaveSceneData(buildSceneData[i]);
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
        MTR_SceneData data = mtr_SceneDataObject.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        return true;
    }

    public MTR_SceneData GetSceneData(Scene scene)
    {
        string sceneName = scene.name;
        return mtr_SceneDataObject.buildSceneData.ToList().Find(x => x.Name == sceneName);
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return this.mtr_SceneDataObject.GetSceneDataByKnot(knot);
    }

    public MTR_SceneData GetActiveSceneData()
    {
        if (mtr_SceneDataObject == null)
            return new MTR_SceneData();
        return mtr_SceneDataObject.GetActiveSceneData();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTR_SceneManager))]
public class MTR_SceneManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MTR_SceneManager _script;

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTR_SceneManager)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Initialize"))
        {
            _script.Initialize();
        }

        if (GUILayout.Button("Show Editor Window"))
        {
            BuildSceneManagerWindow.ShowWindow();
        }

        // Display the active scene name.
        MTR_SceneData activeScene = _script.GetActiveSceneData();
        if (activeScene != null)
        {
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Scene", activeScene.Name);
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Knot", activeScene.knot);
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}


#endif
