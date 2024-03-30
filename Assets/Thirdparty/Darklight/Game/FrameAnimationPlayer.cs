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
        public bool loop; // Should the animation loop
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

    /// <summary>
    /// Plays a frame animation on a sprite renderer
    /// </summary>
    [ExecuteAlways]
    public class FrameAnimationPlayer : MonoBehaviour
    {
        public static int frameRate { get; private set; } = 2; // Global frame rate for all animations
        private float _timePerFrame; // Time each frame should be displayed
        private float _timer = 0f; // Timer to track when to switch to the next frame

        public SpriteRenderer spriteRenderer;
        public SpriteSheet spriteSheet;
        public int currentFrame = 0;

        // Timer to track when to switch to the next frame
        public void SetSpriteSheet(SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            spriteRenderer.sprite = this.spriteSheet.GetSpriteAtFrame(currentFrame);
            currentFrame = 0;
            _timer = 0f;
        }

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            // Calculate how long each frame should be displayed
            _timePerFrame = 1f / frameRate;

            if (spriteSheet != null && spriteSheet.Length > 0)
            {
                // Set the initial sprite frame
                spriteRenderer.sprite = spriteSheet.GetSpriteAtFrame(currentFrame);
            }
        }

        private void Update()
        {
            if (spriteSheet == null || spriteSheet.Length == 0) return;

            _timer += Time.deltaTime; // Update Timer

            // Check if it's time to update to the next frame
            if (_timer >= _timePerFrame)
            {
                currentFrame = (currentFrame + 1) % spriteSheet.Length;
                spriteRenderer.sprite = spriteSheet.GetSpriteAtFrame(currentFrame);

                // Reset the timer, accounting for any "overflow" time past the expected frame duration
                _timer -= _timePerFrame;
            }
        }

        public void FlipTransform(Vector2 moveInput)
        {
            spriteRenderer.flipX = moveInput.x < 0; // Flip the sprite if moving left

            /*
            //how many ways can you flip a sprite?
            if (moveInput.x < 0) { _spriteRenderer.flipX = true; }
            else if (moveInput.x > 0) { _spriteRenderer.flipX = false; }
            */

            //_spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 180 * _flipMultiplier, 0));
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameAnimationPlayer))]
    public class FrameAnimationPlayerEditor : Editor
    {
        private SerializedObject _serializedObject;
        private FrameAnimationPlayer _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (FrameAnimationPlayer)target;
        }

        public override void OnInspectorGUI()
        {
            // Ensure there's a Spritesheet and it has frames
            if (_script == null) return;

            EditorGUILayout.BeginVertical();

            Sprite currentSprite = _script.spriteSheet.GetSpriteAtFrame(_script.currentFrame);
            EditorGUILayout.LabelField($"Global Frame Rate: {FrameAnimationPlayer.frameRate.ToString()}");
            Darklight.UnityExt.CustomInspectorGUI.CreateTwoColumnLabel(
                $"Frame {_script.currentFrame}/{_script.spriteSheet.Length.ToString()}",
                $"{currentSprite.name}"
            );

            Texture2D texture = AssetPreview.GetAssetPreview(currentSprite);
            GUILayout.Label(texture);

            EditorGUILayout.EndVertical();
        }
    }
#endif
}
