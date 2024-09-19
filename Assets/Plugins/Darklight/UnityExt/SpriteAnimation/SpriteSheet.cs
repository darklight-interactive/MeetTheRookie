using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Animation
{
    /// <summary>
    /// The Base Class of Spritesheets.
    /// It holds a collection of sprites for frame animation
    /// </summary>
    [System.Serializable]
    public class SpriteSheet
    {
        public bool loop = true; // Should the animation loop
        public Sprite[] frames = new Sprite[4]; // Collection of sprites to animate
        public int Length => frames.Length; // Number of frames in the animation

        public SpriteSheet() { }

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
    /// A Wr
    /// Stores a collection of sprites for frame animation
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    [System.Serializable]
    public class SpriteSheet<TState>
    {
        public TState state;
        public SpriteSheet spriteSheet;
        public SpriteSheet(TState state, SpriteSheet spriteSheet)
        {
            this.state = state;
            this.spriteSheet = spriteSheet;
        }
    }
}
