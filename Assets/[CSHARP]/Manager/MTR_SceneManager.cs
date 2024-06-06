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
        //SaveBuildSceneData(buildScenePaths);
#endif
        InkyStoryManager.Instance.OnStoryInitialized += OnStoryInitialized;
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

    public MTR_SceneData GetSceneData(Scene sceneName)
    {
        return mtr_SceneDataObject.GetSceneData(sceneName);
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
