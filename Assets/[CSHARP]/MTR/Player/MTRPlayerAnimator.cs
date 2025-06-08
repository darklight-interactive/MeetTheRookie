using System.Collections.Generic;
using Darklight.UnityExt.Animation;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MTRPlayerController))]
[RequireComponent(typeof(SpriteRenderer))]
public class MTRPlayerAnimator : FrameAnimationPlayer
{
    MTRPlayerController _playerController;

    [HorizontalLine(color: EColor.Gray)]
    [Header("Player Animator")]
    public MTRPlayerState animationStateOverride = MTRPlayerState.NULL;
    public List<SpriteSheet<MTRPlayerState>> spriteSheets = new List<SpriteSheet<MTRPlayerState>>();

    public override void Initialize()
    {
        animationStateOverride = spriteSheets[0].state;

        if (animationStateOverride != MTRPlayerState.NULL)
        {
            SpriteSheet spriteSheet = GetSpriteSheetWithState(animationStateOverride);
            if (spriteSheet != null)
                LoadSpriteSheet(spriteSheet);
            else
                Debug.LogError("No sprite sheet found for state: " + animationStateOverride);
        }
        else
        {
            Debug.LogError("No animation state set for player animator");
        }

        _playerController = GetComponent<MTRPlayerController>();
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
        if (_playerController == null)
            return;
        if (_playerController.DirectionFacing == MTRPlayerDirectionFacing.LEFT)
            SetFacing(SpriteDirection.LEFT);
        else if (_playerController.DirectionFacing == MTRPlayerDirectionFacing.RIGHT)
            SetFacing(SpriteDirection.RIGHT);
    }

    public void PlayStateAnimation(MTRPlayerState state)
    {
        // If there is a sprite sheet with the state, load it
        if (spriteSheets.Find(x => x.state == state) != null)
        {
            LoadSpriteSheet(spriteSheets.Find(x => x.state == state).spriteSheet);
            animationStateOverride = state;
        }
    }

    public SpriteSheet GetSpriteSheetWithState(MTRPlayerState state)
    {
        foreach (SpriteSheet<MTRPlayerState> sheet in spriteSheets)
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
[CustomEditor(typeof(MTRPlayerAnimator)), CanEditMultipleObjects]
public class PlayerAnimationEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    MTRPlayerAnimator _script;

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTRPlayerAnimator)target;
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
