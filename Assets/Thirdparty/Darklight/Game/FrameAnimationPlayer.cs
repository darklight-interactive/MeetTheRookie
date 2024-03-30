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
    public class Spritesheet
    {
        public Sprite[] frames;
        public bool loop;

        public int Length => frames.Length;

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
    public class Spritesheet<TState> : Spritesheet where TState : System.Enum
    {
        public TState state;
        public Spritesheet(TState state)
        {
            this.state = state;
        }
    }


    /// <summary>
    /// Plays a frame animation on a sprite renderer
    /// </summary>
    public class FrameAnimationPlayer : MonoBehaviour
    {
        SpriteRenderer _spriteRenderer = null;
        Spritesheet _spriteSheet = null;
        int _frameRate = 10;
        float _frameRateInSeconds => _frameRate * 0.01f;
        int _currentFrame = 0; // Current frame to display
        float _timer; // Timer to track when to switch to the next frame

        public int CurrentFrame => _currentFrame;
        public Spritesheet SpriteSheet => _spriteSheet;
        public FrameAnimationPlayer(SpriteRenderer spriteRenderer, Spritesheet spriteSheet)
        {
            _spriteRenderer = spriteRenderer;
            _spriteSheet = spriteSheet;

            _spriteRenderer.sprite = _spriteSheet.GetSpriteAtFrame(_currentFrame);
        }

        public void SetSpriteSheet(Spritesheet spriteSheet)
        {
            _spriteSheet = spriteSheet;
            _spriteRenderer.sprite = _spriteSheet.GetSpriteAtFrame(_currentFrame);
            _currentFrame = 0;
            _timer = 0f;
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

        public void FlipTransform(Vector2 moveInput)
        {
            if (moveInput.x < 0) { _spriteRenderer.flipX = true; }
            else if (moveInput.x > 0) { _spriteRenderer.flipX = false; }

            //_spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 180 * _flipMultiplier, 0));
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameAnimationPlayer))]
    public class FrameAnimationPlayerEditor : Editor
    {
        private FrameAnimationPlayer _script => target as FrameAnimationPlayer;

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Ensure there's a Spritesheet and it has frames
            if (_script == null) return;
            if (_script.SpriteSheet == null) return;
            if (_script.SpriteSheet.Length == 0) return;

            // Calculate the current frame to display
            Sprite currentSprite = _script.SpriteSheet.GetSpriteAtFrame(_script.CurrentFrame);

            // Draw the sprite in the Inspector
            if (currentSprite != null)
            {
                GUILayout.Label("Current Frame Preview:");
                Rect rect = GUILayoutUtility.GetRect(0, 0);
                rect.height = EditorGUIUtility.currentViewWidth - 40; // Adjust width to fit Inspector

                // Calculate rect width based on sprite's aspect ratio to maintain proportions
                rect.width = rect.height * (currentSprite.rect.width / currentSprite.rect.height);
                rect.x = (EditorGUIUtility.currentViewWidth - rect.width) / 2; // Center the sprite

                EditorGUI.DrawPreviewTexture(rect, currentSprite.texture, null, ScaleMode.ScaleToFit);
            }
        }
    }
#endif
}
