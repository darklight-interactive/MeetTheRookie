using System;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEditorInternal;
using UnityEngine;


public partial class Interactable
{

    /// <summary>
    /// Interactable Internal State Machine <br/>
    /// This class hanndles the functions and events of the Interactable class
    /// </summary>
    protected class StateMachine : FiniteStateMachine<IInteractable.State>
    {
        Interactable _interactable;
        InkyStoryIterator _storyIterator;
        IInteractable.State _currentState;

        public StateMachine(Interactable interactable) : base()
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
            GoToState(IInteractable.State.NULL);
        }

        void InvokeStateEvent(IInteractable.State state)
        {
            switch (state)
            {
                case IInteractable.State.READY:
                    _interactable.OnReadyEvent.Invoke();
                    break;
                case IInteractable.State.TARGET:
                    _interactable.OnTargetEvent.Invoke();
                    break;
                case IInteractable.State.START:
                    _interactable.OnStartEvent.Invoke();
                    break;
                case IInteractable.State.CONTINUE:
                    _interactable.OnContinueEvent.Invoke();
                    break;
                case IInteractable.State.COMPLETE:
                    _interactable.OnCompleteEvent.Invoke();
                    break;
                case IInteractable.State.DISABLED:
                    _interactable.OnDisabledEvent.Invoke();
                    break;
                default:
                    break;
            }
        }

        #region ---- <ABSTRACT_STATE_CLASS> [[ BaseInteractState ]] ------------------------------------ >>>>
        public abstract class BaseInteractState : FiniteState<IInteractable.State>
        {
            // Protected reference to the Interactable for inherited states to use
            protected Interactable interactable;
            protected StateMachine stateMachine;
            protected IInteractable.State stateType;
            protected InkyStoryIterator storyIterator => InkyStoryManager.Iterator;
            public BaseInteractState(StateMachine stateMachine, IInteractable.State stateType)
                : base(stateMachine, stateType)
            {
                this.stateMachine = stateMachine;
                this.stateType = stateType;


                // Set the interactable reference
                // This can be done here because this class is nested within the InteractableStateMachine
                interactable = stateMachine._interactable;
            }

            public override void Enter()
            {
                stateMachine.InvokeStateEvent(stateType);
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ NULL_STATE ]] ------------------------------------ >>>>
        public class NullState : BaseInteractState
        {
            public NullState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.NULL) { }
            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ READY_STATE ]] ------------------------------------ >>>>
        public class ReadyState : BaseInteractState
        {
            public ReadyState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.READY) { }
            public override void Enter()
            {
                base.Enter();
            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ TARGET_STATE ]] ------------------------------------ >>>>
        public class TargetState : BaseInteractState
        {
            public TargetState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.TARGET) { }
            public override void Enter()
            {
                base.Enter();

                interactable._recievers.TryGetValue(InteractionTypeKey.TARGET, out TargetInteractionReciever reciever);
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));
            }

            public override void Execute() { }
            public override void Exit()
            {
                interactable._recievers.TryGetValue(InteractionTypeKey.TARGET, out TargetInteractionReciever reciever);
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));
            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ START_STATE ]] ------------------------------------ >>>>
        public class StartState : BaseInteractState
        {
            public StartState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.START) { }
            public override void Enter()
            {
                base.Enter();
                storyIterator.GoToKnotOrStitch(interactable.Key);
                MTR_AudioManager.Instance.PlayStartInteractionEvent();
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ CONTINUE_STATE ]] ------------------------------------ >>>>
        public class ContinueState : BaseInteractState
        {
            public ContinueState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.CONTINUE) { }
            public override void Enter()
            {
                base.Enter();

                storyIterator.ContinueStory();
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();

                InkyStoryIterator.State storyState = storyIterator.CurrentState;
                Debug.Log($"{PREFIX} :: {interactable.Name} >> Continue >> InkyStory,State.{storyState}");

                string text = storyIterator.CurrentStoryDialogue;
                DialogueInteractionReciever playerDialogueReciever = InteractionSystem.Registry.PlayerInteractor.DialogueReciever;

                switch (storyState)
                {
                    case InkyStoryIterator.State.DIALOGUE:
                        InteractionSystem.Invoke(new DialogueInteractionCommand(playerDialogueReciever, text));
                        break;
                    case InkyStoryIterator.State.CHOICE:
                        break;
                    case InkyStoryIterator.State.END:
                        InteractionSystem.Invoke(new DialogueInteractionCommand(playerDialogueReciever, true));
                        stateMachine.GoToState(IInteractable.State.COMPLETE);
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
            public CompleteState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.COMPLETE) { }
            public override void Enter()
            {
                base.Enter();
                MTR_AudioManager.Instance.PlayEndInteractionEvent();
            }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ DISABLED_STATE ]] ------------------------------------ >>>>
        public class DisabledState : BaseInteractState
        {
            public DisabledState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.DISABLED) { }
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