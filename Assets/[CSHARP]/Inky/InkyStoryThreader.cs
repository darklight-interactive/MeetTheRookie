using Darklight;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public enum MainStoryState { BEGIN, DIALOGUE, CHOICE, END }


/// <summary>
///  This class acts as a singleton thread 
/// </summary>
public class InkyStoryThreader : ISingleton<InkyStoryThreader>
{
    protected InkyStoryStateMachine stateMachine = new InkyStoryStateMachine(MainStoryState.BEGIN);

    #region == [[ STATIC METHODS ]] ===================== >>
    public static string Prefix => ISingleton<InkyStoryThreader>.Prefix;
    public static InkyStoryThreader Instance => ISingleton<InkyStoryThreader>.Instance;
    public static InkyStoryStateMachine StateMachine => Instance.stateMachine;
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
    private const string PATH = "Inky/";
    private const string SPEAKER_TAG = "speaker";

    protected Story currentStory;
    public void StartThread()
    {
        currentStory = LoadStory("npc_prototype");
    }

    public bool ContinueThread()
    {
        if (currentStory.canContinue)
        {
            string storyText = currentStory.Continue();
            InkyDialogue dialogue = new InkyDialogue(storyText);
            Debug.Log($"[Inky] {dialogue.speakerName}: {dialogue.textBody}");
        }

        return currentStory.canContinue;
    }
}




