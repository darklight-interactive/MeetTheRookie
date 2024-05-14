using System.Collections.Generic;
using Darklight.Game.SpriteAnimation;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

[RequireComponent(typeof(SpriteRenderer), typeof(FrameAnimationPlayer))]
public class PlayerAnimator : MonoBehaviour
{

    public List<SpriteSheet<PlayerStateType>> spriteSheets = new List<SpriteSheet<PlayerStateType>>();

    #region [[ FRAME ANIMATION PLAYER ]] ======================================================== >>
    public FrameAnimationPlayer FrameAnimationPlayer => GetComponent<FrameAnimationPlayer>();

    [EasyButtons.Button]
    public void PlayStateAnimation(PlayerStateType state)
    {
        // If there is a sprite sheet with the state, load it
        if (spriteSheets.Find(x => x.state == state) != null)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets.Find(x => x.state == state).spriteSheet);
        }
    }

    public void CreateFrameAnimationPlayer()
    {

        FrameAnimationPlayer.Clear();

        // Load the default sprite sheet
        if (spriteSheets.Count > 0)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets[0].spriteSheet);
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
            if (_script.animationStateOverride != PlayerState.NONE)
            {
                _script.FrameAnimationPlayer.LoadSpriteSheet(_script.GetSpriteSheetWithState(_script.animationStateOverride));
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
