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
    const string PATH = "Inky/";
    public InkyStoryLoader storyLoader => GetComponent<InkyStoryLoader>();
    public InkyKnotIterator currentKnot { get; private set; }

    #region ==== State Machine ====
    public enum State { INIT, LOAD, CONTINUE, CHOICE, END, ERROR }
    public class StateMachine : StateMachine<State>
    {
        public StateMachine(State initialState = State.INIT) : base(initialState) { }
    }
    StateMachine stateMachine = new StateMachine(State.INIT);
    public State currentState => stateMachine.CurrentState;
    #endregion

    [Dropdown("storyLoader.NameKeys")]
    public string currentStoryKey;
    public InkyStory currentStoryWrapper;
    private Story _story => currentStoryWrapper;


    public override void Awake()
    {
        base.Awake();
        stateMachine.ChangeActiveStateTo(State.INIT);

        storyLoader.Load();
        currentStoryWrapper = storyLoader.GetStory(currentStoryKey);
    }

    [Button("Continue Story")]
    public void ContinueStory()
    {
        if (_story.canContinue)
        {
            stateMachine.ChangeActiveStateTo(State.CONTINUE);
            if (currentKnot != null)
            {
                currentKnot.ContinueKnot();
            }
            else
            {
                // Continue the main story thread
                string text = _story.Continue();
                text = text.TrimEnd('\n');
                Console.Log($"{Prefix} ContinueStory -> {text}");
            }
        }
        else if (_story.currentChoices.Count > 0)
        {
            stateMachine.ChangeActiveStateTo(State.CHOICE);
            Console.Log($"{Prefix} Choices: {_story.currentChoices.Count}", 1);

            foreach (Choice choice in _story.currentChoices)
            {
                Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }
        else
        {
            stateMachine.ChangeActiveStateTo(State.END);
            Console.Log($"{Prefix} End of Story");
        }
    }

    public void BindExternalFunction(string funcName, Story.ExternalFunction function, bool lookaheadSafe = false)
    {
        _story.BindExternalFunctionGeneral(funcName, function, lookaheadSafe);
    }

    public object RunExternalFunction(string func, object[] args)
    {
        if (_story.HasFunction(func))
        {
            return _story.EvaluateFunction(func, args);
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