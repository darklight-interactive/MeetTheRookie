using System;

using Ink.Runtime;

using UnityEngine;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
[RequireComponent(typeof(InkyStoryLoader))]
public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>
{
    public InkyStoryLoader storyLoader => GetComponent<InkyStoryLoader>();

    public InkyStoryObject globalStoryObject;

    [ShowOnly] public string _currentSpeaker;

    public override void Awake()
    {
        base.Awake();

        if (globalStoryObject == null) return;
        globalStoryObject.story.ObserveVariable("CURRENT_SPEAKER", (string varName, object newValue) =>
        {
            _currentSpeaker = newValue.ToString();
            Debug.Log($"[InkyStoryManager] >> Current Speaker: {_currentSpeaker}");
        });

        globalStoryObject.story.ObserveVariable("ACTIVE_QUEST_CHAIN", (string varName, object newValue) =>
        {
            Debug.Log($"[InkyStoryManager] >> Active Quest Chain: {newValue}");
        });
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InkyStoryManager))]
public class InkyStoryManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    InkyStoryManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (InkyStoryManager)target;
        _script.Awake();
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