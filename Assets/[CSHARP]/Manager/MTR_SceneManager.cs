using UnityEngine;
using Darklight.UnityExt.SceneManagement;
using EasyButtons;
using NaughtyAttributes;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;



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
    public string knotName;
}


public class MTR_SceneManager : BuildSceneManager<MTR_SceneData>
{


    public override void Initialize()
    {
        base.Initialize();
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
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();


        if (GUILayout.Button("Initialize"))
        {
            _script.Initialize();
        }

        if (GUILayout.Button("Reset"))
        {
            _script.Reset();
        
        }


        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
