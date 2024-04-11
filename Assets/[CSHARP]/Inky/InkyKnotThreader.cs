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
    public InkyVariables currentVariables { get; private set; }
    InkyKnot currentKnot;
    public string currentText => currentStory.currentText;

    public bool LoadStory(string storyName)
    {
        stateMachine.ChangeState(State.LOAD);
        try
        {
            TextAsset storyAsset = (TextAsset)Resources.Load(PATH + storyName);
            currentStory = new Story(storyAsset.text);
            currentStory.onError += (message, type) =>
            {
                Debug.LogError("[Ink] " + type + " " + message);
            };

            // Get Variables
            InkyVariables variables = new InkyVariables(currentStory);

            // Get Tags
            List<string> tags = currentStory.globalTags.ToList();
        }
        catch (Exception e)
        {
            Debug.LogError($"{Prefix} Story Load Error: {e.Message}");
            return false;
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
            currentKnot.ContinueStory();
        }
    }
}




