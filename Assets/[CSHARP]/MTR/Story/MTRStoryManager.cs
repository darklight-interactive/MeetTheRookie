using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

public class MTRStoryManager : InkyStoryManager
{
    public static new MTRStoryManager Instance => (MTRStoryManager)InkyStoryManager.Instance;
    public static string CurrentSpeaker => Instance._currentSpeaker;

    [Header("MTR Speaker Variable")]
    [SerializeField, ShowOnly, NonReorderable] List<string> _speakerList;
    [SerializeField, ShowOnly] string _currentSpeaker;

    #region ( Quest Fields ) ------------------------ >>
    [SerializeField, ShowOnly] private string _mainQuestName;
    [SerializeField, ShowOnly] private List<string> _activeQuestChain = new List<string>();
    [SerializeField, ShowOnly] private List<string> _completedQuestChain = new List<string>();
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

    public static List<string> SpeakerList
    {
        get
        {
            if (Instance._speakerList == null)
            {
                Instance._speakerList = GetVariableByName("Speaker").ToStringList();
            }
            return Instance._speakerList;
        }
    }


    public delegate void SpeakerSet(string speaker);
    public static event SpeakerSet OnNewSpeaker;

    protected override void HandleStoryInitialized()
    {
        base.HandleStoryInitialized();

        if (!Application.isPlaying)
            return;

        if (!MTRStoryManager.GlobalStory.HasFunction("PlaySpecialAnimation"))
        {
            MTRStoryManager.GlobalStory.BindExternalFunction("PlaySpecialAnimation", (string speaker) =>
            {
                MTRGameManager.Instance.PlaySpecialAnimation(speaker);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlaySpecialAnimation' to external function.");
        }

        if (!MTRStoryManager.GlobalStory.HasFunction("PlaySFX"))
        {
            MTRStoryManager.GlobalStory.BindExternalFunction("PlaySFX", (string sfx) =>
            {
                MTRGameManager.Instance.PlayInkySFX(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlaySFX' to external function.");
        }

        MTRStoryManager.GlobalStory.BindExternalFunction("SetSpeaker", (string speaker) =>
        {
            SetSpeaker(speaker);
        });
        Debug.Log($"{Prefix} >> BOUND 'SetSpeaker' to external function.");

        // << OBSERVE VARIABLES >>
        GlobalStory.ObserveVariable(
            "CURRENT_SPEAKER",
            (string varName, object newValue) =>
            {
                _currentSpeaker = newValue.ToString();
                OnNewSpeaker?.Invoke(_currentSpeaker);
                Debug.Log($"{Prefix} >> Current Speaker: {_currentSpeaker}");
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
            "ACTIVE_QUEST_CHAIN",
            (string varName, object newValue) =>
            {
                _activeQuestChain = GetVariableByName("ACTIVE_QUEST_CHAIN").ToStringList();
                Debug.Log($"{Prefix} >> Active Quest Chain: {_activeQuestChain.Count}");
            }
        );

        GlobalStory.ObserveVariable(
            "COMPLETED_QUESTS",
            (string varName, object newValue) =>
            {
                _completedQuestChain = GetVariableByName("COMPLETED_QUESTS").ToStringList();
                Debug.Log($"{Prefix} >> Completed Quest Chain: {_completedQuestChain.Count}");
            }
        );

        GlobalStory.ObserveVariable(
            "GLOBAL_KNOWLEDGE",
            (string varName, object newValue) =>
            {
                _globalKnowledgeList = GetVariableByName("GLOBAL_KNOWLEDGE").ToStringList();
                Debug.Log($"{Prefix} >> Global Knowledge: {_globalKnowledgeList.Count}");
            }
        );

        _speakerList = GetVariableByName("Speaker").ToStringList();
        SetSpeaker(GetVariableByName("CURRENT_SPEAKER").GetValueAsString());
    }

    protected override void HandleStoryDialogue(string dialogue)
    {
        base.HandleStoryDialogue(dialogue);
    }

    public override void Initialize()
    {
        base.Initialize();




    }


    /// <summary>
    /// This is the forceful way to set the speaker value.
    /// </summary>
    /// <param name="speaker"></param>
    public void SetSpeaker(string speaker)
    {
        if (!speaker.Contains("Speaker"))
            speaker = "Speaker." + speaker;

        _currentSpeaker = speaker;
        OnNewSpeaker?.Invoke(speaker);
    }
}