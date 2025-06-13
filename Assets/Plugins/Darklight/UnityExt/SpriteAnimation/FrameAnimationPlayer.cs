using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Animation
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class FrameAnimationPlayer : MonoBehaviour
    {
        public enum SpriteDirection
        {
            NONE,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        protected const string PREFIX = "FrameAnimationPlayer";

        float _timer = 0f; // Timer to track when to switch to the next frame
        bool _animationDone = false;

        [SerializeField, ReadOnly]
        int _currentFrame = 0;

        [SerializeField, ShowOnly]
        SpriteDirection _currentDirection = SpriteDirection.NONE;

        [SerializeField, ReadOnly]
        Sprite _currentSprite;

        [SerializeField, Range(0, 16)]
        int _frameRate = 4;

        [SerializeField]
        SpriteDirection _defaultDirection = SpriteDirection.NONE;
        SpriteSheet _spriteSheet;

        public SpriteSheet SpriteSheet => _spriteSheet;
        protected SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
        protected float timePerFrame => 1 / (float)_frameRate; // Time each frame should be displayed
        protected int FrameRate
        {
            get => _frameRate;
            set => _frameRate = value;
        }

        public void Awake() => Initialize();

        public virtual void Update()
        {
            if (_spriteSheet == null)
                return;
            UpdateFrame();
        }

        public virtual void Initialize()
        {
            if (_spriteSheet == null)
                return;
            LoadSpriteSheet(_spriteSheet);

            if (_defaultDirection != SpriteDirection.NONE)
                SetFacing(_defaultDirection);
        }

        void UpdateFrame()
        {
            if (_spriteSheet == null)
                return;
            if (_currentFrame + 1 == _spriteSheet.Length && !_spriteSheet.loop)
            {
                _animationDone = true;
                return;
            }

            _timer += Time.deltaTime; // Update Timer

            // Check if it's time to update to the next frame
            if (_timer >= timePerFrame)
            {
                _currentFrame = (_currentFrame + 1) % _spriteSheet.Length;
                spriteRenderer.sprite = _spriteSheet.GetSpriteAtFrame(_currentFrame);
                _currentSprite = spriteRenderer.sprite;

                // Reset the timer, accounting for any "overflow" time past the expected frame duration
                _timer -= timePerFrame;
            }
        }

        // Timer to track when to switch to the next frame
        public void LoadSpriteSheet(SpriteSheet spriteSheet)
        {
            if (spriteSheet == null)
            {
                Debug.LogError($"{PREFIX} SpriteSheet is null.", this);
                return;
            }

            _spriteSheet = spriteSheet;
            _currentSprite = _spriteSheet.GetSpriteAtFrame(0);
            spriteRenderer.sprite = _currentSprite;

            _animationDone = false;
            _currentFrame = 0;
            _timer = 0f;
        }

        public void Clear()
        {
            _spriteSheet = null;
            spriteRenderer.sprite = null;
        }

        public void SetFacing(SpriteDirection direction)
        {
            _currentDirection = direction;
            if (_spriteSheet == null)
                return;

            if (_currentDirection == SpriteDirection.LEFT)
                spriteRenderer.flipX = false;
            else if (_currentDirection == SpriteDirection.RIGHT)
                spriteRenderer.flipX = true;
        }

        public bool AnimationIsOver()
        {
            return _animationDone;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FrameAnimationPlayer))]
    public class FrameAnimationPlayerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        FrameAnimationPlayer _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (FrameAnimationPlayer)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                _script.Awake();
            }
        }
    }
#endif
}
