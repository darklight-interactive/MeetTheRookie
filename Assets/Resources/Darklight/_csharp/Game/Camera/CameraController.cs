using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;
using Darklight.Game.Utility;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Camera
{
    [ExecuteAlways]
    public class CameraController : CameraRig
    {
        public enum CameraStateKey { DEFAULT, FOLLOW_TARGET, CLOSE_UP }

        #region State Machine ============================================== >>>>
        public StateMachine _stateMachine;

        public class StateMachine : FiniteStateMachine<CameraStateKey>
        {
            public StateMachine(Dictionary<CameraStateKey, FiniteState<CameraStateKey>> possibleStates, CameraStateKey initialState, params object[] args) : base(possibleStates, initialState, args) { }
        }

        /// <summary>
        /// This state is the default state for the camera. 
        /// It does not follow any target and is in a fixed position.
        /// </summary>
        public class CameraState : FiniteState<CameraStateKey>
        {
            private CameraRig _cameraRig;
            private float _offsetFOV;

            /// <param name="args">
            ///     args[0] = CameraRig ( cameraRig )
            ///     args[1] = float ( FOVOffset )
            /// </param>
            public CameraState(CameraStateKey stateType, params object[] args) : base(stateType, args)
            {
                _cameraRig = (CameraRig)args[0];
                _offsetFOV = (float)args[1];
            }

            public override void Enter()
            {
                _cameraRig.SetOffsetFOV(_offsetFOV);
            }

            public override void Exit() { }
            public override void Execute() { }
        }
        #endregion

        public override void Update()
        {
            base.Update();

            if (_stateMachine != null)
                _stateMachine.Step();
        }
    }
}



