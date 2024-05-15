using System;
using System.Collections.Generic;

using Darklight.Game;
using Darklight.Console;

using Ink.Runtime;

using UnityEngine;
using Darklight.UnityExt.Editor;
using System.Linq;
using EasyButtons;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
[RequireComponent(typeof(InkyStoryLoader))]
public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>
{
    // ----- [[ STATIC FIELDS ]] ----- >>
    public static Story currentStory;

    // ----- [[ SERIALIZED FIELDS ]] ----- >>
    [SerializeField] InkyStoryObject currentStoryObject;
    [SerializeField] InkyKnotIterator currentKnotIterator;

    public void ContinueStory()
    {
        Story story = currentStoryObject.Story;

        if (story.canContinue)
        {
            if (currentKnotIterator != null)
            {
                currentKnotIterator.ContinueKnot();
            }
            else
            {
                // Continue the main story thread
                string text = story.Continue();
                text = text.TrimEnd('\n');
                Console.Log($"{Prefix} ContinueStory -> {text}");
            }
        }
        else if (story.currentChoices.Count > 0)
        {
            Console.Log($"{Prefix} Choices: {story.currentChoices.Count}", 1);

            foreach (Choice choice in story.currentChoices)
            {
                Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }
        else
        {
            Console.Log($"{Prefix} End of Story");
        }
    }

    public void BindExternalFunction(string funcName, Story.ExternalFunction function, bool lookaheadSafe = false)
    {
        currentStory.BindExternalFunctionGeneral(funcName, function, lookaheadSafe);
    }

    public object RunExternalFunction(string func, object[] args)
    {
        if (currentStory.HasFunction(func))
        {
            return currentStory.EvaluateFunction(func, args);
        }
        else
        {
            Debug.LogError("Could not find function: " + func);
            return null;
        }
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

        base.DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif