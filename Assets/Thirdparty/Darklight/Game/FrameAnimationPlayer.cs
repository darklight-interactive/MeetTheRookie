using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight
{
    [System.Serializable]
    public class Spritesheet
    {
        public Sprite[] frames;
        public bool loop;
        public int Length => frames.Length;
        public Sprite GetSpriteAtFrame(int index)
        {
            return frames[index];
        }
    }

    [System.Serializable]
    public class Spritesheet<TState> where TState : System.Enum
    {
        public TState state;
        public Sprite[] frames;
        public bool loop;
        public int Length => frames.Length;

        public Spritesheet(TState state)
        {
            this.state = state;
        }

        public Sprite GetSpriteAtFrame(int index)
        {
            return frames[index];
        }
    }

    public class FrameAnimationPlayer : MonoBehaviour
    {
        SpriteRenderer _spriteRenderer;
        Spritesheet _spriteSheet;
        int _frameRate = 10;
        float _frameRateInSeconds => _frameRate * 0.01f;
        int _currentFrame = 0; // Current frame to display
        float _timer; // Timer to track when to switch to the next frame

        public FrameAnimationPlayer(SpriteRenderer spriteRenderer, Spritesheet spriteSheet)
        {
            _spriteRenderer = spriteRenderer;
            _spriteSheet = spriteSheet;
        }

        public void Update()
        {
            _timer += Time.deltaTime;

            if (_spriteSheet.Length > 0)
            {
                if (_timer >= _frameRate)
                {
                    _currentFrame = (_currentFrame + 1) % _spriteSheet.Length;
                    _spriteRenderer.sprite = _spriteSheet.GetSpriteAtFrame(_currentFrame);
                    _timer = 0f;
                }
            }
        }

    }
}
