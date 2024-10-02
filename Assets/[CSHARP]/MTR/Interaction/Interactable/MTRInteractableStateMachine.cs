using UnityEngine;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Inky;
using Ink.Runtime;

public partial class MTRInteractable
{
    /// <summary>
    /// Interactable Internal State Machine <br/>
    /// This class hanndles the functions and events of the Interactable class
    /// </summary>
    public new class InternalStateMachine : Interactable<InternalData, InternalStateMachine, State, Type>.InternalStateMachine
    {
        MTRInteractable _interactable;
        InkyStoryIterator _storyIterator;
        public InternalStateMachine(MTRInteractable interactable) : base(interactable)
        {
            _interactable = interactable;
            _storyIterator = InkyStoryManager.Iterator;

            NullState nullState = new NullState(this);
            this.currentFiniteState = nullState;

            AddState(nullState);
            AddState(new ReadyState(this));
            AddState(new TargetState(this));
            AddState(new StartState(this));
            AddState(new ContinueState(this));
            AddState(new CompleteState(this));
            AddState(new DisabledState(this));

            // Set the initial state
            GoToState(State.NULL);
        }

        protected void TryGetDialogueReciever(string speakerName, out DialogueInteractionReciever reciever)
        {
            reciever = null;

            string fullTag = $"Speaker.{speakerName}";
            InteractionSystem.Registry.Interactables.TryGetValue(fullTag, out MTRCharacterInteractable character);
            if (character == null)
                Debug.LogError($"{PREFIX} :: Could not find character with speaker tag: {fullTag}");

            if (character != null)
                character.Recievers.TryGetValue(InteractionType.DIALOGUE, out reciever);

            if (reciever == null)
                Debug.LogError($"{PREFIX} :: Could not find dialogue reciever for character: {fullTag}");
            return;
        }

        #region ---- <ABSTRACT_STATE_CLASS> [[ BaseInteractState ]] ------------------------------------ >>>>
        public abstract class BaseInteractState : FiniteState<State>
        {
            // Protected reference to the Interactable for inherited states to use
            protected MTRInteractable interactable;
            protected InternalStateMachine stateMachine;
            protected State stateType;
            protected InkyStoryIterator storyIterator => InkyStoryManager.Iterator;


            public BaseInteractState(InternalStateMachine stateMachine, State stateType)
                : base(stateMachine, stateType)
            {
                this.stateMachine = stateMachine;
                this.stateType = stateType;


                // Set the interactable reference
                // This can be done here because this class is nested within the InteractableStateMachine
                interactable = stateMachine._interactable;
            }

            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ NULL_STATE ]] ------------------------------------ >>>>
        public class NullState : BaseInteractState
        {
            public NullState(InternalStateMachine stateMachine) : base(stateMachine, State.NULL) { }
            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ READY_STATE ]] ------------------------------------ >>>>
        public class ReadyState : BaseInteractState
        {
            public ReadyState(InternalStateMachine stateMachine)
                : base(stateMachine, State.READY) { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ TARGET_STATE ]] ------------------------------------ >>>>
        public class TargetState : BaseInteractState
        {
            TargetInteractionReciever reciever;

            public TargetState(InternalStateMachine stateMachine)
                : base(stateMachine, State.TARGET) { }
            public override void Enter()
            {
                interactable.Recievers.TryGetValue(InteractionType.TARGET, out TargetInteractionReciever targetReciever);
                this.reciever = targetReciever;

                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));

                //Debug.Log($"{PREFIX} :: {interactable.Print()} >> Entered Target State >> Reciever: {reciever}");
            }

            public override void Execute() { }
            public override void Exit()
            {
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));
                //Debug.Log($"{PREFIX} :: {interactable.Print()} >> Exited Target State >> Reciever: {reciever}");
            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ START_STATE ]] ------------------------------------ >>>>
        public class StartState : BaseInteractState
        {
            public StartState(InternalStateMachine stateMachine)
                : base(stateMachine, State.START) { }
            public override void Enter()
            {
                base.Enter();
                storyIterator.GoToKnotOrStitch(interactable._interactionStitch);
                MTR_AudioManager.Instance.PlayStartInteractionEvent();
                stateMachine.GoToState(State.CONTINUE);
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ CONTINUE_STATE ]] ------------------------------------ >>>>
        public class ContinueState : BaseInteractState
        {

            public ContinueState(InternalStateMachine stateMachine)
                : base(stateMachine, State.CONTINUE) { }
            public override void Enter()
            {
                // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                stateMachine.TryGetDialogueReciever(InkyStoryManager.CurrentSpeaker,
                    out DialogueInteractionReciever speakerDialogueReciever);

                if (speakerDialogueReciever.IsInDialogue)
                {
                    speakerDialogueReciever.ForceComplete();
                    Debug.Log($"{PREFIX} :: {interactable.Name} >> Speaker is in dialogue, forcing complete.");
                    return;
                }

                // << CONTINUE THE STORY >>
                storyIterator.ContinueStory();
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();


                InkyStoryIterator.State storyState = storyIterator.CurrentState;
                Debug.Log($"{PREFIX} :: {interactable.Name} >> Continue >> InkyStoryState.{storyState}");

                string text = storyIterator.CurrentStoryDialogue;
                switch (storyState)
                {
                    case InkyStoryIterator.State.DIALOGUE:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(InkyStoryManager.CurrentSpeaker,
                            out DialogueInteractionReciever dialogueReciever);


                        // << INVOKE BUBBLE CREATION EVENT >> -----------
                        InteractionSystem.Invoke(new DialogueInteractionCommand(dialogueReciever, text));
                        break;

                    case InkyStoryIterator.State.CHOICE:
                        // << GET THE CHOICE RECIEVER >> -----------
                        MTRInteractionSystem.PlayerInteractor.Recievers.TryGetValue(InteractionType.CHOICE, out ChoiceInteractionReciever choiceReciever);

                        if (choiceReciever.ChoiceSelected)
                        {
                            choiceReciever.ConfirmChoice();
                            stateMachine.GoToState(State.CONTINUE, true);
                        }
                        else
                        {
                            // << INVOKE THE CHOICE EVENT >> -----------
                            InteractionSystem.Invoke(new ChoiceInteractionCommand(choiceReciever, storyIterator.CurrentStoryChoices));
                        }
                        break;
                    case InkyStoryIterator.State.END:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(InkyStoryManager.CurrentSpeaker,
                            out DialogueInteractionReciever endDialogueReciever);

                        InteractionSystem.Invoke(new DialogueInteractionCommand(endDialogueReciever, true));
                        stateMachine.GoToState(State.COMPLETE);
                        break;
                    default:
                        break;
                }


            }
            public override void Execute() { }
            public override void Exit()
            {
                // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                stateMachine.TryGetDialogueReciever(InkyStoryManager.CurrentSpeaker,
                    out DialogueInteractionReciever endDialogueReciever);

                if (endDialogueReciever.IsInDialogue)
                {
                    //endDialogueReciever.ForceComplete();
                    Debug.Log($"{PREFIX} :: {interactable.Name} >> Speaker is in dialogue, forcing complete.");
                    return;
                }
                else
                {

                    // Destroy the bubble on exit
                    InteractionSystem.Invoke(new DialogueInteractionCommand(endDialogueReciever, true));
                }

            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ COMPLETE_STATE ]] ------------------------------------ >>>>
        public class CompleteState : BaseInteractState
        {
            public CompleteState(InternalStateMachine stateMachine) : base(stateMachine, State.COMPLETE) { }
            public override void Enter()
            {
                base.Enter();

                //Debug.Log($"{PREFIX} :: {interactable.Name} >> Entered Complete State");
                MTR_AudioManager.Instance.PlayEndInteractionEvent();
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ DISABLED_STATE ]] ------------------------------------ >>>>
        public class DisabledState : BaseInteractState
        {
            public DisabledState(InternalStateMachine stateMachine) : base(stateMachine, State.DISABLED) { }
            public override void Enter()
            {
                base.Enter();

            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion
    }
}