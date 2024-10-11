using UnityEngine;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Inky;
using Ink.Runtime;
using System.Collections;

public partial class MTRInteractable
{
    /// <summary>
    /// Interactable Internal State Machine <br/>
    /// This class hanndles the functions and events of the Interactable class
    /// </summary>
    public new class InternalStateMachine : Interactable<InternalData, InternalStateMachine, State, Type>.InternalStateMachine
    {
        MTRInteractable _interactable;
        public InternalStateMachine(MTRInteractable interactable) : base(interactable)
        {
            _interactable = interactable;

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

        protected void TryGetDialogueReciever(string speakerName, out MTRDialogueReciever reciever)
        {
            reciever = null;

            InteractionSystem.Registry.Interactables.TryGetValue(speakerName, out MTRCharacterInteractable character);
            if (character == null)
                Debug.LogError($"{PREFIX} :: Could not find character with speaker tag: {speakerName}");

            if (character != null)
                character.Recievers.TryGetValue(InteractionType.DIALOGUE, out reciever);

            if (reciever == null)
                Debug.LogError($"{PREFIX} :: Could not find dialogue reciever for character: {speakerName}");
            return;
        }

        #region ---- <ABSTRACT_STATE_CLASS> [[ BaseInteractState ]] ------------------------------------ >>>>
        public abstract class BaseInteractState : FiniteState<State>
        {
            // Protected reference to the Interactable for inherited states to use
            protected MTRInteractable interactable;
            protected InternalStateMachine stateMachine;
            protected State stateType;

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
            MTRTargetIconReciever reciever;

            public TargetState(InternalStateMachine stateMachine)
                : base(stateMachine, State.TARGET) { }
            public override void Enter()
            {
                interactable.Recievers.TryGetValue(InteractionType.TARGET, out MTRTargetIconReciever targetReciever);
                this.reciever = targetReciever;

                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));

                interactable.spriteRenderer.material = MTRGameManager.PrefabLibrary.spriteOutlineMaterial;

                Debug.Log($"{PREFIX} :: {interactable.Print()} >> Entered Target State >> Reciever: {reciever}");
            }

            public override void Execute() { }
            public override void Exit()
            {
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));

                interactable.spriteRenderer.material = MTRGameManager.PrefabLibrary.spriteDefaultMaterial;

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
                interactable.StartCoroutine(StartInteractionRoutine());
            }
            public override void Execute() { }
            public override void Exit() { }

            IEnumerator StartInteractionRoutine()
            {
                MTRPlayerInteractor player = MTRInteractionSystem.PlayerInteractor;
                interactable.Recievers.TryGetValue(InteractionType.DESTINATION, out MTRDestinationReciever destinationReciever);
                if (destinationReciever != null)
                {
                    destinationReciever.GetClosestDestination(player.transform.position, out Vector2 destination);
                    player.Controller.StartWalkOverride(destination.x);

                }
                else
                {
                    player.Controller.EnterInteraction();
                }

                yield return new WaitUntil(() => player.Controller.CurrentState == MTRPlayerState.INTERACTION);
                MTRStoryManager.GoToKnotOrStitch(interactable._interactionStitch);
                MTR_AudioManager.Instance.PlayStartInteractionEvent();
                stateMachine.GoToState(State.CONTINUE);
            }
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
                stateMachine.TryGetDialogueReciever(MTRStoryManager.CurrentSpeaker,
                    out MTRDialogueReciever speakerDialogueReciever);

                if (speakerDialogueReciever.IsInDialogue)
                {
                    speakerDialogueReciever.ForceComplete();
                    Debug.Log($"{PREFIX} :: {interactable.Name} >> Speaker is in dialogue, forcing complete.");
                    return;
                }

                // << CONTINUE THE STORY >>
                MTRStoryManager.ContinueStory();
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();


                MTRStoryManager.StoryState storyState = MTRStoryManager.CurrentState;
                Debug.Log($"{PREFIX} :: {interactable.Key} >> Continue >> InkyStoryState.{storyState}");

                string text = MTRStoryManager.CurrentDialogue;
                switch (storyState)
                {
                    case MTRStoryManager.StoryState.DIALOGUE:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(MTRStoryManager.CurrentSpeaker,
                            out MTRDialogueReciever dialogueReciever);


                        // << INVOKE BUBBLE CREATION EVENT >> -----------
                        InteractionSystem.Invoke(new DialogueInteractionCommand(dialogueReciever, text));
                        break;

                    case MTRStoryManager.StoryState.CHOICE:
                        // << GET THE CHOICE RECIEVER >> -----------
                        MTRInteractionSystem.PlayerInteractor.Recievers.TryGetValue(InteractionType.CHOICE, out MTRChoiceReciever choiceReciever);

                        if (choiceReciever.ChoiceSelected)
                        {
                            choiceReciever.ConfirmChoice();
                            stateMachine.GoToState(State.CONTINUE, true);
                        }
                        else
                        {
                            // << INVOKE THE CHOICE EVENT >> -----------
                            InteractionSystem.Invoke(new ChoiceInteractionCommand(choiceReciever, MTRStoryManager.CurrentChoices));
                        }
                        break;
                    case MTRStoryManager.StoryState.END:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(MTRStoryManager.CurrentSpeaker,
                            out MTRDialogueReciever endDialogueReciever);

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
                stateMachine.TryGetDialogueReciever(MTRStoryManager.CurrentSpeaker,
                    out MTRDialogueReciever endDialogueReciever);

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
                MTRSceneController.Instance.CameraController.SetPlayerAsFollowTarget();
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