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
public class MTRSceneData : BuildSceneScriptableData
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
    [SerializeField] MTRCameraBounds _cameraBounds = new MTRCameraBounds();

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, Expandable] MTRCameraRigSettings _cameraRigSettings;

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
    public MTRCameraRigSettings CameraRigSettings => _cameraRigSettings;
    public MTRCameraBounds CameraRigBounds => _cameraBounds;

    public override void Initialize(string path)
    {
        base.Initialize(path);
    }

    public override void Copy(IBuildSceneData data)
    {
        base.Copy(data);
    }

    public override void Refresh()
    {
        base.Refresh();

        if (_sceneKnot == "None")
            FindSceneKnot();

        if (_sceneBounds == null)
            _sceneBounds = new MTRSceneBounds();

        if (_cameraBounds == null)
            _cameraBounds = new MTRCameraBounds();
        _cameraBounds.center.x = _sceneBounds.Center.x;
        _cameraBounds.xAxisBounds.Min = _sceneBounds.Left - 1;
        _cameraBounds.xAxisBounds.Max = _sceneBounds.Right + 1;
    }

    void FindSceneKnot()
    {
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
                _foundSceneKnot = true;
                return;
            }
        }
        else
        {
            _sceneKnot = "None";
            _foundSceneKnot = false;
        }
    }

    void DrawGizmos()
    {
        SceneBounds.DrawGizmos();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRSceneData))]
    public class MTRSceneScriptableDataCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRSceneData _script;
        EasyButtons.Editor.ButtonsDrawer _buttonsDrawer;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRSceneData)target;
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
                _script.Refresh();
            }
        }
    }
#endif
}

