using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

public class MTRStoryManager : InkyStoryManager
{
    public static new MTRStoryManager Instance => (MTRStoryManager)InkyStoryManager.Instance;
    public static string CurrentSpeaker => Instance._currentSpeaker;


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

    public override void Initialize()
    {
        base.Initialize();

        _speakerList = GetVariableByName("Speaker").ToStringList();


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
    }


    /// <summary>
    /// This is the forceful way to set the speaker value.
    /// </summary>
    /// <param name="speaker"></param>
    public void SetSpeaker(string speaker)
    {
        _currentSpeaker = speaker;
        OnNewSpeaker?.Invoke(speaker);
    }
}