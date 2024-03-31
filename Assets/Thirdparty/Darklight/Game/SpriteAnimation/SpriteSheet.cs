using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.SpriteAnimation
{
    /// <summary>
    /// Stores a collection of sprites for frame animation
    /// </summary>
    [System.Serializable]
    public class SpriteSheet
    {
        public bool loop = true; // Should the animation loop
        public Sprite[] frames; // Collection of sprites to animate
        public int Length => frames.Length; // Number of frames in the animation

        /// <summary>
        /// Get the sprite at a specific frame index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sprite GetSpriteAtFrame(int index)
        {
            if (frames != null && index >= 0 && index < frames.Length)
            {
                return frames[index];
            }
            return null;
        }
    }

    /// <summary>
    /// Stores a collection of sprites for frame animation
    /// </summary>
    /// <typeparam name="TState">The related Enum State</typeparam>
    [System.Serializable]
    public class Spritesheet<TState> : SpriteSheet where TState : System.Enum
    {
        public TState state;
        public Spritesheet(TState state)
        {
            this.state = state;
        }
    }
}
