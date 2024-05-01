using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    [ExecuteAlways]
    public class CameraController : CameraRig
    {

        #region State Machine ============================================== >>>>
        public StateMachine _stateMachine;

        public class StateMachine : FiniteStateMachine<CameraStateType>
        {
            public StateMachine(Dictionary<CameraStateType, FiniteState<CameraStateType>> possibleStates, CameraStateType initialState, params object[] args) : base(possibleStates, initialState, args) { }
        }

        public enum CameraStateType
        {
            DEFAULT,
            FOLLOW_TARGET,
            CLOSE_UP
        }

        /// <summary>
        /// This state is the default state for the camera. It does not follow any target and is in a fixed position.
        /// </summary>
        public class CameraState : FiniteState<CameraStateType>
        {
            private float _FOVOffset;

            /// <param name="args">
            ///     args[0] = CameraRig ( cameraRig )
            ///     args[1] = float ( FOVOffset )
            /// </param>
            public CameraState(CameraStateType stateType, params object[] args) : base(stateType, args)
            {
                cameraRig = (CameraRig)args[0];
                _FOVOffset = (float)args[1];
            }

            public override void Enter()
            {
                cameraRig.SetFOVOffset(_FOVOffset);
            }

            public override void Exit() { }
            public override void Execute() { }


            protected CameraRig cameraRig;
        }
        #endregion

        public virtual void Awake()
        {

        }

        public override void Update()
        {
            base.Update();

            if (_stateMachine != null)
                _stateMachine.Step();
        }
    }
}



