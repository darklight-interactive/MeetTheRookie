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
    [SerializeField, HideInInspector] private List<string> _knotNames = new List<string> { "default" };

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
        }
    }
}


/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTR_SceneDataObject : BuildSceneDataObject<MTR_SceneData>
{
    new MTR_SceneData[] buildSceneData
    {
        get
        {
            return base.buildSceneData;
        }
        set
        {
            base.buildSceneData = value;
        }
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetAllBuildSceneData().Find(x => x.knot == knot);
    }

    public EventReference GetActiveBackgroundMusicEvent()
    {
        MTR_SceneData data = GetActiveSceneData();
        return data.backgroundMusicEvent;
    }

    /// <summary>
    /// Saves the build scene data by updating the paths of the BuildSceneData objects
    /// based on the paths in the EditorBuildSettingsScene array.
    /// </summary>
    public override void SaveBuildSceneData(string[] buildScenePaths)
    {
        this.buildScenePaths = buildScenePaths;
        int buildScenePathsLength = buildScenePaths.Length;
        List<MTR_SceneData> newSceneData = new List<MTR_SceneData>(buildScenePathsLength);

        for (int i = 0; i < buildScenePathsLength; i++)
        {
            string scenePath = buildScenePaths[i];

            // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
            if (this.buildSceneData.Length <= i || this.buildSceneData[i].Path != scenePath)
            {
                Debug.Log($"{this.buildSceneData[i].Path} -> Saving scene data for {scenePath}");
                newSceneData.Add(new MTR_SceneData());
            }
            else
            {
                newSceneData.Add(this.buildSceneData[i]);
            }

            // Initialize the scene data.
            newSceneData[i].InitializeData(scenePath);
        }

        // Update the build scene data.
        buildSceneData = newSceneData.ToArray();

#if UNITY_EDITOR
        if (this != null)
            EditorUtility.SetDirty(this);
#endif
        Debug.Log($"{this.name} Saved MTR build scene data.");
    }
}

/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
public class MTR_SceneManager : BuildSceneDataManager<MTR_SceneData>
{
    MTR_SceneDataObject _mtrSceneDataObject
    {
        get
        {
            return buildSceneDataObject as MTR_SceneDataObject;
        }
        set
        {
            buildSceneDataObject = value;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        InkyStoryManager.Instance.OnStoryInitialized += OnStoryInitialized;
    }

    public override void CreateBuildSceneDataObject()
    {
        _mtrSceneDataObject =
            ScriptableObjectUtility.CreateOrLoadScriptableObject<MTR_SceneDataObject>(
                DATA_PATH,
                DATA_FILENAME
            );
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
