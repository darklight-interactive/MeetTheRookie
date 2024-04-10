using Darklight;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;


public enum ThreadState { INIT, LOAD, DIALOGUE, CHOICE, END }
/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable data. 
/// </summary>
public class InkyKnotThreader : StateMachine<ThreadState>, ISingleton<InkyKnotThreader>
{
    private const string PATH = "Inky/";
    private const string SPEAKER_TAG = "speaker";

    #region == [[ STATIC METHODS ]] ===================== >>
    public static string Prefix => ISingleton<InkyKnotThreader>.Prefix;
    public static InkyKnotThreader Instance => ISingleton<InkyKnotThreader>.Instance;
    public static Story LoadStory(string storyName)
    {
        TextAsset storyAsset = (TextAsset)Resources.Load(PATH + storyName);
        Story story = new Story(storyAsset.text);
        story.onError += (message, type) =>
        {
            Debug.LogError("[Ink] " + type + " " + message);
        };
        return story;
    }
    #endregion

    #region == [[ CONSTRUCTORS ]] ===================== >>
    public InkyKnotThreader(ThreadState initialState = ThreadState.INIT) : base(initialState) { }
    #endregion

    Queue<InkyKnot> inkKnotThread = new Queue<InkyKnot>();
    Dictionary<string, InkyKnot> knotDictionary = new Dictionary<string, InkyKnot>();

    public Story currentStory { get; private set; }
    InkyKnot currentKnot;
    public string currentText => currentStory.currentText;
    public void LoadNewStory(string storyName = "npc_prototype")
    {
        ChangeState(ThreadState.LOAD);
        currentStory = LoadStory(storyName);
    }

    public void GoToKnotAt(string pathString)
    {
        ChangeState(ThreadState.LOAD);
        currentKnot = new InkyKnot(currentStory, pathString);
    }

    public void ContinueStory()
    {
        if (currentKnot != null)
        {
            currentKnot.ContinueStory();
        }
    }



}


