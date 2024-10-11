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
public class MTRSceneManager : BuildSceneScriptableDataManager<MTRSceneData, MTRSceneScriptableData>, IUnityEditorListener
{
    public new static string Prefix = "[MTRSceneManager]";
    public new static MTRSceneManager Instance => BuildSceneScriptableDataManager<MTRSceneData, MTRSceneScriptableData>.Instance as MTRSceneManager;
    public static MTRSceneBounds SceneBounds
    {
        get
        {
            MTRSceneBounds[] bounds = FindObjectsByType<MTRSceneBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            return bounds.Length > 0 ? bounds[0] : null;
        }
    }

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
        TryGetSceneDataByKnot(knotName, out MTRSceneData data);
        if (data == null)
            return false;

        SceneController.TryLoadScene(data.Name);
        Debug.Log($"{Prefix} >> Inky ChangeGameScene >> {data.Name}");

        InkyStoryManager.GoToKnotOrStitch(knotName);

        return true;
    }



    //  ---------------- [ Public Methods ] -----------------------------
    protected override void CreateOrLoadScriptableData(string scenePath, out MTRSceneScriptableData obj)
    {
        base.CreateOrLoadScriptableData(scenePath, out obj);

        obj.CameraRigBounds = _cameraBoundsLibrary[obj.Name];
        obj.Refresh();
    }

    public override void Initialize()
    {
        // << Initialize Camera Bounds Library >>
        if (_cameraBoundsLibrary == null
            || _cameraBoundsLibrary.Count == 0 || _cameraBoundsLibrary.Count != SceneNameList.Count)
            _cameraBoundsLibrary = new MTRCameraBoundsLibrary(SceneNameList);

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

    public override void SaveModifiedData(MTRSceneScriptableData scriptObj)
    {
        base.SaveModifiedData(scriptObj);

        TryGetSceneDataByName(scriptObj.Name, out MTRSceneData sceneData);
        if (sceneData == null)
        {
            Debug.LogError($"{Prefix} >> SceneData not found for {scriptObj.Name}");
            return;
        }

        sceneData.Knot = scriptObj.Knot;
        //Debug.Log($"{Prefix} >> Saved modified data for {scriptObj.name}. Knot : {sceneData.Knot}");
    }

    public void TryGetSceneDataByKnot(string knot, out MTRSceneData sceneData)
    {
        sceneData = SceneDataList.Find(x => x.Knot == knot);
    }

    public MTRCameraRigBounds GetActiveCameraBounds()
    {
        MTRCameraRigBounds cameraBounds = null;

        try
        {
            TryGetActiveSceneData(out MTRSceneData activeSceneData);
            if (activeSceneData == null)
                return null;

            cameraBounds = _cameraBoundsLibrary[activeSceneData.Name];
        }
        finally { }

        return cameraBounds;
    }
}

[Serializable]
public class ExpandableItem<T>
    where T : ScriptableObject
{
    [Expandable] public T Item;
}
