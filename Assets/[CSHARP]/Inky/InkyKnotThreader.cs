using System;
using System.Collections.Generic;
using System.Linq;

using Darklight;

using Ink.Runtime;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
public class InkyKnotThreader : ISingleton<InkyKnotThreader>
{
    private const string PATH = "Inky/";
    private const string SPEAKER_TAG = "speaker";
    public static string Prefix => ISingleton<InkyKnotThreader>.Prefix;
    public static InkyKnotThreader Instance => ISingleton<InkyKnotThreader>.Instance;
    public static Darklight.Console Console = new Darklight.Console();


    // ========================  [[ STATE MACHINE ]]  ========================
    public enum State { INIT, LOAD, CONTINUE, CHOICE, END, ERROR }
    public class StateMachine : StateMachine<State>
    {
        public StateMachine(State initialState = State.INIT) : base(initialState) { }
    }

    // ========================  [[ INKY KNOT THREADER ]]  ========================
    StateMachine stateMachine = new StateMachine(State.INIT);
    Queue<InkyKnot> mainKnotThread = new Queue<InkyKnot>();
    Dictionary<string, InkyKnot> knotDictionary = new Dictionary<string, InkyKnot>();

    public State currentState => stateMachine.CurrentState;
    public Story currentStory { get; private set; }
    public InkyGlobalVariables currentVariables { get; private set; }
    public InkyKnot currentKnot { get; private set; }

    public bool LoadStory(string storyName)
    {
        stateMachine.ChangeState(State.LOAD);
        Console.Log($"{Prefix} Loading Story: {storyName}");

        try
        {
            TextAsset storyAsset = (TextAsset)Resources.Load(PATH + storyName);
            currentStory = new Story(storyAsset.text);
        }
        catch (Exception e)
        {
            Debug.LogError($"{Prefix} Story Load Error: {e.Message}");
            return false;
        }
        finally
        {
            currentStory.onError += (message, type) =>
            {
                Debug.LogError("[Ink] " + type + " " + message);
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

            currentStory.ContinueMaximally();
        }
        return true;
    }

    public void GoToKnotAt(string pathString)
    {
        stateMachine.ChangeState(State.LOAD);
        currentKnot = new InkyKnot(currentStory, pathString);
    }

    public void ContinueStory()
    {
        stateMachine.ChangeState(State.LOAD);
        if (currentKnot != null)
        {
            currentKnot.ContinueKnot();
        }
    }
}




