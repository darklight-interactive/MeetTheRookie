using System;
using System.Collections.Generic;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using Ink.Runtime;
using UnityEngine;
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
        public delegate void StoryInitialized(Story story);
        public event StoryInitialized OnStoryInitialized;

        [SerializeField]
        InkyStoryObject _globalStoryObject;

        [SerializeField, ShowOnly]
        List<string> _speakerList;

        [SerializeField, ShowOnly]
        List<string> _globalKnotList;

        [SerializeField, ShowOnly]
        string _currentSpeaker;

        // ------------------------ [[ GLOBAL STORY OBJECT ]] ------------------------ >>
        public static InkyStoryIterator Iterator { get; private set; }

        public static InkyStoryObject GlobalStoryObject
        {
            get { return Instance._globalStoryObject; }
        }

        /// <summary>
        /// List of all the speakers in the Inky Story.
        /// </summary>
        public static List<string> SpeakerList
        {
            get { return Instance._speakerList; }
        }

        /// <summary>
        /// List of all of the knot names in the Inky Story.
        /// </summary>
        public static List<string> GlobalKnotList
        {
            get { return Instance._globalKnotList; }
        }

        public static string CurrentSpeaker
        {
            get { return Instance._currentSpeaker; }
        }

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
            _globalStoryObject.Initialize(); // << INITIALIZE STORY DATA >>

            // << GET VARIABLES >>
            _speakerList = _globalStoryObject.GetVariableByName("Speaker").ToStringList();
            //Debug.Log($"{Prefix} >> Speaker List Count : {SpeakerList.Count}");

            _globalKnotList = _globalStoryObject.KnotNameList;
            //Debug.Log($"{Prefix} >> Global Knots Count : {GlobalKnots.Count}");

            // << OBSERVE VARIABLES >>
            _globalStoryObject.StoryValue.ObserveVariable(
                "CURRENT_SPEAKER",
                (string varName, object newValue) =>
                {
                    _currentSpeaker = newValue.ToString();
                    OnSpeakerSet?.Invoke(_currentSpeaker);
                    Debug.Log($"{Prefix} >> Current Speaker: {_currentSpeaker}");
                }
            );

            _globalStoryObject.StoryValue.ObserveVariable(
                "MAIN_QUEST",
                (string varName, object newValue) =>
                {
                    _mainQuestName = newValue.ToString();
                    Debug.Log($"{Prefix} >> Main Quest: {_mainQuestName}");
                }
            );

            _globalStoryObject.StoryValue.ObserveVariable(
                "ACTIVE_QUEST_CHAIN",
                (string varName, object newValue) =>
                {
                    _activeQuestChain = _globalStoryObject.GetVariableByName("ACTIVE_QUEST_CHAIN").ToStringList();
                    Debug.Log($"{Prefix} >> Active Quest Chain: {_activeQuestChain.Count}");
                }
            );

            _globalStoryObject.StoryValue.ObserveVariable(
                "COMPLETED_QUESTS",
                (string varName, object newValue) =>
                {
                    _completedQuestChain = _globalStoryObject.GetVariableByName("COMPLETED_QUESTS").ToStringList();
                    Debug.Log($"{Prefix} >> Completed Quest Chain: {_completedQuestChain.Count}");
                }
            );

            _globalStoryObject.StoryValue.ObserveVariable(
                "GLOBAL_KNOWLEDGE",
                (string varName, object newValue) =>
                {
                    _globalKnowledgeList = _globalStoryObject.GetVariableByName("GLOBAL_KNOWLEDGE").ToStringList();
                    Debug.Log($"{Prefix} >> Global Knowledge: {_globalKnowledgeList.Count}");
                }
            );



            // << INITIALIZE STORY ITERATOR >>
            Iterator = new InkyStoryIterator(_globalStoryObject);

            Debug.Log($"{Prefix} >> Initialized Inky Story Manager.");
            OnStoryInitialized?.Invoke(_globalStoryObject.StoryValue);
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
