using System.Collections.Generic;
using Darklight.UnityExt.Animation;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MTRPlayerInputController))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : FrameAnimationPlayer
{
    MTRPlayerInputController _playerController;

    [HorizontalLine(color: EColor.Gray)]
    [Header("Player Animator")]
    public PlayerState animationStateOverride = PlayerState.NULL;
    public List<SpriteSheet<PlayerState>> spriteSheets = new List<SpriteSheet<PlayerState>>();

    public override void Initialize()
    {
        animationStateOverride = spriteSheets[0].state;

        if (animationStateOverride != PlayerState.NULL)
        {
            SpriteSheet spriteSheet = GetSpriteSheetWithState(animationStateOverride);
            if (spriteSheet != null)
                LoadSpriteSheet(spriteSheet);
            else
                Debug.LogError("No sprite sheet found for state: " + animationStateOverride);
        }
        else { Debug.LogError("No animation state set for player animator"); }

        _playerController = GetComponent<MTRPlayerInputController>();
        UpdateFacing();
    }

    public override void Update()
    {
        base.Update();
        UpdateFacing();
    }

    void UpdateFacing()
    {
        // Update facing direction from the player controller
        if (_playerController == null) return;
        if (_playerController.Facing == PlayerFacing.LEFT)
            SetFacing(SpriteDirection.LEFT);
        else if (_playerController.Facing == PlayerFacing.RIGHT)
            SetFacing(SpriteDirection.RIGHT);
    }

    public void PlayStateAnimation(PlayerState state)
    {
        // If there is a sprite sheet with the state, load it
        if (spriteSheets.Find(x => x.state == state) != null)
        {
            LoadSpriteSheet(spriteSheets.Find(x => x.state == state).spriteSheet);
            animationStateOverride = state;
        }
    }

    public SpriteSheet GetSpriteSheetWithState(PlayerState state)
    {
        foreach (SpriteSheet<PlayerState> sheet in spriteSheets)
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
[CustomEditor(typeof(PlayerAnimator)), CanEditMultipleObjects]
public class PlayerAnimationEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    PlayerAnimator _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (PlayerAnimator)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {


            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
