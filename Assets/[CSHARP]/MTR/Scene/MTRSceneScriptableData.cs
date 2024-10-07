using System.Collections.Generic;
using System.Linq;

using Darklight.UnityExt.BuildScene;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;

using EasyButtons;

using FMODUnity;

using UnityEngine;
using UnityEngine.SceneManagement;
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
    string _savedKnot;
    List<string> _knotList = new List<string> { "Default" };
    List<string> SceneKnotList
    {
        get
        {
            if (InkyStoryManager.Instance.SceneKnotList != null)
                _knotList = InkyStoryManager.Instance.SceneKnotList;
            return _knotList;
        }
    }

    [Header("MTR STORY SETTINGS")]
    [SerializeField, Dropdown("SceneKnotList")] string _knot = "Default";

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, Expandable] MTRCameraRigSettings _cameraRigSettings;
    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;

    [Header("MTR GAMEPLAY SETTINGS")]
    [SerializeField] List<int> _spawnPoints = new List<int>();

    public MTRCameraRigBounds CameraRigBounds { get => _cameraRigBounds; set => _cameraRigBounds = value; }

    void OnValidate()
    {
        if (_knot != _savedKnot)
        {
            _savedKnot = _knot;
        }
    }

    public override MTRSceneData ToData()
    {
        return new MTRSceneData()
        {
            Path = this.Path,
            Knot = _knot,
        };
    }

    [EasyButtons.Button]
    public void Refresh()
    {

        if (!_knot.Contains("scene"))
        {
            // << PARSE SCENE NAME >>
            string sceneName = Name.ToLower().Replace(" ", ""); // Get the scene name and remove spaces
            sceneName = sceneName.Replace("-", "_"); // Replace hyphens with underscores

            // << FIND RLEATED KNOT >>
            List<string> sceneNameParts = sceneName.Split('_').ToList();
            if (sceneNameParts.Contains("scene"))
            {
                string sceneIndex = sceneNameParts[1];
                string sectionIndex = sceneNameParts[2];

                // Check if the scene knot exists
                if (SceneKnotList.Contains($"scene{sceneIndex}_{sectionIndex}"))
                {
                    _knot = $"scene{sceneIndex}_{sectionIndex}";
                    Debug.Log($"Scriptable Data for scene {sceneName} has found a matching knot: {_knot}");
                }
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

