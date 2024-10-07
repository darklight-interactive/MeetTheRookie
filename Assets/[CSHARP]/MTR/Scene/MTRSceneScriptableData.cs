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
    bool _foundSceneKnot;
    List<string> _knotList = new List<string> { "Default" };
    List<string> _editor_sceneKnotList
    {
        get
        {
            if (InkyStoryManager.Instance.SceneKnotList != null)
                _knotList = InkyStoryManager.Instance.SceneKnotList;
            return _knotList;
        }
    }

    [Header("MTR STORY SETTINGS")]
    [SerializeField, Dropdown("_editor_sceneKnotList"), DisableIf("_foundSceneKnot")] string _knot = "";

    [Header("MTR CAMERA SETTINGS")]
    [SerializeField, Expandable] MTRCameraRigSettings _cameraRigSettings;
    [SerializeField, Expandable] MTRCameraRigBounds _cameraRigBounds;

    [Header("MTR GAMEPLAY SETTINGS")]
    [SerializeField] MTRSceneSpawnInfo _spawnInfo;


    public string Knot { get => _knot; set => _knot = value; }
    public MTRCameraRigSettings CameraRigSettings { get => _cameraRigSettings; set => _cameraRigSettings = value; }
    public MTRCameraRigBounds CameraRigBounds { get => _cameraRigBounds; set => _cameraRigBounds = value; }
    public MTRSceneSpawnInfo SpawnInfo { get => _spawnInfo; set => _spawnInfo = value; }

    public override void CopyData(MTRSceneData data)
    {
        base.CopyData(data);

        _foundSceneKnot = data.FoundSceneKnot;
        _knot = data.Knot;
        _spawnInfo = data.SpawnInfo;
    }

    public override MTRSceneData ToData()
    {
        return new MTRSceneData()
        {
            Path = this.Path,
            Knot = _knot,
            SpawnInfo = _spawnInfo,
        };
    }

    public override void Refresh()
    {
        if (_knot == null)
        {
            _knot = "Default";
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

