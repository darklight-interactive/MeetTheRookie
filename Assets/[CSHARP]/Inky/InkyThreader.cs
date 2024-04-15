using System;
using System.Collections.Generic;
using System.Linq;

using Darklight.Game;
using Darklight.Console;

using Ink.Runtime;

using UnityEngine;
using Darklight.Game.Utility;
using Darklight.UnityExt;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
public class InkyThreader : MonoBehaviourSingleton<InkyThreader>
{
    private const string PATH = "Inky/";
    private const string SPEAKER_TAG = "speaker";
    public static ConsoleGUI Console = new ConsoleGUI();

    // ========================  [[ STATE MACHINE ]]  ========================
    public enum State { INIT, LOAD, CONTINUE, CHOICE, END, ERROR }
    public class StateMachine : StateMachine<State>
    {
        public StateMachine(State initialState = State.INIT) : base(initialState) { }
    }
    StateMachine stateMachine = new StateMachine(State.INIT);


    // ========================  [[ INKY KNOT THREADER ]]  ========================
    Queue<InkyKnot> mainKnotThread = new Queue<InkyKnot>();
    Dictionary<string, InkyKnot> knotDictionary = new Dictionary<string, InkyKnot>();

    public State currentState => stateMachine.CurrentState;
    public Story currentStory { get; private set; }
    public InkyGlobalVariables currentVariables { get; private set; }
    public InkyKnot currentKnot { get; private set; }

    public string currentStoryName = "scene1";
    [SerializeField, ShowOnly] private string currentStoryFilePath => PATH + currentStoryName;

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
        stateMachine.ChangeState(State.LOAD);
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
            currentStory.onError += (message, type) =>
            {
                Debug.LogError("[Ink] " + type + " " + message);
                Console.Log($"{Prefix} Story Error: {message}", 0, LogSeverity.Error);
            };

            // Get Variables
            InkyGlobalVariables variables = new InkyGlobalVariables(currentStory);

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

    public void GoToKnotAt(string pathString)
    {
        stateMachine.ChangeState(State.LOAD);
        currentKnot = new InkyKnot(currentStory, pathString);
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            stateMachine.ChangeState(State.CONTINUE);
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
            stateMachine.ChangeState(State.CHOICE);
            Console.Log($"{Prefix} Choices: {currentStory.currentChoices.Count}", 1);

            foreach (Choice choice in currentStory.currentChoices)
            {
                Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }
        else
        {
            stateMachine.ChangeState(State.END);
            Console.Log($"{Prefix} End of Story");

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InkyThreader))]
    public class InkyKnotThreaderEditor : Editor
    {

        private void OnEnable()
        {
            InkyThreader threader = (InkyThreader)target;

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InkyThreader threader = (InkyThreader)target;

            EditorGUILayout.Space();

            CustomInspectorGUI.CreateTwoColumnLabel("Current State: ", threader.currentState.ToString());
            CustomInspectorGUI.CreateTwoColumnLabel("Current Story: ", threader.currentStoryName);
            CustomInspectorGUI.CreateTwoColumnLabel("Current Knot: ", threader.currentKnot?.ToString() ?? "None");


            if (threader.currentState == State.INIT && GUILayout.Button("Load Story"))
            {
                threader.LoadStory(threader.currentStoryName);
            }

            if (GUILayout.Button("Continue Story"))
            {
                threader.ContinueStory();
            }

            InkyThreader.Console.DrawInEditor();
        }
    }
}
#endif



