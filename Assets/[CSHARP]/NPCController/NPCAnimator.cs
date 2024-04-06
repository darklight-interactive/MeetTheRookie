using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(
    typeof(SpriteRenderer)
    //typeof(FrameAnimationPlayer)
    )]
public class NPCAnimator : MonoBehaviour
{
    private NPCController _controller => GetComponent<NPCController>();
    public NPCStateMachine stateMachine
    {
        get => _controller.stateMachine;
        set => _controller.stateMachine = value;
    }
    public NPCState animationStateOverride = NPCState.NONE;
    public List<SpriteSheet<NPCState>> spriteSheets = new List<SpriteSheet<NPCState>>();

    #region [[ FRAME ANIMATION PLAYER ]] ======================================================== >>
    public FrameAnimationPlayer FrameAnimationPlayer { get; private set; }
    public void CreateFrameAnimationPlayer()
    {
        stateMachine = new NPCStateMachine(NPCState.IDLE, this);

        FrameAnimationPlayer = GetComponentInChildren<FrameAnimationPlayer>();

        if (FrameAnimationPlayer == null)
        {
            // Add the required components
            FrameAnimationPlayer = this.gameObject.AddComponent<FrameAnimationPlayer>();
            SpriteRenderer sr = this.gameObject.AddComponent<SpriteRenderer>();
        }

        FrameAnimationPlayer.Clear();

        // Load the default sprite sheet
        if (spriteSheets.Count > 0)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets[0].spriteSheet);
            animationStateOverride = spriteSheets[0].state;
        }
    }

    public SpriteSheet GetSpriteSheetWithState(NPCState state)
    {
        foreach (SpriteSheet<NPCState> sheet in spriteSheets)
        {
            if (sheet.state == state)
            {
                return sheet.spriteSheet;
            }
        }
        return null;
    }
    #endregion

    public void Awake()
    {
        CreateFrameAnimationPlayer();
    }



}

#if UNITY_EDITOR
[CustomEditor(typeof(NPCAnimator)), CanEditMultipleObjects]
public class NPCAnimationEditor : Editor
{
    SerializedObject _serializedObject;
    NPCAnimator _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (NPCAnimator)target;
        _script.CreateFrameAnimationPlayer();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            if (_script.animationStateOverride != NPCState.NONE)
            {
                _script.stateMachine.ChangeState(_script.animationStateOverride);
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
