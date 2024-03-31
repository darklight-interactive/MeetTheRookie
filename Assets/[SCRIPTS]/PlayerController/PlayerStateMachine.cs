using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;

public enum PlayerState { NONE, IDLE, WALK, INTERACT }
public class PlayerStateMachine : StateMachine<PlayerState>
{
    private PlayerAnimator _animator;

    public PlayerStateMachine(PlayerState state) : base(state) { }
    public PlayerStateMachine(PlayerState state, PlayerAnimator animator) : base(state)
    {
        _animator = animator;
    }

    public override void ChangeState(PlayerState newState)
    {
        base.ChangeState(newState);
    }

    public override void OnStateChanged(PlayerState previousState, PlayerState newState)
    {
        Debug.Log("Player OnStateChanged " + newState);

        // Load the related Spritesheet to the FrameAnimationPlayer
        if (_animator == null) return;
        if (newState == PlayerState.NONE) return;
        _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.spritesheetDictionary[newState]);



    }
}
