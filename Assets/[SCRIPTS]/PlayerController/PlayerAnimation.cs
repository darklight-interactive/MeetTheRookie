using System.Collections;
using System.Collections.Generic;
using Darklight;
using UnityEngine;


[RequireComponent(typeof(FrameAnimationPlayer))]
public class PlayerAnimation : MonoBehaviour
{
    #region [[ STATE MACHINE ]] ============================================== >>
    public enum AnimationState { IDLE, WALK, INTERACT }
    public class AnimStateMachine : StateMachine<AnimationState>
    {
        public AnimStateMachine(AnimationState state) : base(state) { }
    }
    #endregion

    #region [[ CLASS REFEREMCES ]] ================================================= >>
    AnimStateMachine _animStateMachine = new AnimStateMachine(AnimationState.IDLE);
    PlayerController _playerController => GetComponentInParent<PlayerController>();
    SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();
    #endregion

    public Spritesheet<AnimationState> idleAnimation = new(AnimationState.IDLE);
    public Spritesheet<AnimationState> walkAnimation = new(AnimationState.WALK);
    public Spritesheet<AnimationState> interactAnimation = new Spritesheet<AnimationState>(AnimationState.INTERACT);

    // << Flip Sprite based on Input >> ============================== >>
    private int _flipMultiplier = 1;
    public void FlipTransform(Vector2 moveInput)
    {
        if (moveInput.x < 0) { _flipMultiplier = 0; }
        else if (moveInput.x > 0) { _flipMultiplier = 1; }

        _spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 180 * _flipMultiplier, 0));
    }

}
