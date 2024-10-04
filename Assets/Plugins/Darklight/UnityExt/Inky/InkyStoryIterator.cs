using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Behaviour;
using Ink.Runtime;
using UnityEngine;
using UnityEditorInternal;
using System.Linq;
using UnityEngine.XR;
using System;

namespace Darklight.UnityExt.Inky
{
    /// <summary>
    /// This class is responsible for iterating through the Ink story and handling the different states of the story.
    /// It implements a simple state machine to track the current state of the story.
    /// </summary>
    [Serializable]
    public class InkyStoryIterator : SimpleStateMachine<InkyStoryIterator.State>
    {
        const string PREFIX = "[InkyStoryIterator]";
        InkyStoryObject _storyObject;
        Dictionary<Choice, int> _choiceMap = new Dictionary<Choice, int>();

        [SerializeField, ShowOnly] public State _currentIteratorState;
        [SerializeField, ShowOnly] string _currentStoryKnot;
        [SerializeField, ShowOnly] string _currentStoryDialogue;
        List<Choice> _currentChoices;


        // ======== [[ PROPERTIES ]] ================================ >>
        public string CurrentStoryKnot { get => _currentStoryKnot; set => _currentStoryKnot = value; }
        public string CurrentStoryDialogue
        {
            get => _currentStoryDialogue = _storyObject.StoryValue.currentText.Trim();
        }
        public List<Choice> CurrentStoryChoices
        {
            get => _currentChoices = _choiceMap.Keys.ToList();
        }

        // ======== [[ EVENTS ]] ================================ >>
        public delegate void StoryDialogueEvent(string text, string speaker = "");
        public delegate void StoryChoiceEvent(List<Choice> choices);
        public event StoryDialogueEvent OnDialogue;
        public event StoryChoiceEvent OnChoice;

        // ------------------- [[ CONSTRUCTORS ]] -------------------
        public InkyStoryIterator(InkyStoryObject inkyStoryObject, State initialState = State.NULL)
            : base(initialState)
        {
            _storyObject = inkyStoryObject;
            GoToState(initialState);
        }

        public override void OnStateChanged(State previousState, State newState)
        {
            _currentIteratorState = newState;
        }

        // ======== <PRIVATE_METHODS> ================================ >>
        void HandleStoryDialogue(Story story)
        {
            GoToState(State.DIALOGUE);
            story.Continue();

            // Check if empty, if so, continue again
            if (CurrentStoryDialogue == null || CurrentStoryDialogue == "" || CurrentStoryDialogue == "\n")
            {
                ContinueStory();
                return;
            }

            // Invoke the Dialogue Event
            OnDialogue?.Invoke(CurrentStoryDialogue);
            Debug.Log($"{PREFIX} Dialogue: {CurrentStoryDialogue}");
        }

        void HandleStoryChoices(Story story)
        {
            // Return if already in choice state
            if (CurrentState == State.CHOICE) return;

            // Go To Choice State
            GoToState(State.CHOICE);

            // Set the choiceMap
            _choiceMap.Clear();
            foreach (Choice choice in story.currentChoices)
            {
                _choiceMap.Add(choice, choice.index);
            }

            // Invoke the Choice Event
            OnChoice?.Invoke(story.currentChoices);
            Debug.Log($"{PREFIX} Choices: {story.currentChoices.Count}");
        }

        void HandleStoryEnd()
        {
            GoToState(State.END);

            Debug.Log($"{PREFIX} End of Knot");
        }

        void HandleTags()
        {
            List<string> currentTags = _storyObject.StoryValue.currentTags;

            // loop through each tag and handle it accordingly
            foreach (string tag in currentTags)
            {
                // parse the tag
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    Debug.LogWarning($"{PREFIX} Error: Tag is not formatted correctly: {tag}");
                }
                else
                {
                    string tagKey = splitTag[0].Trim();
                    string tagValue = splitTag[1].Trim();

                    // handle the tag
                    switch (tagKey)
                    {
                        default:
                            Debug.LogWarning($"{PREFIX} Tag not handled: {tag}");
                            break;
                    }
                }
            }
        }


        // ======== <PUBLIC_METHODS> ================================ >>
        public void GoToKnotOrStitch(string knotName)
        {
            try
            {
                _storyObject.StoryValue.ChoosePathString(knotName);
                CurrentStoryKnot = knotName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{PREFIX} Error: {e.Message}, this");
            }
            finally
            {
                GoToState(State.START);
                Debug.Log($"{PREFIX} Moved to Knot: {knotName}");
            }
        }

        public void ContinueStory()
        {
            // Check if null
            if (_storyObject == null || CurrentState == State.NULL)
            {
                Debug.LogError($"{PREFIX} Error: Story is null");
                return;
            }

            // Get the story
            Story story = _storyObject.StoryValue;

            // Check if end
            if (CurrentState == State.END)
            {
                Debug.LogWarning($"{PREFIX} Knot has ended");
                return;
            }

            // << HANDLE STORY STATE >> ------------------------------------ >>
            // -- ( DIALOGUE STATE ) ----
            if (story.canContinue) HandleStoryDialogue(story);

            // -- ( CHOICE STATE ) ----
            else if (story.currentChoices.Count > 0) HandleStoryChoices(story);

            // -- ( END STATE ) ----
            else HandleStoryEnd();

            // << HANDLE TAGS >> ------------------------------------ >>
            HandleTags();
        }

        public void ChooseChoice(Choice choice)
        {
            Debug.Log($"{PREFIX} Choice Selected: {choice.text}");
            _storyObject.StoryValue.ChooseChoiceIndex(choice.index);
            _choiceMap.Clear();
            ContinueStory();
        }



        public enum State
        {
            NULL,
            START,
            DIALOGUE,
            CHOICE,
            END
        }
    }
}
