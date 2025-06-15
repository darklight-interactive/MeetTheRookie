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
        else
        {
            Debug.LogError("No animation state set for player animator");
        }

        base.Initialize();
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

    public void SetFacingTowardsPosition(Vector2 position)
    {
        if (position.x > transform.position.x)
            SetFacing(SpriteDirection.RIGHT);
        else
            SetFacing(SpriteDirection.LEFT);
    }

    public void SetFrameRate(int frameRate)
    {
        FrameRate = frameRate;
    }
}
