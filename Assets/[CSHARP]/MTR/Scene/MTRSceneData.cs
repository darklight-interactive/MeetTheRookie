using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.BuildScene;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTRSceneData : BuildSceneScriptableData
{
    bool _foundSceneKnot;
    List<string> _knotList = new List<string> { "None" };
    List<string> _stitchList = new List<string> { "None" };

    [Header("MTR STORY SETTINGS")]
    [SerializeField, ShowOnly]
    string _internalSceneKnot = "None";

    [SerializeField, Dropdown("_dropdown_sceneKnotList"), HideIf("_foundSceneKnot")]
    string _sceneKnot = "None";

    [Header("MTR INTERACTION SETTINGS")]
    [SerializeField]
    bool _forceInteractionOnEnter;

    [SerializeField, Dropdown("_dropdown_sceneStitchList"), ShowIf("_forceInteractionOnEnter")]
    string _onEnterInteractionStitch = "None";

    [Header("MTR SCENE SETTINGS")]
    [SerializeField, Range(0f, 5f)]
    float _cameraBoundsOffset = 0.25f;

    [SerializeField]
    MTRSceneBounds _sceneBounds = new MTRSceneBounds();

    [SerializeField]
    MTRCameraBounds _cameraBounds = new MTRCameraBounds();

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField]
    MTRCameraController.SettingType _onStartCameraSetting = MTRCameraController.SettingType.DEFAULT;

    #region ---- < PROPERTIES > ---------------------------------
    List<string> _dropdown_sceneKnotList
    {
        get
        {
            if (MTRStoryManager.Instance.SceneKnotList != null)
            {
                _knotList = new List<string>() { "None" };
                _knotList.AddRange(MTRStoryManager.Instance.SceneKnotList);
            }
            return _knotList;
        }
    }
    List<string> _dropdown_sceneStitchList
    {
        get
        {
            if (MTRStoryManager.Instance.SceneKnotList != null && _sceneKnot != null)
            {
                if (MTRStoryManager.Instance.SceneKnotList.Contains(_sceneKnot))
                {
                    _stitchList = new List<string>() { "None" };
                    _stitchList.AddRange(InkyStoryManager.GetAllStitchesInKnot(_sceneKnot));
                }
                else
                {
                    _stitchList = new List<string>() { "None" };
                }
            }
            return _stitchList;
        }
    }

    public string SceneKnot => _sceneKnot;
    public string OnEnterStitch => _onEnterInteractionStitch;
    public MTRSceneBounds SceneBounds => _sceneBounds;
    public MTRCameraController.SettingType OnStartCameraSetting => _onStartCameraSetting;
    public MTRCameraBounds CameraRigBounds => _cameraBounds;
    #endregion

    //  ---------------- [ METHODS ] -----------------------------
    public override void Refresh()
    {
        base.Refresh();

        RefreshSceneSettings();
        RefreshCameraSettings();
        RefreshStorySettings();
    }

    void RefreshSceneSettings()
    {
        // Initialize scene bounds if not set
        _sceneBounds ??= new MTRSceneBounds();

        // Set default X-axis values for the scene bounds if not set
        if (_sceneBounds.XAxisValues == Vector2.zero)
            _sceneBounds.XAxisValues = new Vector2(-5, 5);
    }

    void RefreshCameraSettings()
    {
        // Initialize camera bounds if not set
        _cameraBounds ??= new MTRCameraBounds();

        // Set the camera bounds to match the scene bounds
        _cameraBounds.Center = _sceneBounds.Center;
        _cameraBounds.XAxisValues.x = _sceneBounds.XAxisValues.x - _cameraBoundsOffset;
        _cameraBounds.XAxisValues.y = _sceneBounds.XAxisValues.y + _cameraBoundsOffset;
        _cameraBounds.YAxisValues.x = _sceneBounds.YAxisValues.x - _cameraBoundsOffset;
        _cameraBounds.YAxisValues.y = _sceneBounds.YAxisValues.y + _cameraBoundsOffset;
    }

    void RefreshStorySettings()
    {
        // << VALIDATE FOUND KNOT
        if (_foundSceneKnot)
        {
            // Update the internal scene knot if the scene knot has changed
            if (_sceneKnot != "None" && _internalSceneKnot != _sceneKnot)
            {
                _internalSceneKnot = _sceneKnot;
                return;
            }
            else if (_sceneKnot == "None" && _internalSceneKnot != "None")
            {
                _sceneKnot = _internalSceneKnot;
                return;
            }
            else if (_sceneKnot == "None" && _internalSceneKnot == "None")
            {
                _foundSceneKnot = false;
            }
        }

        // << CHECK IF SCENE KNOT IS SET >>
        if (!_foundSceneKnot && _sceneKnot != "None")
        {
            _internalSceneKnot = _sceneKnot;
            _foundSceneKnot = true;
        }

        // << GET SCENE KNOT LIST >>
        List<string> sceneKnotList = MTRStoryManager.Instance.SceneKnotList;
        if (sceneKnotList == null || sceneKnotList.Count == 0)
        {
            _foundSceneKnot = false;
            return;
        }

        // << PARSE SCENE NAME >>
        string sceneName = Name.ToLower();
        sceneName = sceneName.Replace(" ", ""); // Get the scene name and remove spaces
        sceneName = sceneName.Replace("-", "_"); // Replace hyphens with underscores

        // << FIND RLEATED KNOT >>
        List<string> sceneNameParts = sceneName.Split('_').ToList();
        if (sceneNameParts.Contains("scene"))
        {
            string sceneIndex = sceneNameParts[1];
            string sectionIndex = sceneNameParts[2];

            // Check if the scene knot exists
            if (sceneKnotList.Contains($"scene{sceneIndex}_{sectionIndex}"))
            {
                _sceneKnot = $"scene{sceneIndex}_{sectionIndex}";
                _internalSceneKnot = _sceneKnot;
                _foundSceneKnot = true;
                return;
            }
        }
    }

    public void DrawGizmos()
    {
        SceneBounds.DrawGizmos();
        CameraRigBounds.DrawGizmos();
    }
}
