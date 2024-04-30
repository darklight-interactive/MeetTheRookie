using System.Collections;
using System.Collections.Generic;
using Darklight.Game;
using Darklight.Game.Camera;
using Darklight.Game.Utility;
using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game
{

    public enum CameraState
    {
        DEFAULT,
        FOLLOW_TARGET,
        CLOSE_UP
    }

    [System.Serializable]
    public class CameraStateMachine : FiniteStateMachine<CameraState>
    {
        public CameraStateMachine(CameraState initialState, Dictionary<CameraState, IState<CameraState>> possibleStates, GameObject parent) : base(initialState, possibleStates, parent)
        {
            GoToState(initialState);
        }

        public override void Step()
        {
            base.Step();
        }

        public override void GoToState(CameraState state, params object[] enterArgs)
        {
            if (currentState == state) return;
            base.GoToState(state, enterArgs);
        }

    }

    /// <summary>
    /// This state is the default state for the camera. It does not follow any target and is in a fixed position.
    /// </summary>
    public class CameraDefaultState : IState<CameraState>
    {
        CameraRig3D _rig3D;
        CameraSettings _settings;
        public CameraDefaultState(CameraRig3D rig3D, ref CameraSettings settings)
        {
            _settings = settings;
            _rig3D = rig3D;
        }

        public FiniteStateMachine<CameraState> StateMachine { get; set; }

        public void Enter(params object[] enterArgs)
        {
            _rig3D.ActiveSettings = _settings;
        }

        public void Exit()
        {
            //throw new System.NotImplementedException();
        }

        public void Execute(params object[] executeArgs)
        {
            //throw new System.NotImplementedException();
        }
    }

}