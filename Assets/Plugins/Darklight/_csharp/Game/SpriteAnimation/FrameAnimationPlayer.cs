using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.SpriteAnimation
{
    public static class GlobalFrameRate
    {
        private static int _frameRate = 4;  // Global frame rate for all animations

        public static int FrameRate
        {
            get => _frameRate;
            set => _frameRate = Mathf.Max(1, value); // Ensure framerate is at least 1
        }

        public static float TimePerFrame => 1f / FrameRate; // Time each frame should be displayed
    }



    /// <summary>
    /// Plays a frame animation on a sprite renderer
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FrameAnimationPlayer : MonoBehaviour
    {
        private float _timer = 0f; // Timer to track when to switch to the next frame
        private SpriteRenderer _spriteRenderer => GetComponent<SpriteRenderer>();
        private SpriteSheet currentSpriteSheet = null;
        private int currentFrame = 0;

        private void Start()
        {
            InitializeSprite();
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void InitializeSprite()
        {
            if (currentSpriteSheet != null && currentSpriteSheet.Length > 0)
            {
                _spriteRenderer.sprite = currentSpriteSheet.GetSpriteAtFrame(currentFrame);
            }
        }

        public void UpdateAnimation()
        {
            if (currentSpriteSheet == null) return;
            if (currentFrame + 1 == currentSpriteSheet.Length && !currentSpriteSheet.loop) return;

            _timer += Time.deltaTime; // Update Timer

            // Check if it's time to update to the next frame
            if (_timer >= GlobalFrameRate.TimePerFrame)
            {
                currentFrame = (currentFrame + 1) % currentSpriteSheet.Length;
                _spriteRenderer.sprite = currentSpriteSheet.GetSpriteAtFrame(currentFrame);

                // Reset the timer, accounting for any "overflow" time past the expected frame duration
                _timer -= GlobalFrameRate.TimePerFrame;
            }
        }

        public Sprite GetCurrentSprite()
        {
            return currentSpriteSheet.GetSpriteAtFrame(currentFrame);
        }

        // Timer to track when to switch to the next frame
        public void LoadSpriteSheet(SpriteSheet spriteSheet)
        {
            this.currentSpriteSheet = spriteSheet;
            try
            {
                Sprite sprite = spriteSheet.GetSpriteAtFrame(0);
                if (sprite != null)
                {
                    _spriteRenderer.sprite = sprite;
                }
            }
            catch { }

            currentFrame = 0;
            _timer = 0f;
        }

        public void Clear()
        {
            currentSpriteSheet = null;
            _spriteRenderer.sprite = null;
        }

        public void FlipTransform(Vector2 moveInput)
        {
            // Flip the sprite based on the input direction only if the player is moving
            float x = moveInput.x;
            if (x > 0.1f || x < -0.1f)
            {
                _spriteRenderer.flipX = moveInput.x > 0f;
            }

            //how many ways can you flip a sprite?

            /*
            if (moveInput.x < 0) { _spriteRenderer.flipX = true; }
            else if (moveInput.x > 0) { _spriteRenderer.flipX = false; }
            */

            //_spriteRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, 180 * _flipMultiplier, 0));
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

            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            // Only use this in the editor
            if (Application.isPlaying) return;

            // do things
            _script.UpdateAnimation();

            // force loop update so that inputs aren't required
            EditorApplication.QueuePlayerLoopUpdate();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            DrawDefaultInspector();

            // Ensure there's a Spritesheet and it has frames
            if (_script == null) return;
            Sprite currentSprite = _script.GetCurrentSprite();
            if (currentSprite == null) return;

            Texture2D texture = AssetPreview.GetAssetPreview(currentSprite);
            GUILayout.Label(texture);


            _serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
