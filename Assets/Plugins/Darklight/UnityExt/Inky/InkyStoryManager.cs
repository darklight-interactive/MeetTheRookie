using System;
using System.Collections.Generic;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using Ink.Runtime;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Inky
{
    /// <summary>
    ///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data.
    /// </summary>
    public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>, IUnityEditorListener
    {
        public static Ink.Runtime.Story GlobalStory => Instance._globalStoryObject.StoryValue;
        public static InkyStoryObject GlobalStoryObject => Instance._globalStoryObject;
        public static InkyStoryIterator Iterator => Instance._iterator;
        public static List<string> SpeakerList
        {
            get
            {
                if (Instance._speakerList == null)
                    Instance._speakerList = new List<string>();
                return Instance._speakerList;
            }
        }
        public static List<string> KnotList
        {
            get
            {
                if (Instance._knotList == null)
                    Instance._knotList = new List<string>();
                return Instance._knotList;
            }
        }
        public static string CurrentSpeaker => Instance._currentSpeaker;

        public delegate void StoryInitialized(Story story);
        public event StoryInitialized OnStoryInitialized;

        [SerializeField, Expandable] InkyStoryObject _globalStoryObject;
        [SerializeField] InkyStoryIterator _iterator;
        [SerializeField, ShowOnly, NonReorderable] List<string> _knotList;
        [SerializeField, ShowOnly, NonReorderable] List<string> _speakerList;
        [SerializeField, ShowOnly] string _currentSpeaker;

        // ------------------------ [[ GLOBAL STORY OBJECT ]] ------------------------ >>


        // ----------- [[ STORY ITERATOR ]] ------------ >>
        #region ----- [[ SPEAKER HANDLING ]] ------------------------ >>
        public delegate void SpeakerSet(string speaker);
        public event SpeakerSet OnSpeakerSet;

        /// <summary>
        /// This is the forceful way to set the speaker value.
        /// </summary>
        /// <param name="speaker"></param>
        public void SetSpeaker(string speaker)
        {
            _currentSpeaker = speaker;
            OnSpeakerSet?.Invoke(speaker);
        }

        #endregion

        #region ----- [[ QUEST HANDLING ]] ------------------------ >>
        [SerializeField, ShowOnly] private string _mainQuestName;
        [SerializeField, ShowOnly] private List<string> _activeQuestChain = new List<string>();
        [SerializeField, ShowOnly] private List<string> _completedQuestChain = new List<string>();
        #endregion

        #region ----- [[ CLUE HANDLING ]] ------------------------ >>
        [SerializeField, ShowOnly] private List<string> _globalKnowledgeList = new List<string>();
        #endregion

        // ------------------------ [[ METHODS ]] ------------------------ >>
        public void OnEditorReloaded()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (_globalStoryObject == null)
                return;

            // << INITIALIZE STORY DATA >>
            _globalStoryObject.Initialize();
            _knotList = _globalStoryObject.KnotList;

            // << GET VARIABLES >>
            _speakerList = _globalStoryObject.GetVariableByName("Speaker").ToStringList();
            //Debug.Log($"{Prefix} >> Speaker List Count : {SpeakerList.Count}");

            Story story = _globalStoryObject.StoryValue; // << GET STORY OBJECT >>

            // << OBSERVE VARIABLES >>
            story.ObserveVariable(
                "CURRENT_SPEAKER",
                (string varName, object newValue) =>
                {
                    _currentSpeaker = newValue.ToString();
                    OnSpeakerSet?.Invoke(_currentSpeaker);
                    Debug.Log($"{Prefix} >> Current Speaker: {_currentSpeaker}");
                }
            );

            story.ObserveVariable(
                "MAIN_QUEST",
                (string varName, object newValue) =>
                {
                    _mainQuestName = newValue.ToString();
                    Debug.Log($"{Prefix} >> Main Quest: {_mainQuestName}");
                }
            );

            story.ObserveVariable(
                "ACTIVE_QUEST_CHAIN",
                (string varName, object newValue) =>
                {
                    _activeQuestChain = _globalStoryObject.GetVariableByName("ACTIVE_QUEST_CHAIN").ToStringList();
                    Debug.Log($"{Prefix} >> Active Quest Chain: {_activeQuestChain.Count}");
                }
            );

            story.ObserveVariable(
                "COMPLETED_QUESTS",
                (string varName, object newValue) =>
                {
                    _completedQuestChain = _globalStoryObject.GetVariableByName("COMPLETED_QUESTS").ToStringList();
                    Debug.Log($"{Prefix} >> Completed Quest Chain: {_completedQuestChain.Count}");
                }
            );

            story.ObserveVariable(
                "GLOBAL_KNOWLEDGE",
                (string varName, object newValue) =>
                {
                    _globalKnowledgeList = _globalStoryObject.GetVariableByName("GLOBAL_KNOWLEDGE").ToStringList();
                    Debug.Log($"{Prefix} >> Global Knowledge: {_globalKnowledgeList.Count}");
                }
            );

            // << CREATE ITERATOR >> ------------------------------------ >>
            _iterator = new InkyStoryIterator(_globalStoryObject);

            Debug.Log($"{Prefix} >> Initialized Inky Story Manager with Story: {_globalStoryObject.name} {story}");
            OnStoryInitialized?.Invoke(story);
        }

        object QuestStarted(object[] args)
        {
            Debug.Log("Quest Started! >> " + args[0].ToString());
            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InkyStoryManager))]
    public class InkyStoryManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        InkyStoryManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (InkyStoryManager)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                _script.Initialize();
            }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
