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
public class MTRSceneManager : BuildSceneScriptableDataManager<MTRSceneScriptableData>, IUnityEditorListener
{
    public new static string Prefix = "[MTRSceneManager]";
    public new static MTRSceneManager Instance => BuildSceneScriptableDataManager<MTRSceneScriptableData>.Instance as MTRSceneManager;

    //  ================================ [[ Fields ]] ================================
    MTRSceneController _sceneController;
    MTRCameraBoundsLibrary _cameraBoundsLibrary;


    protected override string AssetPath => "Assets/Resources/MeetTheRookie/BuildSceneData";

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
    public MTRCameraBoundsLibrary CameraBoundsLibrary => _cameraBoundsLibrary;
    public Scene ActiveScene => SceneManager.GetActiveScene();



    void HandleStoryInitialized()
    {
        if (!Application.isPlaying)
            return;

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
        TryGetSceneDataByKnot(knotName, out MTRSceneScriptableData data);
        if (data == null)
            return false;

        SceneController.TryLoadScene(data.Name);
        Debug.Log($"{Prefix} >> Inky ChangeGameScene >> {data.Name}");

        InkyStoryManager.GoToKnotOrStitch(knotName);

        return true;
    }

    public override void Initialize()
    {
        // << Initialize Camera Bounds Library >>
        if (_cameraBoundsLibrary == null
            || _cameraBoundsLibrary.Count == 0 || _cameraBoundsLibrary.Count != NameList.Count)
            _cameraBoundsLibrary = new MTRCameraBoundsLibrary(NameList);

        // << Set Active Camera Bounds >>
        MTRCameraRigBounds activeCameraBounds = GetActiveCameraBounds();
        if (activeCameraBounds != null)
        {
            if (SceneController.CameraController != null)
                SceneController.CameraController.Rig.SetBounds(activeCameraBounds);
        }

        // << Initialize Story >>
        MTRStoryManager.OnStoryInitialized += HandleStoryInitialized;

        base.Initialize();
    }

    public void TryGetSceneDataByKnot(string knot, out MTRSceneScriptableData sceneData)
    {
        sceneData = DataList.Find(x => x.SceneKnot == knot);
    }

    public MTRCameraRigBounds GetActiveCameraBounds()
    {
        MTRCameraRigBounds cameraBounds = null;
        try
        {
            _cameraBoundsLibrary.TryGetValue(ActiveSceneData.Name, out cameraBounds);
        }
        catch (Exception e) { }

        return cameraBounds;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        ActiveSceneData.SceneBounds.DrawGizmos();
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
            if (GUILayout.Button("Initialize")) _script.Initialize();
            if (GUILayout.Button("Clear")) _script.Clear();
            EditorGUILayout.EndHorizontal();


            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}

[Serializable]
public class ExpandableItem<T>
    where T : ScriptableObject
{
    [Expandable] public T Item;
}
