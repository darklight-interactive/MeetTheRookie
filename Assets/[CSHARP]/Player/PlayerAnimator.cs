using System.Collections.Generic;
using Darklight.Game.SpriteAnimation;
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
    #endregion

    public void Awake()
    {
        FrameAnimationPlayer.Clear();

        // Load the default sprite sheet
        if (spriteSheets.Count > 0)
        {
            FrameAnimationPlayer.LoadSpriteSheet(spriteSheets[0].spriteSheet);
        }
    }
}
