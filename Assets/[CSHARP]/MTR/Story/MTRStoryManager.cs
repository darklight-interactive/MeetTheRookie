using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

public class MTRStoryManager : InkyStoryManager
{
    public static new MTRStoryManager Instance => (MTRStoryManager)InkyStoryManager.Instance;
    public static string CurrentSpeaker => Instance._currentSpeaker;
    public static bool IsReady;

    [Header("MTR Speaker Variable")]
    [SerializeField] StoryVariableContainer _speakerVariable;
    [SerializeField, ShowOnly] string _currentSpeaker;

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

    public List<string> SpeakerList
    {
        get
        {
            return _speakerVariable.ValueAsStringList;
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
                MTR_AudioManager.Instance.PlayOneShotByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlaySFX' to external function.");
        }

        if (!MTRStoryManager.GlobalStory.HasFunction("PlayLoopingSFX"))
        {
            MTRStoryManager.GlobalStory.BindExternalFunction("PlayLoopingSFX", (string sfx) =>
            {
                MTR_AudioManager.Instance.StartRepeatSFXByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'PlayLoopingSFX' to external function.");
        }

        if (!MTRStoryManager.GlobalStory.HasFunction("StopLoopingSFX"))
        {
            MTRStoryManager.GlobalStory.BindExternalFunction("StopLoopingSFX", (string sfx) =>
            {
                MTR_AudioManager.Instance.StopRepeatSFXByPath(sfx);
            });
            Debug.Log($"{Prefix} >> BOUND 'StopLoopingSFX' to external function.");
        }

        MTRStoryManager.GlobalStory.BindExternalFunction("SetSpeaker", (string speaker) =>
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
            Debug.LogError($"{Prefix} >> Speaker List not found.");
            IsInitialized = false;
        }

        // << INITIALIZE CURRENT SPEAKER >> ------------------------ >>
        TryGetVariableValue("CURRENT_SPEAKER", out object currentSpeaker);
        if (currentSpeaker != null)
            SetSpeaker(currentSpeaker.ToString());
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