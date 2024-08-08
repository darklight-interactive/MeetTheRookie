using System.Collections.Generic;
using Darklight.UnityExt.Animation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(
    typeof(SpriteRenderer)
    //typeof(FrameAnimationPlayer)
    )]
public class NPC_Animator : MonoBehaviour
{
    private NPC_Controller _controller => GetComponent<NPC_Controller>();
    public NPC_StateMachine StateMachine
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
[CustomEditor(typeof(NPC_Animator)), CanEditMultipleObjects]
public class NPCAnimationEditor : Editor
{
    SerializedObject _serializedObject;
    NPC_Animator _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (NPC_Animator)target;
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
                _script.FrameAnimationPlayer.LoadSpriteSheet(_script.GetSpriteSheetWithState(_script.animationStateOverride));
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
