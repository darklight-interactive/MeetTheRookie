using System.Collections.Generic;
using Darklight.Game.SpriteAnimation;
using UnityEngine;
#if UNITY_EDITOR
#endif

[RequireComponent(typeof(SpriteRenderer), typeof(FrameAnimationPlayer))]
public class PlayerAnimator : MonoBehaviour
{
    public PlayerState animationStateOverride = PlayerState.NONE;
    public List<SpriteSheet<PlayerState>> spriteSheets = new List<SpriteSheet<PlayerState>>();

    #region [[ FRAME ANIMATION PLAYER ]] ======================================================== >>
    public FrameAnimationPlayer FrameAnimationPlayer => GetComponent<FrameAnimationPlayer>();
    public void PlayStateAnimation(PlayerState state)
    {
        // If there is a sprite sheet with the state, load it
        if (spriteSheets.Find(x => x.state == state) != null)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets.Find(x => x.state == state).spriteSheet);
            animationStateOverride = state;
        }
    }
    #endregion

    public void Awake()
    {
        FrameAnimationPlayer.Clear();

        // Load the default sprite sheet
        if (spriteSheets.Count > 0)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets[0].spriteSheet);
            animationStateOverride = spriteSheets[0].state;
        }
    }
}
