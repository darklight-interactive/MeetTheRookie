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
public class MTRCharacterAnimator : FrameAnimationPlayer
{
    private NPC_Controller _controller => GetComponent<NPC_Controller>();
    public NPC_StateMachine StateMachine
    {
        get => _controller.stateMachine;
        set => _controller.stateMachine = value;
    }
    public NPCState animationStateOverride = NPCState.NONE;
    public List<SpriteSheet<NPCState>> spriteSheets = new List<SpriteSheet<NPCState>>();

    public override void Initialize()
    {
        animationStateOverride = spriteSheets[0].state;

        if (animationStateOverride != NPCState.NONE)
        {
            SpriteSheet spriteSheet = GetSpriteSheetWithState(animationStateOverride);
            if (spriteSheet != null)
                LoadSpriteSheet(spriteSheet);
            else
                Debug.LogError("No sprite sheet found for state: " + animationStateOverride);
        }
        else { Debug.LogError("No animation state set for player animator"); }

        UpdateFacing();
    }

    public override void Update()
    {
        base.Update();
        UpdateFacing();
    }

    void UpdateFacing()
    {

    }

    public void PlayStateAnimation(NPCState state)
    {
        // If there is a sprite sheet with the state, load it
        if (spriteSheets.Find(x => x.state == state) != null)
        {
            LoadSpriteSheet(spriteSheets.Find(x => x.state == state).spriteSheet);
            animationStateOverride = state;
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTRCharacterAnimator)), CanEditMultipleObjects]
public class NPCAnimationEditor : Editor
{
    SerializedObject _serializedObject;
    MTRCharacterAnimator _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTRCharacterAnimator)target;
        _script.Awake();
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
                _script.LoadSpriteSheet(_script.GetSpriteSheetWithState(_script.animationStateOverride));
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
