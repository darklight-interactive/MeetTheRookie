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
    ROY_RODGERSON,
    JENNY,
    CALVIN,
    JOSH,
    IRENE,
    JENKINS
}

public enum MTRMystery
{
    UNKNOWN,
    MYSTERY_0,
    MYSTERY_1,
    MYSTERY_2,
    MYSTERY_3
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
    [SerializeField]
    StoryVariableContainer _speakerVariable;

    [SerializeField, ShowOnly]
    MTRSpeaker _currentSpeaker;

    #region ( Quest Fields ) ------------------------ >>
    [SerializeField, ShowOnly]
    private string _mainQuestName;

    [SerializeField]
    StoryVariableContainer _activeQuests;

    [SerializeField]
    StoryVariableContainer _completedQuests;
    #endregion

    #region ( Clue Fields ) ------------------------ >>
    [SerializeField]
    StoryVariableContainer _globalKnowledge;

    [SerializeField]
    StoryVariableContainer _mystery0Clues;

    [SerializeField]
    StoryVariableContainer _mystery1Clues;

    [SerializeField]
    StoryVariableContainer _mystery2Clues;

    [SerializeField]
    StoryVariableContainer _mystery3Clues;

    #endregion


    public List<string> SceneKnotList
    {
        get
        {
            TryGetKnotsWithSubstring("scene", out List<string> knots);
            return knots;
        }
    }

    public List<string> ActiveQuestList
    {
        get { return _activeQuests.ValueAsStringList; }
    }

    public List<string> CompletedQuestList
    {
        get { return _completedQuests.ValueAsStringList; }
    }

    public delegate void SpeakerUpdateEvent(MTRSpeaker speaker);
    public delegate void StringUpdateEvent(string str);
    public delegate void ListUpdateEvent(List<string> list);
    public delegate void MysteryKnowledgeUpdateEvent(int mysteryIndex);
    public static event SpeakerUpdateEvent OnNewSpeaker;
    public static event ListUpdateEvent OnActiveQuestListUpdate;
    public static event ListUpdateEvent OnCompletedQuestListUpdate;
    public static event MysteryKnowledgeUpdateEvent OnMysteryKnowledgeUpdate;
    public static event ListUpdateEvent OnGlobalKnowledgeUpdate;
    public static event StringUpdateEvent OnRequestSpecialUI;

    protected override void HandleStoryInitialized()
    {
        base.HandleStoryInitialized();

        if (!Application.isPlaying)
            return;

        if (!GlobalStory.HasFunction("PlaySpecialAnimation"))
        {
            GlobalStory.BindExternalFunction(
                "PlaySpecialAnimation",
                (string speaker) =>
                {
                    MTRSpeaker speakerEnum = GetSpeaker(speaker);
                    MTRGameManager.Instance.PlaySpecialAnimation(speakerEnum);
                }
            );
            Debug.Log($"{Prefix} >> BOUND 'PlaySpecialAnimation' to external function.");
        }

        if (!GlobalStory.HasFunction("PlaySFX"))
        {
            GlobalStory.BindExternalFunction(
                "PlaySFX",
                (string sfx) =>
                {
                    MTR_AudioManager.Instance.PlayOneShotByPath(sfx);
                }
            );
            Debug.Log($"{Prefix} >> BOUND 'PlaySFX' to external function.");
        }

        if (!GlobalStory.HasFunction("PlayLoopingSFX"))
        {
            GlobalStory.BindExternalFunction(
                "PlayLoopingSFX",
                (string sfx) =>
                {
                    MTR_AudioManager.Instance.StartRepeatSFXByPath(sfx);
                }
            );
            Debug.Log($"{Prefix} >> BOUND 'PlayLoopingSFX' to external function.");
        }

        if (!GlobalStory.HasFunction("StopLoopingSFX"))
        {
            GlobalStory.BindExternalFunction(
                "StopLoopingSFX",
                (string sfx) =>
                {
                    MTR_AudioManager.Instance.StopRepeatSFXByPath(sfx);
                }
            );
            Debug.Log($"{Prefix} >> BOUND 'StopLoopingSFX' to external function.");
        }

        GlobalStory.BindExternalFunction(
            "SetSpeaker",
            (string speaker) =>
            {
                SetSpeaker(speaker);
            }
        );
        Debug.Log($"{Prefix} >> BOUND 'SetSpeaker' to external function.");

        GlobalStory.BindExternalFunction(
            "DiscoverMystery",
            (int mystery) =>
            {
                OnMysteryKnowledgeUpdate?.Invoke(mystery);
                Debug.Log($"{Prefix} >> DiscoverMystery: {mystery}");
            }
        );

        GlobalStory.BindExternalFunction(
            "RequestSpecialUI",
            (string ui) =>
            {
                OnRequestSpecialUI?.Invoke(ui);
                Debug.Log($"{Prefix} >> RequestSpecialUI: {ui}");
            }
        );

        // << OBSERVE VARIABLES >> ------------------------ >>
        GlobalStory.ObserveVariable(
            "CURRENT_SPEAKER",
            (string varName, object newValue) =>
            {
                SetSpeaker(newValue.ToString());
            }
        );

        GlobalStory.ObserveVariable(
            "MAIN_QUEST",
            (string varName, object newValue) =>
            {
                _mainQuestName = newValue.ToString();
                Debug.Log($"{Prefix} >> Main Quest: {_mainQuestName}");
            }
        );

        GlobalStory.ObserveVariable(
            "ACTIVE_QUESTS",
            (string varName, object newValue) =>
            {
                SetVariable(varName, newValue);
                TryGetVariableContainer(varName, out _activeQuests);
                OnActiveQuestListUpdate?.Invoke(_activeQuests.ValueAsStringList);
                Debug.Log($"{Prefix} >> Active Quest Chain: {_activeQuests.Value}");
            }
        );

        GlobalStory.ObserveVariable(
            "COMPLETED_QUESTS",
            (string varName, object newValue) =>
            {
                SetVariable(varName, newValue);
                TryGetVariableContainer(varName, out _completedQuests);
                OnCompletedQuestListUpdate?.Invoke(_completedQuests.ValueAsStringList);
                Debug.Log($"{Prefix} >> Completed Quest Chain: {_completedQuests.Value}");
            }
        );

        GlobalStory.ObserveVariable(
            "GLOBAL_KNOWLEDGE",
            (string varName, object newValue) =>
            {
                SetVariable(varName, newValue);
                TryGetVariableContainer(varName, out _globalKnowledge);
                OnGlobalKnowledgeUpdate?.Invoke(_globalKnowledge.ValueAsStringList);
                Debug.Log($"{Prefix} >> Global Knowledge: {_globalKnowledge.Value}");
            }
        );

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
            //Debug.Log($"{Prefix} >> Speaker List: {speakerVar}");
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

        // << INITIALIZE CLUES >> ------------------------ >>
        TryGetVariableContainer("Mystery0", out _mystery0Clues);
        TryGetVariableContainer("Mystery1", out _mystery1Clues);
        TryGetVariableContainer("Mystery2", out _mystery2Clues);
        TryGetVariableContainer("Mystery3", out _mystery3Clues);
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

    public static List<string> GetClueList(MTRMystery mystery)
    {
        switch (mystery)
        {
            case MTRMystery.MYSTERY_0:
                return Instance._mystery0Clues.ValueAsStringList;
            case MTRMystery.MYSTERY_1:
                return Instance._mystery1Clues.ValueAsStringList;
            case MTRMystery.MYSTERY_2:
                return Instance._mystery2Clues.ValueAsStringList;
            case MTRMystery.MYSTERY_3:
                return Instance._mystery3Clues.ValueAsStringList;
            default:
                return new List<string>();
        }
    }
}
