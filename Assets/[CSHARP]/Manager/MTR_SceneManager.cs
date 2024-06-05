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

[System.Serializable]
public class MTR_SceneData : BuildSceneData
{
    private InkyStoryObject _globalStoryObject;
    private List<string> _knotNames = new List<string> { "default" };

    [SerializeField, ShowOnly] private string _savedKnotData = "default";

    [Dropdown("_knotNames")]
    public string knot;
    public EventReference backgroundMusicEvent;

    public override void InitializeData(string path)
    {
        base.InitializeData(path);

        if (InkyStoryManager.Instance != null)
        {
            _globalStoryObject = InkyStoryManager.GlobalStoryObject;
            _knotNames = _globalStoryObject.KnotNameList;
            _savedKnotData = knot;
        }
    }
}

/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
public class MTR_SceneManager : BuildSceneDataManager<MTR_SceneData>
{
    protected MTR_SceneDataObject _mtrSceneDataObject => (MTR_SceneDataObject)buildSceneDataObject;

    public override void Initialize()
    {
        base.Initialize();
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
        MTR_SceneData data = _mtrSceneDataObject.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        return true;
    }

    public MTR_SceneData GetSceneData(Scene sceneName)
    {
        return _mtrSceneDataObject.GetSceneData(sceneName);
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return this._mtrSceneDataObject.GetSceneDataByKnot(knot);
    }

    public MTR_SceneData GetActiveSceneData()
    {
        if (_mtrSceneDataObject == null)
            return new MTR_SceneData();
        return _mtrSceneDataObject.GetActiveSceneData();
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
