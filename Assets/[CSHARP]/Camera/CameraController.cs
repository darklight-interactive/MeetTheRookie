using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum CameraState
{
    DEFAULT,
    FOLLOW_TARGET,
    CLOSE_UP
}

[ExecuteAlways]
public class CameraController : CameraRig
{

    #region State Machine ============================================== >>>>
    public class StateMachine : FiniteStateMachine<CameraState>
    {
        public StateMachine(Dictionary<CameraState, FiniteState<CameraState>> possibleStates, CameraState initialState, params object[] args) : base(possibleStates, initialState, args) { }
    }

    /// <summary>
    /// This state is the default state for the camera. It does not follow any target and is in a fixed position.
    /// </summary>
    public class State : FiniteState<CameraState>
    {
        private float _initialFOV;
        private float _targetFOV;

        /// <param name="args">
        ///     args[0] = CameraRig ( cameraRig )
        ///     args[1] = float ( targetFOV )
        /// </param>
        public State(CameraState stateType, params object[] args) : base(stateType, args)
        {
            cameraRig = (CameraRig)args[0];
            _targetFOV = (float)args[1];
        }

        public override void Enter()
        {
            _initialFOV = cameraRig.cameraFov;
        }

        public override void Exit()
        {
            cameraRig.cameraFov = _initialFOV;
        }

        public override void Execute()
        {
            cameraRig.cameraFov = _targetFOV; // set the target FOV - the cameraRig will lerp to this value
        }

        protected CameraRig cameraRig;
    }
    #endregion

    [Space(10), Header("Camera Controller")]
    [SerializeField] private StateMachine _stateMachine;

    private State defaultState;
    private State followTargetState;
    private State closeUpState;

    public void Awake()
    {
        defaultState = new State(CameraState.DEFAULT, this, 60f);
        followTargetState = new State(CameraState.FOLLOW_TARGET, this, 50f);
        closeUpState = new State(CameraState.CLOSE_UP, this, 40f);

        _stateMachine = new StateMachine(new Dictionary<CameraState, FiniteState<CameraState>>()
        {
            { CameraState.DEFAULT, defaultState },
            { CameraState.FOLLOW_TARGET, followTargetState },
            { CameraState.CLOSE_UP, closeUpState }
        }, CameraState.DEFAULT);
    }
}




