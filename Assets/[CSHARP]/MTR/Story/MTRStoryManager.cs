using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

public enum MTRSpeaker
{
    UNKNOWN,
    LUPE,
    MISRA,
    CHIEF_THELTON,
    MARLOWE,
    BETH,
    MEL,
    ROY_RODGERS,
    JENNY,
    CALVIN,
    JOSH,
    IRENE,
    JENKINS
}

public class MTRStoryManager : InkyStoryManager
{
    public static new MTRStoryManager Instance => (MTRStoryManager)InkyStoryManager.Instance;
    public static MTRSpeaker CurrentSpeaker 
    {
        get => Instance._currentSpeaker;
        set => Instance._currentSpeaker = value;
    }
    public static bool IsReady;

    [Header("MTR Speaker Variable")]
    [SerializeField] StoryVariableContainer _speakerVariable;
    [SerializeField, ShowOnly] MTRSpeaker _currentSpeaker;

    #region ( Quest Fields ) ------------------------ >>
    [SerializeField, ShowOnly] private string _mainQuestName;
    [SerializeField] StoryVariableContainer _activeQuestChain;
    [SerializeField] StoryVariableContainer _completedQuestChain;
    #endregion

    #region ( Clue Fields ) ------------------------ >>
    [SerializeField, ShowOnly] private List<string> _globalKnowledgeList = new List<string>();
    #endregion


    public List<string> SceneKnotList
    {
        get
        {
            TryGetKnotsWithSubstring("scene", out List<string> knots);
            return knots;
        }
    }

    public delegate void SpeakerSet(MTRSpeaker speaker);
    public static event SpeakerSet OnNewSpeaker;



    protected override void HandleStoryInitialized()
    {
        base.HandleStoryInitialized();

        if (!Application.isPlaying)
            return;

        if (!GlobalStory.HasFunction("PlaySpecialAnimation"))
        {
            GlobalStory.BindExternalFunction("PlaySpecialAnimation", (string speaker) =>
            {
                MTRSpeaker speakerEnum = GetSpeaker(speaker);
                MTRGameManager.Instance.PlaySpecialAnimation(speakerEnum);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlaySpecialAnimation' to external function.");
        }

        if (!GlobalStory.HasFunction("PlaySFX"))
        {
            GlobalStory.BindExternalFunction("PlaySFX", (string sfx) =>
            {
                MTR_AudioManager.Instance.PlayOneShotByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlaySFX' to external function.");
        }

        if (!GlobalStory.HasFunction("PlayLoopingSFX"))
        {
            GlobalStory.BindExternalFunction("PlayLoopingSFX", (string sfx) =>
            {
                MTR_AudioManager.Instance.StartRepeatSFXByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlayLoopingSFX' to external function.");
        }

        if (!GlobalStory.HasFunction("StopLoopingSFX"))
        {
            GlobalStory.BindExternalFunction("StopLoopingSFX", (string sfx) =>
            {
                MTR_AudioManager.Instance.StopRepeatSFXByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'StopLoopingSFX' to external function.");
        }

        GlobalStory.BindExternalFunction("SetSpeaker", (string speaker) =>
        {
            SetSpeaker(speaker);
        });
        Debug.Log($"{Prefix} >> BOUND 'SetSpeaker' to external function.");



        // << OBSERVE VARIABLES >> ------------------------ >>
        GlobalStory.ObserveVariable(
            "CURRENT_SPEAKER",
            (string varName, object newValue) =>
            {
                SetSpeaker(newValue.ToString());
            }
        );

        /*
        GlobalStory.ObserveVariable(
            "MAIN_QUEST",
            (string varName, object newValue) =>
            {
                _mainQuestName = newValue.ToString();
                Debug.Log($"{Prefix} >> Main Quest: {_mainQuestName}");
            }
        );

        GlobalStory.ObserveVariable(
            "ACTIVE_QUEST_CHAIN",
            (string varName, object newValue) =>
            {
                SetVariable("ACTIVE_QUEST_CHAIN", newValue);
                TryGetVariableContainer(varName, out StoryVariableContainer _activeQuestChain);
                Debug.Log($"{Prefix} >> Active Quest Chain: {_activeQuestChain.Value}");
            }
        );

        GlobalStory.ObserveVariable(
            "COMPLETED_QUESTS",
            (string varName, object newValue) =>
            {
                _completedQuestChain = TryGetVariableByName("COMPLETED_QUESTS").ToStringList();
                Debug.Log($"{Prefix} >> Completed Quest Chain: {_completedQuestChain.Count}");
            }
        );

        GlobalStory.ObserveVariable(
            "GLOBAL_KNOWLEDGE",
            (string varName, object newValue) =>
            {
                _globalKnowledgeList = TryGetVariableByName("GLOBAL_KNOWLEDGE").ToStringList();
                Debug.Log($"{Prefix} >> Global Knowledge: {_globalKnowledgeList.Count}");
            }
        );
        */

        IsReady = true;
    }

    protected override void HandleStoryDialogue(string dialogue)
    {
        base.HandleStoryDialogue(dialogue);
    }

    public override void Initialize()
    {
        base.Initialize();

        // << INITIALIZE SPEAKER VARIABLE >> ------------------------ >>
        TryGetVariableContainer("Speaker", out StoryVariableContainer speakerVar);
        if (speakerVar != null)
        {
            this._speakerVariable = speakerVar;
            Debug.Log($"{Prefix} >> Speaker List: {speakerVar.ToString()}");
        }
        else
        {
            //Debug.LogError($"{Prefix} >> Speaker List not found.");
            IsInitialized = false;
            Initialize(); // Call again
        }

        // << INITIALIZE CURRENT SPEAKER >> ------------------------ >>
        TryGetVariableValue("CURRENT_SPEAKER", out object currentSpeaker);
        if (currentSpeaker != null)
            SetSpeaker(currentSpeaker.ToString());
    }

    public static MTRSpeaker GetSpeaker(string speaker)
    {
        // Remove "Speaker." prefix if present
        speaker = speaker.Replace("Speaker.", "").ToUpper();

        // Try to parse the string to enum
        if (System.Enum.TryParse(speaker, out MTRSpeaker speakerEnum))
        {
            return speakerEnum;
        }
        return MTRSpeaker.UNKNOWN;
    }

    /// <summary>
    /// Sets the current speaker by converting the input string to its corresponding MTRSpeaker enum value
    /// </summary>
    /// <param name="speaker">Speaker name, with or without "Speaker." prefix</param>
    public static void SetSpeaker(string speaker)
    {
        MTRSpeaker speakerEnum = GetSpeaker(speaker);
        CurrentSpeaker = speakerEnum;
        OnNewSpeaker?.Invoke(speakerEnum);
    }

}