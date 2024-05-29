using System.Collections.Generic;

using Darklight.UnityExt.Editor;
using Darklight.UnityExt.SceneManagement;

using EasyButtons;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MTR_SceneData : BuildSceneData
{
    private List<string> _knotNames
    {
        get
        {
            return InkyStoryManager.GlobalStoryObject.KnotNames;
        }
    }

    [NaughtyAttributes.Dropdown("_knotNames")]
    public string knot;
}


public class MTR_SceneManager : BuildSceneManager<MTR_SceneData>
{
    private Dictionary<string, MTR_SceneData> _sceneDataByKnot = new Dictionary<string, MTR_SceneData>();

    public override void Initialize()
    {
        base.Initialize();

        foreach (MTR_SceneData scene in buildScenes)
        {
            _sceneDataByKnot.Add(scene.knot, scene);
        }
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return _sceneDataByKnot[knot];
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
            BuildSceneManagementWindow.ShowWindow();
        }

        // Display the active scene name.
        MTR_SceneData activeScene = _script.GetActiveSceneData();
        if (activeScene != null)
        {
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Scene", activeScene.name);
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Knot", activeScene.knot);
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}


#endif
