using System.Collections.Generic;
using System.Linq;

using Darklight.UnityExt.BuildScene;

using UnityEngine;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTRSceneScriptableData : BuildSceneScriptableData<MTRSceneData>
{
    bool _foundSceneKnot;
    List<string> _knotList = new List<string> { "None" };
    List<string> _stitchList = new List<string> { "None" };


    [Header("MTR STORY SETTINGS")]
    [SerializeField, Dropdown("_dropdown_sceneKnotList"), DisableIf("_foundSceneKnot")]
    string _sceneKnot = "None";

    [SerializeField, Dropdown("_dropdown_sceneStitchList")]
    string _onEnterInteractionStitch = "None";

    [Header("MTR SCENE SETTINGS")]
    [SerializeField] MTRSceneBounds _sceneBounds = new MTRSceneBounds();

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, Expandable] MTRCameraRigSettings _cameraRigSettings;
    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;

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

    public override MTRSceneData ToData()
    {
        return new MTRSceneData()
        {
            SceneKnot = _sceneKnot,
            OnEnterStitch = _onEnterInteractionStitch,

            SceneBounds = new MTRSceneBounds(_sceneBounds, true),

            CameraRigSettings = _cameraRigSettings,
            CameraRigBounds = _cameraRigBounds
        };
    }

    public override void Refresh()
    {
        TrySearchForSceneKnot();
        if (_sceneKnot == "None")
        {
            _foundSceneKnot = false;
        }

        _sceneBounds = new MTRSceneBounds(_sceneBounds, true);
    }

    void TrySearchForSceneKnot()
    {
        // << GET SCENE KNOT LIST >>
        List<string> sceneKnotList = MTRStoryManager.Instance.SceneKnotList;
        if (sceneKnotList == null || sceneKnotList.Count == 0)
            return;

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
                _foundSceneKnot = true;

                //Debug.Log($"< MTRSceneData > >> Found SceneKnot for {Name} >> ({Knot})");
                return;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRSceneScriptableData))]
    public class MTRSceneScriptableDataCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRSceneScriptableData _script;
        EasyButtons.Editor.ButtonsDrawer _buttonsDrawer;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRSceneScriptableData)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh"))
            {
                _script.Refresh();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}

