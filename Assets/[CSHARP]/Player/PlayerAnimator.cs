using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    public PlayerState animationStateOverride = PlayerState.NONE;
    public List<SpriteSheet<PlayerState>> spriteSheets = new List<SpriteSheet<PlayerState>>();

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
    #endregion

    public void Awake()
    {
        CreateFrameAnimationPlayer();
    }



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

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
