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
public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>
{
    const string PATH = "Inky/";
    const string SPEAKER_TAG = "speaker";

    // ========================  [[ STATE MACHINE ]]  ========================
    public enum State { INIT, LOAD, CONTINUE, CHOICE, END, ERROR }
    public class StateMachine : StateMachine<State>
    {
        public StateMachine(State initialState = State.INIT) : base(initialState) { }
    }
    StateMachine stateMachine = new StateMachine(State.INIT);
    public State currentState => stateMachine.CurrentState;


    public Story currentStory { get; private set; }
    public InkyKnotIterator currentKnot { get; private set; }

    public string currentStoryName = "scene1";
    [SerializeField, ShowOnly] private string currentStoryFilePath => PATH + currentStoryName;

    public override void Awake()
    {
        base.Awake();
        stateMachine.ChangeActiveStateTo(State.INIT);
    }

    [Button("Load Story")]
    public void LoadStory()
    {
        LoadStory(currentStoryName);
    }

    private void Start()
    {
        LoadStory(currentStoryName);
    }

    /// <summary>
    /// Load a story from the Resources folder.
    /// </summary>
    /// <param name="storyName">The name of the Story File</param>
    /// <returns></returns>
    public bool LoadStory(string storyName)
    {
        stateMachine.ChangeActiveStateTo(State.LOAD);
        currentStoryName = storyName;
        Console.Log($"{Prefix} Loading Story: {storyName}");

        try
        {
            TextAsset storyAsset = (TextAsset)Resources.Load(PATH + storyName);
            currentStory = new Story(storyAsset.text);
        }
        catch (Exception e)
        {
            Console.Log($"{Prefix} Story Load Error: {e.Message}", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Story Load Error: {e.Message}");
            return false;
        }
        finally
        {
            // Set up Error Handling
            currentStory.onError += (message, type) =>
            {
                Debug.LogError("[Ink] " + type + " " + message);
                Console.Log($"{Prefix} Story Error: {message}", 0, LogSeverity.Error);
            };

            // Get Tags
            List<string> tags = currentStory.globalTags;
            if (tags != null && tags.Count > 0)
            {
                foreach (string tag in tags)
                {
                    Console.Log($"{Prefix} Found Tag: {tag}", 3);
                }
            }
        }

        Console.Log($"{Prefix} Story Loaded: {storyName}");
        return true;
    }



    public InkyKnotIterator CreateKnotIterator(string knotPath)
    {
        stateMachine.ChangeActiveStateTo(State.LOAD);
        currentKnot = new InkyKnotIterator(currentStory, knotPath);
        return currentKnot;
    }

    [Button("Continue Story")]
    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            stateMachine.ChangeActiveStateTo(State.CONTINUE);
            if (currentKnot != null)
            {
                currentKnot.ContinueKnot();
            }
            else
            {
                // Continue the main story thread
                string text = currentStory.Continue();
                text = text.TrimEnd('\n');
                Console.Log($"{Prefix} ContinueStory -> {text}");
            }
        }
        else if (currentStory.currentChoices.Count > 0)
        {
            stateMachine.ChangeActiveStateTo(State.CHOICE);
            Console.Log($"{Prefix} Choices: {currentStory.currentChoices.Count}", 1);

            foreach (Choice choice in currentStory.currentChoices)
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

    // Get All Knots and Stiches - https://github.com/inkle/ink/issues/406
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
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif