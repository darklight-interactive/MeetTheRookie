using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer), typeof(FrameAnimationPlayer))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _controller => GetComponent<PlayerController>();
    public PlayerStateMachine StateMachine => _controller.stateMachine;
    public PlayerState animationStateOverride = PlayerState.NONE;
    public Dictionary<PlayerState, SpriteSheet> spritesheetDictionary = new Dictionary<PlayerState, SpriteSheet>();

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

        spritesheetDictionary[PlayerState.IDLE] = new SpriteSheet();

        //FrameAnimationPlayer.LoadSpriteSheet(spritesheetDictionary[PlayerState.IDLE]);
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

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (_script.spritesheetDictionary.Count > 1)
        {
            DrawDictionary(_script.spritesheetDictionary);
        }

        if (EditorGUI.EndChangeCheck())
        {
            if (_script.animationStateOverride != PlayerState.NONE)
            {
                _script.StateMachine.ChangeState(_script.animationStateOverride);
            }

            _serializedObject.ApplyModifiedProperties();
        }
    }

    public static void DrawDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        EditorGUILayout.BeginVertical();
        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(kvp.Key as SerializedProperty);
            EditorGUILayout.PropertyField(kvp.Value as SerializedProperty);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }


}
#endif
