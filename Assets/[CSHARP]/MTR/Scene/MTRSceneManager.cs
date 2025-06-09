using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.BuildScene;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using FMODUnity;
using Ink.Runtime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
[RequireComponent(typeof(MTRSceneController))]
public class MTRSceneManager : BuildSceneScriptableDataManager<MTRSceneData>, IUnityEditorListener
{
    public static new string Prefix = "[MTRSceneManager]";
    public static new MTRSceneManager Instance =>
        BuildSceneScriptableDataManager<MTRSceneData>.Instance as MTRSceneManager;

    //  ================================ [[ Fields ]] ================================
    MTRSceneController _sceneController;

    protected override string AssetPath => "Assets/Resources/MeetTheRookie/BuildSceneData";

    [SerializeField]
    SceneObject _mainMenuScene;

    public MTRSceneController SceneController
    {
        get
        {
            if (_sceneController == null)
            {
                _sceneController = FindFirstObjectByType<MTRSceneController>();
            }
            return _sceneController;
        }
    }

    void HandleStoryInitialized()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        MTRStoryManager.GlobalStory.BindExternalFunction(
            "ChangeGameScene",
            (string knotName) => ChangeGameScene(knotName)
        );
        Debug.Log($"{Prefix} >> BOUND 'ChangeGameScene' to external function.");
    }

    /// <summary>
    /// This is the main external function that is called from the Ink story to change the game scene.
    /// </summary>
    /// <param name="args">0 : The name of the sceneKnot</param>
    /// <returns>False if BuildSceneData is null. True if BuildSceneData is valid.</returns>
    bool ChangeGameScene(string knotName)
    {
        TryGetSceneDataByKnot(knotName, out MTRSceneData data);
        if (data == null)
            return false;

        SceneController.TryLoadScene(data.Name);
        Debug.Log($"{Prefix} >> Inky ChangeGameScene >> {data.Name}");

        RefreshData();
        InkyStoryManager.GoToPath(knotName);

        return true;
    }

    protected override void RefreshData()
    {
        base.RefreshData();

        SceneController.CameraController?.Refresh();
    }

    public override void Initialize()
    {
        base.Initialize();

        // If in editor, set the camera to follow the player
        if (!Application.isPlaying)
            SceneController.CameraController?.SetPlayerAsFollowTarget();

        // << Set Active Camera Bounds && Settings >>
        SceneController.CameraController?.Refresh();

        // << Initialize Story >>
        MTRStoryManager.OnStoryInitialized += HandleStoryInitialized;
    }

    public void TryGetSceneDataByKnot(string knot, out MTRSceneData sceneData)
    {
        sceneData = DataList.Find(x => x.SceneKnot == knot);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (ActiveSceneData != null)
            ActiveSceneData.DrawGizmos();
    }

    public void LoadMainMenu()
    {
        SceneController.TryLoadScene(_mainMenuScene);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRSceneManager))]
    public class MTRSceneManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRSceneManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRSceneManager)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Initialize"))
                _script.Initialize();
            if (GUILayout.Button("Clear"))
                _script.Clear();
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                _script.RefreshData();
            }
        }
    }
#endif
}

[Serializable]
public class ExpandableItem<T>
    where T : ScriptableObject
{
    [Expandable]
    public T Item;
}
