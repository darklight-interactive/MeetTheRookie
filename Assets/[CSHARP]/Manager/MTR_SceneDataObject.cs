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
public class MTR_SceneDataObject : BuildSceneDataObject<MTR_SceneData>
{
    public void Initialize(string[] buildScenePaths)
    {
        this.buildScenePaths = buildScenePaths;
        Initialize();
    }

    public List<MTR_SceneData> GetBuildSceneData()
    {
        return buildSceneData.ToList();
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetBuildSceneData().Find(x => x.knot == knot);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTR_SceneDataObject))]
public class MTR_SceneDataObjectCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MTR_SceneDataObject _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTR_SceneDataObject)target;
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

        Repaint();

    }
}
#endif
