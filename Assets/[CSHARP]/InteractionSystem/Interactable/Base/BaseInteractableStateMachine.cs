using System;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEditorInternal;
using UnityEngine;

public partial class BaseInteractable
{
    /// <summary>
    /// Interactable Internal State Machine <br/>
    /// This class hanndles the functions and events of the Interactable class
    /// </summary>
    public new class InternalStateMachine : Interactable<InternalData, InternalStateMachine, State, Type>.InternalStateMachine
    {
        public BaseInteractable _interactable;
        public InternalStateMachine(BaseInteractable interactable) : base(interactable)
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


        #region ---- <ABSTRACT_STATE_CLASS> [[ BaseInteractState ]] ------------------------------------ >>>>
        public abstract class BaseInteractState : FiniteState<State>
        {
            // Protected reference to the Interactable for inherited states to use
            protected BaseInteractable interactable;
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
            public TargetState(InternalStateMachine stateMachine)
                : base(stateMachine, State.TARGET) { }
            public override void Enter()
            {
                interactable.Recievers.TryGetValue(InteractionType.TARGET, out MTRTargetIconReciever reciever);
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, true));
            }

            public override void Execute() { }
            public override void Exit()
            {
                interactable.Recievers.TryGetValue(InteractionType.TARGET, out MTRTargetIconReciever reciever);
                if (reciever != null)
                    InteractionSystem.Invoke(new TargetInteractionCommand(reciever, false));
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

            }
            public override void Execute() { }
            public override void Exit() { }
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
