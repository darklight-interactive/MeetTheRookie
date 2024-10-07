using System;
using System.Collections.Generic;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using Ink.Runtime;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using Ink;


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
        #region ---- < STATIC_PROPERTIES > --------------------------------- 
        protected static StoryIterator Iterator
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogError("InkyStoryManager: Instance is null.");
                    return null;
                }

                if (Instance._iterator == null)
                {
                    Instance._iterator = new StoryIterator();
                }
                return Instance._iterator;
            }
        }

        public static Story GlobalStory
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogError("InkyStoryManager: Instance is null.");
                    return null;
                }

                if (Instance._story == null)
                {
                    Instance._story = CreateStory(Instance._storyAsset);
                }
                return Instance._story;
            }
        }

        public static bool IsInitialized => Instance._isInitialized;

        public static string CurrentKnot => Instance._currentStoryKnot;
        public static string CurrentDialogue
        {
            get
            {
                return GlobalStory.currentText.Trim();
            }
        }
        public static List<Choice> CurrentChoices
        {
            get
            {
                return GlobalStory.currentChoices;
            }
        }

        public static StoryState CurrentState => Iterator.CurrentState;
        #endregion

        //  ---------------- [ Private Fields ] -----------------------------
        bool _isInitialized;

        Story _story;
        StoryIterator _iterator;
        string[] _knots;
        string[] _globalTags;
        Dictionary<Choice, int> _choiceMap = new Dictionary<Choice, int>();

        //  ---------------- [ Serialized Fields ] -----------------------------
        [SerializeField] TextAsset _storyAsset;


        [Header("Active Story Info")]
        [SerializeField, ShowOnly] string _currentStoryKnot;
        [SerializeField, ShowOnly] string _currentStoryDialogue;
        [SerializeField, ShowOnly] public StoryState _currentStoryState;


        [Header("Story Data")]
        [SerializeField] List<StoryKnotContainer> _storyKnotContainers = new List<StoryKnotContainer>();
        [SerializeField] List<InkyVariable> _variables = new List<InkyVariable>();



        #region ---- < Properties > --------------------------------- 
        public List<string> KnotList
        {
            get
            {
                if (_knots == null)
                    _knots = GetAllKnots(_story).ToArray();
                return _knots.ToList();
            }
        }
        #endregion


        #region ---- < Events > --------------------------------- 


        public delegate void StorySimpleEvent();
        public delegate void StoryDialogueEvent(string text);
        public delegate void StoryChoiceEvent(List<Choice> choices);

        public static event StorySimpleEvent OnStoryInitialized;
        public static event StorySimpleEvent OnStartKnot;
        public static event StoryDialogueEvent OnNewDialogue;
        public static event StoryChoiceEvent OnNewChoices;
        public static event StorySimpleEvent OnEndKnot;
        #endregion


        //  ================================ [[ METHODS ]] ================================
        void RepopulateKnotContainers(List<string> knotList)
        {
            _storyKnotContainers.Clear();
            foreach (string knot in knotList)
            {
                _storyKnotContainers.Add(new StoryKnotContainer(knot));
            }
        }


        protected virtual void HandleStoryError(string message, ErrorType errorType)
        {
            Debug.LogError($"{Prefix} Ink Error: {errorType} :: {message}");
        }

        protected virtual void HandleStoryInitialized()
        {
            Debug.Log($"{Prefix} Story Initialized");
        }

        protected virtual void HandleStoryStart()
        {
            Debug.Log($"{Prefix} Story Started");
        }

        protected virtual void HandleStoryDialogue(string dialogue)
        {
            _currentStoryDialogue = dialogue;
            Debug.Log($"{Prefix} Dialogue: {dialogue}");
        }

        protected virtual void HandleStoryChoices(List<Choice> choices)
        {
            Debug.Log($"{Prefix} Choices: {choices.Count}");
        }

        protected virtual void HandleStoryEnd()
        {
            Debug.Log($"{Prefix} End of Knot");
        }

        // ---- ( IUnityEditorListener ) ---------------------------------
        public void OnEditorReloaded()
        {
            if (!Application.isPlaying)
                Initialize();
        }

        // ---- ( MonoBehaviourSingleton ) ---------------------------------
        public override void Initialize()
        {
            if (IsInitialized) return;

            // return if no ink story set
            if (_storyAsset == null)
            {
                Debug.LogError($"{Prefix} Ink story not set.");
                return;
            }

            // << CREATE STORY >> ------------------------------------ >>
            this._story = CreateStory(_storyAsset);
            this._knots = GetAllKnots(_story).ToArray();
            this._variables = GetAllVariables(_story);

            if (_story.globalTags != null)
                this._globalTags = _story.globalTags.ToArray();

            // << CREATE CONTAINERS >> ------------------------------------ >>
            RepopulateKnotContainers(KnotList);


            // << GET VARIABLES >>

            // << OBSERVE EVENTS >>
            if (Application.isPlaying)
            {
                _story.onError += HandleStoryError;
                OnStoryInitialized += HandleStoryInitialized;
                OnStartKnot += HandleStoryStart;
                OnNewDialogue += HandleStoryDialogue;
                OnNewChoices += HandleStoryChoices;
                OnEndKnot += HandleStoryEnd;
            }



            // << CREATE ITERATOR >> ------------------------------------ >>
            _iterator = new StoryIterator();

            // << CONFIRM INITIALIZATION >> ------------------------------------ >>
            _isInitialized = true;
            OnStoryInitialized?.Invoke();
        }



        public void TryGetKnotsWithSubstring(string substring, out List<string> knots)
        {
            substring = substring.ToLower();
            knots = new List<string>();

            if (this._knots != null)
            {
                foreach (string knot in this._knots)
                {
                    knot.ToLower();
                    if (knot.Contains(substring))
                    {
                        knots.Add(knot);
                    }
                }
            }
            return;
        }

        #region ---- < STATIC_METHODS > --------------------------------- 

        /// <summary>
        /// Creates an Ink story from a TextAsset.
        /// </summary>
        /// <param name="inkTextAsset">
        ///     The TextAsset containing the Ink story. Typically, this is a generated .json file.
        /// </param>
        static Story CreateStory(TextAsset inkTextAsset)
        {
            return new Story(inkTextAsset.text);
        }

        /// <summary>
        /// Retrieves all knots in an Ink story.
        /// </summary>
        /// <param name="story">
        ///     The Ink story from which to extract knots.
        /// </param>
        /// <returns>
        ///     A list of knot names.
        /// </returns>
        static List<string> GetAllKnots(Story story)
        {
            return story.mainContentContainer.namedContent.Keys.ToList();
        }

        /// <summary>
        /// Retrieves all variables from an Ink story and wraps them in a dictionary.
        /// </summary>
        /// <param name="story">
        ///    The Ink story from which to extract variables.
        /// </param>
        /// <returns>
        ///    A dictionary of variable names and their values.
        ///    The key is the variable name and the value is the variable value.
        ///    The variable value is an object, so it must be cast to the appropriate type.
        ///    For example, if the variable is an integer, cast it to an integer.
        /// </returns>
        static List<InkyVariable> GetAllVariables(Story story)
        {
            List<InkyVariable> output = new List<InkyVariable>();
            foreach (string variableName in story.variablesState)
            {
                object variableValue = story.variablesState[variableName];
                InkyVariable inkyVariable = new InkyVariable(variableName, variableValue);
                if (inkyVariable is null)
                {
                    Debug.LogWarning($"Variable {variableName} is null.");
                    continue;
                }
                output.Add(inkyVariable);
            }
            return output;
        }

        /// <summary>
        /// Retrieves all stitches in a knot from an Ink story.
        /// </summary>
        /// <param name="story">
        ///     The Ink story from which to extract stitches.
        /// </param>
        /// <param name="knot">
        ///     The knot from which to extract stitches.
        /// </param>
        /// <returns>
        ///     A list of stitch names.
        /// </returns>
        public static List<string> GetAllStitchesInKnot(string knot)
        {
            Container container = GlobalStory.KnotContainerWithName(knot);
            List<string> stitches = new List<string>();
            foreach (string stitch in container.namedContent.Keys.ToList())
            {
                stitches.Add($"{knot}.{stitch}");
            }
            return stitches;
        }


        static void RepopulateChoiceMap()
        {
            Instance._choiceMap.Clear();
            foreach (Choice choice in GlobalStory.currentChoices)
            {
                Instance._choiceMap.Add(choice, choice.index);
            }
        }

        public static InkyVariable GetVariableByName(string variableName)
        {
            return Instance._variables.Find(variable => variable.Key == variableName);
        }


        public static void GoToKnotOrStitch(string knotName)
        {
            try
            {
                GlobalStory.ChoosePathString(knotName);
                Instance._currentStoryKnot = knotName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{Prefix} Error: {e.Message}, this");
            }
            finally
            {
                Iterator.GoToState(StoryState.START);
                Debug.Log($"{Prefix} Moved to Knot: {knotName}");
            }
        }

        public static void ContinueStory()
        {
            Iterator.ContinueStory();
        }

        public static void ChooseChoice(Choice choice) => ChooseChoice(choice.index);
        public static void ChooseChoice(int index)
        {
            TryGetChoices(out List<Choice> choices);
            Debug.Log($"{Prefix} Choice Selected: {choices[index].text}");

            GlobalStory.ChooseChoiceIndex(index);
            Instance._choiceMap.Clear();
            ContinueStory();
        }

        public static void TryGetChoices(out List<Choice> choices)
        {
            choices = GlobalStory.currentChoices;
        }

        public static void TryGetTags(out IEnumerable<string> tags)
        {
            tags = GlobalStory.currentTags;
        }
        #endregion

        #region < CLASS > [[ STORY KNOT CONTAINER ]] ================================================================
        [Serializable]
        public class StoryKnotContainer
        {
            [SerializeField, ShowOnly] string _knot;
            [SerializeField, ShowOnly, NonReorderable] List<string> _stitches;

            public string Name { get => _knot; set => _knot = value; }
            public List<string> Stitches { get => _stitches; set => _stitches = value; }

            public StoryKnotContainer(string knot)
            {
                _knot = knot;
                _stitches = GetAllStitchesInKnot(knot);
            }
        }
        #endregion

        #region < CLASS > [[ STORY ITERATOR ]] ================================================================
        /// <summary>
        /// This class is responsible for iterating through the Ink story and handling the different states of the story.
        /// It implements a simple StoryState machine to track the current StoryState of the story.
        /// </summary>
        [Serializable]
        public class StoryIterator : SimpleStateMachine<StoryState>
        {

            // ======== [[ PROPERTIES ]] ================================ >>
            protected Story story => GlobalStory;

            // ------------------- [[ CONSTRUCTORS ]] -------------------
            public StoryIterator() : base(StoryState.NULL) { }
            public override void OnStateChanged(StoryState previousState, StoryState newState)
            {
                Instance._currentStoryState = newState;
            }

            #region ---- ( Story Handlers ) --------------------------------- 
            void HandleStoryDialogue()
            {
                GoToState(StoryState.DIALOGUE);
                story.Continue();

                // Check if empty, if so, continue again
                string currentDialogue = story.currentText.Trim();
                if (currentDialogue == null || currentDialogue == "" || currentDialogue == "\n")
                {
                    ContinueStory();
                    return;
                }

                // Invoke the Dialogue Event
                OnNewDialogue?.Invoke(currentDialogue);
            }

            void HandleStoryChoices()
            {
                // Return if already in choice StoryState
                if (CurrentState == StoryState.CHOICE) return;

                // Go To Choice StoryState
                GoToState(StoryState.CHOICE);

                // Set the choiceMap
                RepopulateChoiceMap();

                // Invoke the Choice Event
                OnNewChoices?.Invoke(story.currentChoices);
            }

            void HandleStoryEnd()
            {
                GoToState(StoryState.END);

                OnEndKnot?.Invoke();
            }

            void HandleTags()
            {
                List<string> currentTags = story.currentTags;

                // loop through each tag and handle it accordingly
                foreach (string tag in currentTags)
                {
                    // parse the tag
                    string[] splitTag = tag.Split(':');
                    if (splitTag.Length != 2)
                    {
                        Debug.LogWarning($"{Prefix} Error: Tag is not formatted correctly: {tag}");
                    }
                    else
                    {
                        string tagKey = splitTag[0].Trim();
                        string tagValue = splitTag[1].Trim();

                        // handle the tag
                        switch (tagKey)
                        {
                            default:
                                Debug.LogWarning($"{Prefix} Tag not handled: {tag}");
                                break;
                        }
                    }
                }
            }
            #endregion

            // ======== <PUBLIC_METHODS> ================================ >>


            public void ContinueStory()
            {
                // Check if null
                if (CurrentState == StoryState.NULL)
                {
                    Debug.LogError($"{Prefix} Error: Story is null");
                    return;
                }

                // Check if end
                if (CurrentState == StoryState.END)
                {
                    Debug.LogWarning($"{Prefix} Knot has ended");
                    return;
                }

                // << HANDLE STORY StoryState >> ------------------------------------ >>
                // -- ( DIALOGUE StoryState ) ----
                if (story.canContinue) HandleStoryDialogue();

                // -- ( CHOICE StoryState ) ----
                else if (story.currentChoices.Count > 0) HandleStoryChoices();

                // -- ( END StoryState ) ----
                else HandleStoryEnd();

                // << HANDLE TAGS >> ------------------------------------ >>
                HandleTags();
            }


        }
        #endregion

        public enum StoryState
        {
            NULL,
            START,
            DIALOGUE,
            CHOICE,
            END
        }

    }
}



