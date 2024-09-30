using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using Darklight.UnityExt.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTRSceneDataObject : BuildSceneDataObject<MTRSceneData>
{
    public void Initialize(string[] buildScenePaths)
    {
        this.buildScenePaths = buildScenePaths;
        Initialize();
    }

    public List<MTRSceneData> GetBuildSceneData()
    {
        return buildSceneData.ToList();
    }

    public MTRSceneData GetSceneDataByKnot(string knot)
    {
        return GetBuildSceneData().Find(x => x.knot == knot);
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
