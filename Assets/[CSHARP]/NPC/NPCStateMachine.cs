using System.Collections;
using System.Collections.Generic;
using Darklight;
using Darklight.Game.SpriteAnimation;
using UnityEngine;

public enum NPCState { NONE, IDLE, WALK, SPEAK, FOLLOW, HIDE, CHASE }

public class NPCStateMachine : StateMachine<NPCState>
{
    private NPCAnimator _animator;

    public NPCStateMachine(NPCState state) : base(state) { }
    public NPCStateMachine(NPCState state, NPCAnimator animator) : base(state)
    {
        _animator = animator;
    }

    public override void ChangeState(NPCState newState)
    {
        base.ChangeState(newState);
    }

    public override void OnStateChanged(NPCState previousState, NPCState newState)
    {

        // Load the related Spritesheet to the FrameAnimationPlayer
        if (_animator == null) return;
        if (newState == NPCState.NONE) return;

        //Debug.Log($"NPC OnStateChanged {previousState} -> {newState}");
        _animator.FrameAnimationPlayer.LoadSpriteSheet(_animator.GetSpriteSheetWithState(newState));

    }

}