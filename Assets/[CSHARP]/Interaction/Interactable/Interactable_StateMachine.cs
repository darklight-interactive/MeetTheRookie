using System;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEditorInternal;
using UnityEngine;


public partial class Interactable
{
    [Serializable]
    public class StateMachine : FiniteStateMachine<IInteractable.State>
    {
        Interactable _interactable;
        InkyStoryIterator _storyIterator;

        [Header("State Machine")]
        [SerializeField, ShowOnly] IInteractable.State _currentState;

        public StateMachine(Interactable interactable) : base()
        {
            _interactable = interactable;
            _storyIterator = InkyStoryManager.Iterator;

            AddState(new NullState(this));
            AddState(new ReadyState(this));
            AddState(new TargetState(this));
            AddState(new StartState(this));
            AddState(new ContinueState(this));
            AddState(new CompleteState(this));
            AddState(new DisabledState(this));

            // Set the initial state
            GoToState(IInteractable.State.NULL);
        }

        #region ---- <ABSTRACT_CLASS> [[ InteractState ]] ------------------------------------ >>>>
        public abstract class BaseInteractState : FiniteState<IInteractable.State>
        {
            // Protected reference to the Interactable for inherited states to use
            protected Interactable interactable;
            protected StateMachine stateMachine;
            protected InkyStoryIterator storyIterator => InkyStoryManager.Iterator;
            public BaseInteractState(StateMachine stateMachine, IInteractable.State stateType) : base(stateMachine, stateType)
            {
                this.stateMachine = stateMachine;

                // Set the interactable reference
                // This can be done here because this class is nested within the InteractableStateMachine
                interactable = stateMachine._interactable;
            }
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
            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ TARGET_STATE ]] ------------------------------------ >>>>
        public class TargetState : BaseInteractState
        {
            public TargetState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.TARGET) { }
            public override void Enter()
            {
                interactable._iconHandler?.ShowInteractIcon();
            }
            public override void Execute() { }
            public override void Exit()
            {
                interactable._iconHandler?.HideInteractIcon();
            }
        }
        #endregion

        #region ---- <STATE_CLASS> [[ START_STATE ]] ------------------------------------ >>>>
        public class StartState : BaseInteractState
        {
            public StartState(StateMachine stateMachine) : base(stateMachine, IInteractable.State.START) { }
            public override void Enter()
            {
                storyIterator.GoToKnotOrStitch(interactable.InteractionStitch);
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
                storyIterator.ContinueStory();
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();

                InkyStoryIterator.State storyState = storyIterator.CurrentState;
                switch (storyState)
                {
                    case InkyStoryIterator.State.CHOICE:
                        Debug.Log($"INTERACTABLE :: {interactable.Name} >> Choices Found");
                        break;
                    case InkyStoryIterator.State.END:
                        stateMachine.GoToState(IInteractable.State.COMPLETE);
                        break;
                    default:
                        Debug.Log($"INTERACTABLE :: {interactable.Name} >> Continue Interaction");
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
            public override void Enter() { }
            public override void Execute() { }
            public override void Exit() { }
        }
        #endregion
    }
}