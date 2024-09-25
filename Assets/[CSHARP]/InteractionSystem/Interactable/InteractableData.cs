using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using UnityEngine;

public partial class Interactable
{
    [Serializable]
    public class InternalData
    {
        #region -- (( CONSTANTS )) ------------------- >>
        public const string DEFAULT_NAME = "DefaultName";
        public const string DEFAULT_KEY = "DefaultKey";
        public const string DEFAULT_LAYER = "Interactable";
        #endregion

        Interactable _interactable;
        SpriteRenderer _spriteRenderer;
        BoxCollider2D _collider;

        [SerializeField] string _name = DEFAULT_NAME;
        [SerializeField, ShowOnly] string _key = DEFAULT_KEY;
        [SerializeField, ShowOnly] string _layer = DEFAULT_LAYER;

        [Header("Flags")]
        [SerializeField, ShowOnly] bool _isRegistered;
        [SerializeField, ShowOnly] bool _isPreloaded;
        [SerializeField, ShowOnly] bool _isInitialized;


        [Header("Interaction System Data")]
        public InteractionRequestPreset interactionRequest;


        [Header("Sprite Data")]
        [SerializeField] Sprite _sprite;
        [SerializeField] Color _defaultTint = Color.white;
        [SerializeField] Color _interactionTint = Color.yellow;


        public string Name
        {
            get
            {
                if (_name == null || _name == string.Empty)
                    _name = DEFAULT_NAME;
                return _name;
            }
        }
        public string Key
        {
            get
            {
                if (_key == null || _key == string.Empty)
                    _key = DEFAULT_KEY;
                return _key;
            }
        }
        public string Layer
        {
            get => _layer;
            set
            {
                _layer = value;
                _interactable.gameObject.layer = LayerMask.NameToLayer(value);
            }
        }
        public bool IsRegistered => _isRegistered;
        public bool IsPreloaded => _isPreloaded;
        public bool IsInitialized => _isInitialized;
        public Sprite Sprite => _sprite;

        public InternalData(Interactable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
        {
            Preload(interactable, name, key);
        }

        public bool Preload(Interactable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
        {
            _interactable = interactable;
            _name = name;
            _key = key;
            _interactable.gameObject.layer = LayerMask.NameToLayer(DEFAULT_LAYER);

            SetFlagsToDefault();

            PreloadSpriteRenderer();
            PreloadBoxCollider();

            interactionRequest = InteractionSystem.Factory.CreateOrLoadRequestPreset();
            PreloadRecievers();

            _isRegistered = InteractionSystem.Registry.TryRegisterInteractable(_interactable);
            _isPreloaded = ConfirmPreloadIsValid(true);

            if (_isPreloaded)
                _interactable.name = BuildObjectName();
            return _isPreloaded;
        }
        public bool Initialize()
        {
            if (_interactable == null)
            {
                Debug.LogError($"{PREFIX} {Name} :: Cannot Initialize Interactable. Interactable is null", _interactable);
                return false;
            }
            else if (!_isPreloaded)
            {
                if (Preload(_interactable) == false)
                {
                    Debug.LogError($"{PREFIX} {Name} :: Cannot Initialize Interactable. Preload Failed", _interactable);
                    return false;
                }
            }

            Debug.Log($"{PREFIX} Initialized Interactable {Name} :: {Key}", _interactable);
            return _isInitialized = true;
        }


        #region == [[ PRELOAD ]] ================ >>
        bool ConfirmPreloadIsValid(bool logErrors = false)
        {
            bool isValid = IsRegistered
                && _interactable._recievers.Count > 0
                && !_interactable._recievers.HasUnsetKeysOrValues();

            if (!isValid && logErrors)
            {
                string outLog = $"{PREFIX} {Name} :: Preload Validation Failed";
                if (!IsRegistered)
                    outLog += " :: Not Registered";
                if (_interactable._recievers.Count == 0)
                    outLog += " :: No Recievers Requested";
                if (_interactable._recievers.HasUnsetKeysOrValues())
                    outLog += " :: Found Unset Recievers";
                Debug.LogError(outLog, _interactable);
            }

            return isValid;
        }

        void PreloadSpriteRenderer()
        {
            // << GET OR ADD SPRITE RENDERER >> ------------------------------------
            _spriteRenderer = _interactable.GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                _spriteRenderer = _interactable.gameObject.AddComponent<SpriteRenderer>();

            // << SET THE INITIAL SPRITE >> ------------------------------------
            // If the data contains a sprite, assign it to the sprite renderer
            if (_sprite != null)
                _spriteRenderer.sprite = _sprite;
            // Else if the sprite renderer has a sprite, assign it to the data
            else if (_spriteRenderer.sprite != null)
                _sprite = _spriteRenderer.sprite;

            // << SET THE DEFAULT TINT >> ------------------------------------
            _spriteRenderer.color = _defaultTint;
        }
        void PreloadBoxCollider()
        {
            _collider = _interactable.GetComponent<BoxCollider2D>();
            if (_collider == null)
                _collider = _interactable.gameObject.AddComponent<BoxCollider2D>();

            // << SET THE COLLIDER SIZE >> ------------------------------------
            // Set the collider size to half the size of the transform scale
            _collider.size = Vector2.one * _interactable.transform.localScale.x * 0.5f;
        }

        #region -- (( PRELOAD RECIEVERS )) ------------------- >>
        void PreloadRecievers()
        {
            _interactable._recievers.Reset();

            List<InteractionTypeKey> requestedKeys = interactionRequest.Keys.ToList();
            foreach (InteractionTypeKey key in requestedKeys)
            {
                _interactable._recievers.TryGetValue(key, out InteractionReciever currentHandlerValue);
                if (currentHandlerValue == null)
                {
                    InteractionReciever recieverInChild = GetRecieverInChildren(key);
                    if (recieverInChild != null)
                    {
                        _interactable._recievers[key] = recieverInChild;
                        continue;
                    }

                    GameObject recieverGameObject = interactionRequest.CreateRecieverGameObject(key);
                    if (recieverGameObject == null)
                    {
                        Debug.LogError($"CreateInteractionHandler failed for key {key}. GameObject is null.", _interactable);
                        continue;
                    }
                    else
                    {
                        recieverGameObject.transform.SetParent(_interactable.transform);
                        recieverGameObject.transform.localPosition = Vector3.zero;
                        recieverGameObject.transform.localRotation = Quaternion.identity;
                        recieverGameObject.transform.localScale = Vector3.one;
                        recieverGameObject.name = $"{key} Interaction Handler";
                    }

                    InteractionReciever handler = recieverGameObject.GetComponent<InteractionReciever>();
                    if (handler == null)
                    {
                        Debug.LogError($"CreateInteractionHandler failed for key {key}. GameObject does not contain InteractionHandler.", _interactable);
                        ObjectUtility.DestroyAlways(recieverGameObject);
                        continue;
                    }



                    _interactable._recievers[key] = handler;
                }
            }

            RemoveUnusedRecievers();

            //Debug.Log($"Preloaded Interaction Handlers for {Name}. Count {_handlerLibrary.Count}", this);
        }

        void RemoveUnusedRecievers()
        {
            InteractionReciever[] allRecieversInChildren = _interactable.GetComponentsInChildren<InteractionReciever>();
            foreach (InteractionReciever childReciever in allRecieversInChildren)
            {
                // If the reciever is not in the library, destroy it
                if (!_interactable._recievers.ContainsKey(childReciever.InteractionType)
                    || _interactable._recievers[childReciever.InteractionType] != childReciever)
                {
                    ObjectUtility.DestroyAlways(childReciever.gameObject);
                }
            }
        }

        InteractionReciever GetRecieverInChildren(InteractionTypeKey key)
        {
            InteractionReciever[] recievers = _interactable.GetComponentsInChildren<InteractionReciever>();
            foreach (InteractionReciever reciever in recievers)
            {
                if (reciever.InteractionType == key)
                    return reciever;
            }
            return null;
        }
        #endregion
        #endregion

        void SetFlagsToDefault()
        {
            _isRegistered = false;
            _isPreloaded = false;
            _isInitialized = false;
        }

        public string BuildNameKey()
        {
            if (Name == string.Empty) _name = DEFAULT_NAME;
            if (Name == DEFAULT_NAME && _sprite != null)
                _name = _sprite.name;
            if (Key == string.Empty) _key = DEFAULT_KEY;
            return $"{_name} : {_key}";
        }

        public string BuildObjectName()
        {
            return $"{PREFIX} {BuildNameKey()}";
        }
    }
}