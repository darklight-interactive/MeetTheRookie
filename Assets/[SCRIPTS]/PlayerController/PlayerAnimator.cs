using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerAnimator : MonoBehaviour
{

    #region [[ STATE MACHINE ]] ============================================== >>
    public enum PlayerState { IDLE, WALK, INTERACT }
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
        public PlayerStateMachine(PlayerState state) : base(state) { }
    }
    public PlayerStateMachine StateMachine { get; private set; } = new PlayerStateMachine(PlayerState.IDLE);
    public PlayerState CurrentState => StateMachine.CurrentState;
    #endregion

    #region [[ PUBLIC INSPECTOR VALUES ]] ======================================================== >>
    public Spritesheet<PlayerState> idleAnimation = new(PlayerState.IDLE);
    public Spritesheet<PlayerState> walkAnimation = new(PlayerState.WALK);
    public Spritesheet<PlayerState> interactAnimation = new Spritesheet<PlayerState>(PlayerState.INTERACT);
    #endregion

    #region [[ FRAME ANIMATION PLAYER ]] ======================================================== >>
    public FrameAnimationPlayer FrameAnimationPlayer { get; private set; }
    public void CreateFrameAnimationPlayer()
    {
        FrameAnimationPlayer = GetComponentInChildren<FrameAnimationPlayer>();
        if (FrameAnimationPlayer == null)
        {
            // Add the required components
            FrameAnimationPlayer = this.gameObject.AddComponent<FrameAnimationPlayer>();
            SpriteRenderer sr = this.gameObject.AddComponent<SpriteRenderer>();

            // Create new Frame Animation Player
            FrameAnimationPlayer.SetSpriteSheet(idleAnimation);
        }
        else
        {
            FrameAnimationPlayer.SetSpriteSheet(idleAnimation);
        }
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerAnimator)), CanEditMultipleObjects]
public class PlayerAnimationEditor : Editor
{
    SerializedObject _serializedObject;
    PlayerAnimator _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (PlayerAnimator)target;
        _script.CreateFrameAnimationPlayer();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();
        EditorGUILayout.Space();

        Darklight.UnityExt.CustomInspectorGUI.DrawEnumValue(_script.CurrentState, "Current State");
        Darklight.UnityExt.CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(_serializedObject);

        _serializedObject.ApplyModifiedProperties();
    }
}
#endif
