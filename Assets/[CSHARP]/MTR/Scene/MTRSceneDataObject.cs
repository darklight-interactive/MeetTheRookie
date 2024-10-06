using System.Collections.Generic;
using System.Linq;

using Darklight.UnityExt.BuildScene;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;

using EasyButtons;

using FMODUnity;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTRSceneDataObject : BuildSceneScriptableData<MTRSceneData>
{
    [SerializeField, ShowOnly, NonReorderable] List<string> buildSceneNames = new List<string>();

    public List<string> SceneNames
    {
        //get => buildSceneNames = SceneData.Select(x => x.Name).ToList();
        get => buildSceneNames;
    }

    [Button]
    public void Initialize(string[] buildScenePaths)
    {
        //this.buildScenePaths = buildScenePaths;
        //this.buildSceneNames = SceneData.Select(x => x.Name).ToList();

        //Initialize();
    }

    public List<MTRSceneData> GetBuildSceneData()
    {
        //return buildSceneData.ToList();
        return null;
    }

    public MTRSceneData GetSceneDataByKnot(string knot)
    {
        //return GetBuildSceneData().Find(x => x.knot == knot);
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTRSceneDataObject))]
public class MTRSceneDataObjectCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MTRSceneDataObject _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTRSceneDataObject)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();



        if (EditorGUI.EndChangeCheck())
        {

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
