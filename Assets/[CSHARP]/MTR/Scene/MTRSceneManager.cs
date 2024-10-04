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
using Darklight.UnityExt.Library;
using System.Collections;






#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
[RequireComponent(typeof(MTRSceneController))]
public class MTRSceneManager : BuildSceneDataManager<MTRSceneData>, IUnityEditorListener
{
    public new static string Prefix = "[ MTRSceneManager ]";
    public new static MTRSceneManager Instance => BuildSceneDataManager<MTRSceneData>.Instance as MTRSceneManager;
    public static MTRSceneBounds SceneBounds
    {
        get
        {
            MTRSceneBounds[] bounds = FindObjectsByType<MTRSceneBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            return bounds.Length > 0 ? bounds[0] : null;
        }
    }

    [SerializeField, ShowOnly] string _sceneToLoad;
    [SerializeField, Expandable] MTRSceneDataObject _sceneDataObject;
    [SerializeField] MTRCameraBoundsLibrary _cameraBoundsLibrary;

    public MTRSceneController SceneController => GetComponent<MTRSceneController>();
    public MTRSceneDataObject SceneDataObject => _sceneDataObject;
    public List<string> SceneNames => _sceneDataObject.SceneNames;
    public MTRCameraBoundsLibrary CameraBoundsLibrary => _cameraBoundsLibrary;

    public void OnEditorReloaded()
    {
        Awake();
    }

    public override void Awake()
    {
        base.Awake();

        if (_cameraBoundsLibrary == null
            || _cameraBoundsLibrary.Count == 0 || _cameraBoundsLibrary.Count != SceneNames.Count)
            _cameraBoundsLibrary = new MTRCameraBoundsLibrary(SceneNames);

        MTRCameraRigBounds activeCameraBounds = GetActiveCameraBounds();
        if (activeCameraBounds != null)
        {
            SceneController.CameraController.Rig.SetBounds(activeCameraBounds);
        }
    }

    public void OnEnable()
    {
        OnSceneChanged += SceneController.OnActiveSceneChanged;
    }

    public void OnDisable()
    {
        OnSceneChanged -= SceneController.OnActiveSceneChanged;
    }

    void OnStoryInitialized(Story story)
    {
        if (!Application.isPlaying)
            return;
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
        MTRSceneData data = _sceneDataObject.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        _sceneToLoad = data.Name;
        return true;
    }

    public override void Initialize()
    {
#if UNITY_EDITOR
        _sceneDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<MTRSceneDataObject>(
            DATA_PATH,
                DATA_FILENAME
            );

        if (_sceneDataObject == null)
        {
            Debug.LogError($"{this.name} Failed to create or load build scene data object.");
            return;
        }
        else
        {
            Debug.Log($"{this.name} Build Scene Data Object loaded successfully. {_sceneDataObject}");
        }

        base.LoadBuildScenes();
#endif

        _sceneDataObject.Initialize(buildScenePaths);

        //SaveBuildSceneData(buildScenePaths);
        Debug.Log($"{Prefix} Initialized.");

        Story story = InkyStoryManager.GlobalStory;
        if (story != null)
            OnStoryInitialized(story);
        else
            Debug.LogError($"{Prefix} Story is null.");
    }

    /// <summary>
    /// Saves the build scene data by updating the paths of the BuildSceneData objects
    /// based on the paths in the EditorBuildSettingsScene array.
    /// </summary>
    public override void SaveBuildSceneData(string[] buildScenePaths)
    {
#if UNITY_EDITOR
        this.buildScenePaths = buildScenePaths;
        List<MTRSceneData> buildSceneData = _sceneDataObject.GetBuildSceneData();

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

    IEnumerator LoadSceneAsyncRoutine()
    {
        // Begin loading the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad);

        // Prevent the scene from being activated immediately
        asyncOperation.allowSceneActivation = false;

        // While the scene is still loading
        while (!asyncOperation.isDone)
        {
            // Output the current progress (0 to 0.9)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // If the loading is almost done (progress >= 90%), allow activation
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;

                yield return new WaitForSeconds(0.5f);
                SceneController.StateMachine.GoToState(MTRSceneState.INITIALIZE);
            }

            yield return null;
        }
    }

    public MTRSceneData GetSceneData(string name)
    {
        return _sceneDataObject.GetSceneData(name);
    }

    public MTRSceneData GetSceneDataByKnot(string knot)
    {
        return this._sceneDataObject.GetSceneDataByKnot(knot);
    }

    public MTRSceneData GetActiveSceneData()
    {
        if (_sceneDataObject == null)
            return new MTRSceneData();
        return _sceneDataObject.GetActiveSceneData();
    }

    public MTRCameraRigBounds GetActiveCameraBounds()
    {
        return _cameraBoundsLibrary[GetActiveSceneData().Name];
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

        if (!Application.isPlaying)
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

