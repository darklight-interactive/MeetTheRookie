using System;

using Ink.Runtime;

using UnityEngine;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;



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
    private List<string> _storyKnots => InkyStoryObject.GetAllKnots(globalStoryObject.story);


    [System.Serializable]
    public class SceneKnot
    {
        public SceneObject sceneName;
        [Dropdown("_storyKnots")]
        public string knotName;
    }



    [Tooltip("List of Scene Knots that will be used to track the progress of the story.")]
    public List<SceneKnot> sceneKnots = new List<SceneKnot>();


    public InkyStoryObject globalStoryObject;

    [ShowOnly] public string _currentSpeaker;
    [ShowOnly] public List<string> _speakers;

    public override void Awake()
    {
        base.Awake();

        if (globalStoryObject == null) return;

        _speakers = globalStoryObject.GetVariableByName("Speaker").ToStringList();

        globalStoryObject.BindExternalFunction("QuestStarted", QuestStarted);
        globalStoryObject.BindExternalFunction("GoToScene", GoToScene);

        globalStoryObject.story.ObserveVariable("Speaker", (string varName, object newValue) =>
        {
            InkyVariable speaker = new InkyVariable(varName, newValue);
            _speakers = speaker.ToStringList();

            Debug.Log($"[InkyStoryManager] >> Speaker List: {_speakers}");
        });

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

    public object GoToScene(object[] args)
    {
        Debug.Log("Go To Scene! >> " + args[0].ToString());
        return false;
    }

    public object QuestStarted(object[] args)
    {
        Debug.Log("Quest Started! >> " + args[0].ToString());
        return false;
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