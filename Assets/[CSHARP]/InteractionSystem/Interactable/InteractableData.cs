using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;

public partial class Interactable
{
    [Serializable]
    public class InternalData
    {
        #region -- (( CONSTANTS )) ------------------- >>
        public const string DEFAULT_NAME = "DefaultName";
        public const string DEFAULT_KEY = "DefaultKey";
        #endregion

        Interactable _interactable;
        SpriteRenderer _spriteRenderer;
        BoxCollider2D _collider;

        [SerializeField, ShowOnly] InteractableType _type = InteractableType.BASE;
        [SerializeField] string _name = DEFAULT_NAME;
        [SerializeField, ShowOnly] string _key = DEFAULT_KEY;
        [SerializeField, ShowOnly] string _layer;

        [Header("Flags")]
        [SerializeField, ShowOnly] bool _isRegistered;
        [SerializeField, ShowOnly] bool _isPreloaded;
        [SerializeField, ShowOnly] bool _isInitialized;


        [Header("Interaction System Data")]
        [SerializeField, Expandable] InteractionRequestDataObject _interactionRequest;


        [Header("Sprite Data")]
        [SerializeField] Sprite _sprite;
        [SerializeField] Color _defaultTint = Color.white;
        [SerializeField] Color _interactionTint = Color.yellow;

        public InteractableType Type => _type;
        public string Name
        {
            get
            {
                if (_name == null || _name == string.Empty)
                    _name = DEFAULT_NAME;
                return _name;
            }
            set
            {
                _name = value;
                _interactable.name = BuildObjectName();
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
        public InteractionRequestDataObject InteractionRequest => _interactionRequest;

        public InternalData(Interactable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
        {
            Preload(interactable, name, key);
        }

        public void SetInteractionRequest(InteractionRequestDataObject request)
        {
            _interactionRequest = request;
        }

        public bool Preload(Interactable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
        {
            SetFlagsToDefault();

            _interactable = interactable;
            _name = name;
            _key = key;

            // << SET THE INTERACTABLE TYPE >> ------------------------------------
            if (interactable is PlayerInteractor)
                _type = InteractableType.PLAYER;
            else if (interactable is NPC_Interactable)
                _type = InteractableType.NPC;
            else
                _type = InteractableType.BASE;

            // << SET THE INTERACTABLE LAYER >> ------------------------------------
            switch (_type)
            {
                case InteractableType.PLAYER:
                    _layer = InteractionSystem.Settings.PlayerLayer;
                    break;
                case InteractableType.NPC:
                    _layer = InteractionSystem.Settings.NPCLayer;
                    break;
                default:
                    _layer = InteractionSystem.Settings.InteractableLayer;
                    break;
            }
            _interactable.gameObject.layer = LayerMask.NameToLayer(_layer);


            // << PRELOAD GAME COMPONENTS >> ------------------------------------
            PreloadSpriteRenderer();
            PreloadBoxCollider();
            _defaultTint = Color.white;

            // << PRELOAD RECIEVERS >> ------------------------------------
            // Load the interaction request data object by interactable type
            InteractionSystem.Factory.CreateOrLoadInteractionRequestDataObject(_type, out _interactionRequest);
            InteractionSystem.Factory.GenerateInteractableRecievers(_interactable, _interactionRequest.Keys.ToList());

            // << VALIDATE PRELOAD >> ------------------------------------
            _isRegistered = InteractionSystem.Registry.TryRegisterInteractable(_interactable);
            _isPreloaded = ConfirmPreloadIsValid(true);
            if (_isPreloaded && _type == InteractableType.BASE)
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
            if (Key == string.Empty) _key = DEFAULT_KEY;
            return $"{_name} : {_key}";
        }

        public string BuildObjectName()
        {
            return $"{_type} {BuildNameKey()}";
        }
    }
}