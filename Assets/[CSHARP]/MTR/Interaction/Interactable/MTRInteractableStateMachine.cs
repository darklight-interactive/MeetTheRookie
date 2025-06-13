using System.Collections;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Inky;
using Ink.Runtime;
using UnityEngine;

public partial class MTRInteractable
{
    /// <summary>
    /// Interactable Internal State Machine <br/>
    /// This class hanndles the functions and events of the Interactable class
    /// </summary>
    public new class InternalStateMachine
        : Interactable<InternalData, InternalStateMachine, State, Type>.InternalStateMachine
    {
        MTRInteractable _interactable;

        public InternalStateMachine(MTRInteractable interactable)
            : base(interactable)
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

        /// <summary>
        /// Try to get the dialogue reciever for the given speaker
        /// </summary>
        /// <param name="speaker">The speaker to get the dialogue reciever for</param>
        /// <param name="reciever">The dialogue reciever for the given speaker</param>
        protected void TryGetDialogueReciever(MTRSpeaker speaker, out MTRDialogueReciever reciever)
        {
            reciever = null;

            if (speaker == MTRSpeaker.LUPE)
            {
                MTRInteractionSystem.PlayerInteractor.Recievers.TryGetValue(
                    InteractionType.DIALOGUE,
                    out reciever
                );
            }
            else
            {
                // << FIND THE CHARACTER >>
                bool found = false;
                Debug.Log(
                    $"{PREFIX} :: Trying to find dialogue reciever for {speaker} in {InteractionSystem.Registry.Interactables.Count} interactables. \n{string.Join("\n", InteractionSystem.Registry.Interactables.Keys)}\n"
                );
                foreach (var interactable in InteractionSystem.Registry.Interactables)
                {
                    // check if the interactable is a character & match the speaker tag
                    if (
                        interactable.Value is MTRCharacterInteractable character
                        && character.SpeakerTag == speaker
                    )
                    {
                        character.GenerateRecievers(); // Confirm the recievers are generated

                        // output the dialogue reciever
                        character.Recievers.TryGetValue(InteractionType.DIALOGUE, out reciever);
                        found = true;
                        break;
                    }
                }

                // << LOG ERRORS >>
                if (!found)
                    Debug.LogError(
                        $"{PREFIX} :: Could not find character with speaker tag: {speaker}"
                    );

                if (reciever == null)
                    Debug.LogError(
                        $"{PREFIX} :: Could not find dialogue reciever for character: {speaker}"
                    );
            }
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
            public NullState(InternalStateMachine stateMachine)
                : base(stateMachine, State.NULL) { }

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
                interactable.Recievers.TryGetValue(
                    InteractionType.TARGET,
                    out MTRTargetIconReciever targetReciever
                );
                this.reciever = targetReciever;

                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));

                interactable.spriteRenderer.material = MTRGameManager
                    .PrefabLibrary
                    .spriteOutlineMaterial;

                //Debug.Log($"{PREFIX} :: {interactable.Print()} >> Entered Target State >> Reciever: {reciever}");
            }

            public override void Execute() { }

            public override void Exit()
            {
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));

                interactable.spriteRenderer.material = MTRGameManager
                    .PrefabLibrary
                    .spriteDefaultMaterial;

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
                interactable.Recievers.TryGetValue(
                    InteractionType.DESTINATION,
                    out MTRDestinationReciever destinationReciever
                );
                if (destinationReciever != null && interactable._allowWalkToDestination)
                {
                    destinationReciever.GetClosestValidDestination(
                        player.transform.position,
                        out Vector2 destination
                    );
                    player.Controller.StartWalkOverride(destination.x);

                    Debug.Log(
                        $"{PREFIX} :: {interactable.Name} >> Starting walk override to {destination}"
                    );
                }
                else
                {
                    player.Controller.StateMachine.GoToState(MTRPlayerState.INTERACTION);
                }

                yield return new WaitUntil(
                    () => player.Controller.CurrentState == MTRPlayerState.INTERACTION
                );
                MTRStoryManager.GoToPath(interactable.InteractionStitch);
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
                stateMachine.TryGetDialogueReciever(
                    MTRStoryManager.CurrentSpeaker,
                    out MTRDialogueReciever speakerDialogueReciever
                );

                if (speakerDialogueReciever != null && speakerDialogueReciever.IsInDialogue)
                {
                    speakerDialogueReciever.ForceComplete();
                    Debug.Log(
                        $"{PREFIX} :: {interactable.Name} >> Speaker is in dialogue, forcing complete."
                    );
                    return;
                }

                // << CONTINUE THE STORY >>
                MTRStoryManager.ContinueStory();
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();

                MTRStoryManager.StoryState storyState = MTRStoryManager.CurrentState;
                Debug.Log(
                    $"{PREFIX} :: {interactable.Key} >> Continue >> InkyStoryState.{storyState}"
                );

                string text = MTRStoryManager.CurrentDialogue;
                switch (storyState)
                {
                    case MTRStoryManager.StoryState.DIALOGUE:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(
                            MTRStoryManager.CurrentSpeaker,
                            out MTRDialogueReciever dialogueReciever
                        );

                        // << INVOKE BUBBLE CREATION EVENT >> -----------
                        InteractionSystem.Invoke(
                            new DialogueInteractionCommand(dialogueReciever, text)
                        );
                        break;

                    case MTRStoryManager.StoryState.CHOICE:
                        // << GET THE CHOICE RECIEVER >> -----------
                        MTRInteractionSystem.PlayerInteractor.Recievers.TryGetValue(
                            InteractionType.CHOICE,
                            out MTRChoiceReciever choiceReciever
                        );

                        if (choiceReciever.ChoiceSelected)
                        {
                            choiceReciever.ConfirmChoice();
                            stateMachine.GoToState(State.CONTINUE, true);
                        }
                        else
                        {
                            // << INVOKE THE CHOICE EVENT >> -----------
                            InteractionSystem.Invoke(
                                new ChoiceInteractionCommand(
                                    choiceReciever,
                                    MTRStoryManager.CurrentChoices
                                )
                            );
                        }
                        break;
                    case MTRStoryManager.StoryState.END:
                        // << GET THE DIALOGUE RECIEVER OF THE SPEAKER >>
                        stateMachine.TryGetDialogueReciever(
                            MTRStoryManager.CurrentSpeaker,
                            out MTRDialogueReciever endDialogueReciever
                        );

                        InteractionSystem.Invoke(
                            new DialogueInteractionCommand(endDialogueReciever, true)
                        );
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
                stateMachine.TryGetDialogueReciever(
                    MTRStoryManager.CurrentSpeaker,
                    out MTRDialogueReciever endDialogueReciever
                );

                if (endDialogueReciever != null && endDialogueReciever.IsInDialogue)
                {
                    //endDialogueReciever.ForceComplete();
                    Debug.Log(
                        $"{PREFIX} :: {interactable.Name} >> Speaker is in dialogue, forcing complete."
                    );
                    return;
                }
                else
                {
                    // Destroy the bubble on exit
                    InteractionSystem.Invoke(
                        new DialogueInteractionCommand(endDialogueReciever, true)
                    );
                }
            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ COMPLETE_STATE ]] ------------------------------------ >>>>
        public class CompleteState : BaseInteractState
        {
            public CompleteState(InternalStateMachine stateMachine)
                : base(stateMachine, State.COMPLETE) { }

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
            public DisabledState(InternalStateMachine stateMachine)
                : base(stateMachine, State.DISABLED) { }

            public override void Enter()
            {
                base.Enter();
                interactable.SetColliderEnabled(false);
            }

            public override void Execute() { }

            public override void Exit()
            {
                base.Exit();
                interactable.SetColliderEnabled(true);
            }
        }
        #endregion
    }
}
