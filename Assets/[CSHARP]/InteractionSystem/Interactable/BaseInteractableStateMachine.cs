using System;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEditorInternal;
using UnityEngine;


/// <summary>
/// Interactable Internal State Machine <br/>
/// This class hanndles the functions and events of the Interactable class
/// </summary>
public class BaseInteractableStateMachine : FiniteStateMachine<BaseInteractableState>
{
    BaseInteractable _interactable;
    InkyStoryIterator _storyIterator;
    BaseInteractableState _currentState;

    public BaseInteractableStateMachine(BaseInteractable interactable) : base()
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
        GoToState(BaseInteractableState.NULL);
    }

    #region ---- <ABSTRACT_STATE_CLASS> [[ BaseInteractState ]] ------------------------------------ >>>>
    public abstract class BaseInteractState : FiniteState<BaseInteractableState>
    {
        // Protected reference to the Interactable for inherited states to use
        protected BaseInteractable interactable;
        protected BaseInteractableStateMachine stateMachine;
        protected BaseInteractableState stateType;
        protected InkyStoryIterator storyIterator => InkyStoryManager.Iterator;
        public BaseInteractState(BaseInteractableStateMachine stateMachine, BaseInteractableState stateType)
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
        public NullState(BaseInteractableStateMachine stateMachine) : base(stateMachine, BaseInteractableState.NULL) { }
        public override void Enter() { }
        public override void Execute() { }
        public override void Exit() { }
    }
    #endregion

    #region ---- <STATE_CLASS> [[ READY_STATE ]] ------------------------------------ >>>>
    public class ReadyState : BaseInteractState
    {
        public ReadyState(BaseInteractableStateMachine stateMachine)
            : base(stateMachine, BaseInteractableState.READY) { }
    }
    #endregion

    #region ---- <STATE_CLASS> [[ TARGET_STATE ]] ------------------------------------ >>>>
    public class TargetState : BaseInteractState
    {
        public TargetState(BaseInteractableStateMachine stateMachine)
            : base(stateMachine, BaseInteractableState.TARGET) { }
        public override void Enter()
        {
            interactable.Recievers.TryGetValue(InteractionType.TARGET, out TargetInteractionReciever reciever);
            if (reciever != null)
                InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));
        }

        public override void Execute() { }
        public override void Exit()
        {
            interactable.Recievers.TryGetValue(InteractionType.TARGET, out TargetInteractionReciever reciever);
            if (reciever != null)
                InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));
        }
    }
    #endregion

    #region ---- <STATE_CLASS> [[ START_STATE ]] ------------------------------------ >>>>
    public class StartState : BaseInteractState
    {
        public StartState(BaseInteractableStateMachine stateMachine)
            : base(stateMachine, BaseInteractableState.START) { }
        public override void Enter()
        {
            base.Enter();
            storyIterator.GoToKnotOrStitch(interactable.Key);
            MTR_AudioManager.Instance.PlayStartInteractionEvent();
            stateMachine.GoToState(BaseInteractableState.CONTINUE);
        }
        public override void Execute() { }
        public override void Exit() { }
    }
    #endregion

    #region ---- <STATE_CLASS> [[ CONTINUE_STATE ]] ------------------------------------ >>>>
    public class ContinueState : BaseInteractState
    {
        public ContinueState(BaseInteractableStateMachine stateMachine)
            : base(stateMachine, BaseInteractableState.CONTINUE) { }
        public override void Enter()
        {
            storyIterator.ContinueStory();
            MTR_AudioManager.Instance.PlayContinuedInteractionEvent();

            InkyStoryIterator.State storyState = storyIterator.CurrentState;
            //Debug.Log($"{PREFIX} :: {interactable.Name} >> Continue >> InkyStoryState.{storyState}");

            string text = storyIterator.CurrentStoryDialogue;

            // Get the player interactor and dialogue reciever
            InteractionSystem.Registry.Interactables.TryGetValue("Player", out BaseInteractable playerInteractor);
            playerInteractor.Recievers.TryGetValue(InteractionType.DIALOGUE, out DialogueInteractionReciever playerDialogueReciever);



            switch (storyState)
            {
                case InkyStoryIterator.State.DIALOGUE:
                    InteractionSystem.Invoke(new DialogueInteractionCommand(playerDialogueReciever, text));
                    break;
                case InkyStoryIterator.State.CHOICE:
                    break;
                case InkyStoryIterator.State.END:
                    InteractionSystem.Invoke(new DialogueInteractionCommand(playerDialogueReciever, true));
                    stateMachine.GoToState(BaseInteractableState.COMPLETE);
                    break;
                default:
                    break;
            }
        }
        public override void Execute() { }
        public override void Exit() { }
    }
    #endregion

    #region ---- <STATE_CLASS> [[ COMPLETE_STATE ]] ------------------------------------ >>>>
    public class CompleteState : BaseInteractState
    {
        public CompleteState(BaseInteractableStateMachine stateMachine) : base(stateMachine, BaseInteractableState.COMPLETE) { }
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
        public DisabledState(BaseInteractableStateMachine stateMachine) : base(stateMachine, BaseInteractableState.DISABLED) { }
        public override void Enter()
        {
            base.Enter();

        }
        public override void Execute() { }
        public override void Exit() { }
    }
    #endregion
}