using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;

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

        // Load the related Spritesheet to the FrameAnimationPlayer
        if (_animator == null) return;
        if (newState == PlayerState.NONE) return;

        Debug.Log($"Player OnStateChanged {previousState} -> {newState}");
        _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(newState));
    }
}
