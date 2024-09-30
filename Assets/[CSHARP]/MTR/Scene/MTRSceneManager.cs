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
using Darklight.UnityExt.Utility;




#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
[RequireComponent(typeof(MTRSceneController))]
public class MTRSceneManager : BuildSceneDataManager<MTRSceneData>
{
    public static MTRSceneController Controller => Instance.GetComponent<MTRSceneController>();
    public static MTRSceneController.InternalStateMachine StateMachine => Controller.StateMachine;
    public static MTRSceneState CurrentState => Controller.CurrentState;

    [SerializeField, Expandable] MTRSceneDataObject _sceneData;



    public override void Initialize()
    {
#if UNITY_EDITOR
        _sceneData = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRSceneDataObject>(
            DATA_PATH,
                DATA_FILENAME
            );

        if (_sceneData == null)
        {
            Debug.LogError($"{this.name} Failed to create or load build scene data object.");
            return;
        }
        else
        {
            Debug.Log($"{this.name} Build Scene Data Object loaded successfully. {_sceneData}");
        }

        base.LoadBuildScenes();
#endif

        _sceneData.Initialize(buildScenePaths);

        //SaveBuildSceneData(buildScenePaths);
        InkyStoryManager.Instance.OnStoryInitialized += OnStoryInitialized;
    }

    /// <summary>
    /// Saves the build scene data by updating the paths of the BuildSceneData objects
    /// based on the paths in the EditorBuildSettingsScene array.
    /// </summary>
    public override void SaveBuildSceneData(string[] buildScenePaths)
    {
#if UNITY_EDITOR
        this.buildScenePaths = buildScenePaths;
        List<MTRSceneData> buildSceneData = _sceneData.GetBuildSceneData();

        for (int i = 0; i < buildScenePaths.Length; i++)
        {
            string scenePath = buildScenePaths[i];

            // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
            if (buildSceneData.Count <= i || buildSceneData[i].Path != scenePath)
            {
                buildSceneData.Add(new MTRSceneData());
                Debug.Log($"{this.name} -> Added new MTR_SceneData object.");
            }

            // Initialize the scene data.
            buildSceneData[i].InitializeData(scenePath);
            //mtr_SceneDataObject.SaveSceneData(buildSceneData[i]);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"{this.name} Saved build scene data.");
#endif
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
    bool ChangeGameScene(string knotName)
    {
        MTRSceneData data = _sceneData.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        return true;
    }

    public MTRSceneData GetSceneData(string name)
    {
        return _sceneData.GetSceneData(name);
    }

    public MTRSceneData GetSceneDataByKnot(string knot)
    {
        return this._sceneData.GetSceneDataByKnot(knot);
    }

    public MTRSceneData GetActiveSceneData()
    {
        if (_sceneData == null)
            return new MTRSceneData();
        return _sceneData.GetActiveSceneData();
    }

    public void LoadSceneByKnot(string knot)
    {
        MTRSceneData data = GetSceneDataByKnot(knot);
        LoadScene(data.Name);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTRSceneManager))]
public class MTR_SceneManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MTRSceneManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTRSceneManager)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Initialize"))
        {
            _script.Initialize();
        }

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

