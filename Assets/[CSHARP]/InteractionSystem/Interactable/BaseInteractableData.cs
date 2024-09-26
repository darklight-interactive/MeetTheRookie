using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class BaseInteractableData
{
    #region -- (( CONSTANTS )) ------------------- >>
    public const string DEFAULT_NAME = "DefaultName";
    public const string DEFAULT_KEY = "DefaultKey";
    #endregion

    BaseInteractable _interactable;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _collider;

    [SerializeField, ShowOnly] BaseInteractableType _type = BaseInteractableType.BASE;
    [SerializeField] string _name = DEFAULT_NAME;
    [SerializeField, ShowOnly] string _key = DEFAULT_KEY;
    [SerializeField, ShowOnly] string _layer;

    [Header("Flags")]
    [SerializeField, ShowOnly] bool _isRegistered;
    [SerializeField, ShowOnly] bool _isPreloaded;
    [SerializeField, ShowOnly] bool _isInitialized;


    [Header("Interaction System Data")]
    [SerializeField] InteractionRequestDataObject _interactionRequest;


    [Header("Sprite Data")]
    [SerializeField] Sprite _sprite;

    public string Prefix => $"[{GetType().Name}]";
    public BaseInteractableType Type => _type;
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

    public BaseInteractableData(BaseInteractable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
    {
        Preload(interactable, name, key);
    }

    public void SetInteractionRequest(InteractionRequestDataObject request)
    {
        _interactionRequest = request;
    }

    public bool Preload(BaseInteractable interactable, string name = DEFAULT_NAME, string key = DEFAULT_KEY)
    {
        SetFlagsToDefault();

        _interactable = interactable;
        _name = name;
        _key = key;

        // << SET THE INTERACTABLE TYPE >> ------------------------------------
        if (interactable is PlayerInteractor)
            _type = BaseInteractableType.PLAYER;
        else if (interactable is NPC_Interactable)
            _type = BaseInteractableType.NPC;
        else
            _type = BaseInteractableType.BASE;

        // << SET THE INTERACTABLE LAYER >> ------------------------------------
        switch (_type)
        {
            case BaseInteractableType.PLAYER:
                _layer = InteractionSystem.Settings.PlayerLayer;
                break;
            case BaseInteractableType.NPC:
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

        // << PRELOAD RECIEVERS >> ------------------------------------
        // Load the interaction request data object by interactable type
        InteractionSystem.Factory.CreateOrLoadInteractionRequestDataObject(_type, out _interactionRequest);
        InteractionSystem.Factory.GenerateInteractableRecievers(_interactable, _interactionRequest.Keys.ToList());

        // << VALIDATE PRELOAD >> ------------------------------------
        _isRegistered = InteractionSystem.Registry.TryRegisterInteractable(_interactable);
        _isPreloaded = ConfirmPreloadIsValid(true);
        if (_isPreloaded && _type == BaseInteractableType.BASE)
            _interactable.name = BuildObjectName();
        return _isPreloaded;
    }


    public bool Initialize()
    {
        if (_interactable == null)
        {
            Debug.LogError($"{Prefix} {Name} :: Cannot Initialize Interactable. Interactable is null", _interactable);
            return false;
        }
        else if (!_isPreloaded)
        {
            if (Preload(_interactable) == false)
            {
                Debug.LogError($"{Prefix} {Name} :: Cannot Initialize Interactable. Preload Failed", _interactable);
                return false;
            }
        }

        Debug.Log($"{Prefix} Initialized Interactable {Name} :: {Key}", _interactable);
        return _isInitialized = true;
    }


    #region == [[ PRELOAD ]] ================ >>
    bool ConfirmPreloadIsValid(bool logErrors = false)
    {
        bool isValid = IsRegistered
            && _interactable.Recievers.Count > 0
            && !_interactable.Recievers.HasUnsetKeysOrValues();

        if (!isValid && logErrors)
        {
            string outLog = $"{Prefix} {Name} :: Preload Validation Failed";
            if (!IsRegistered)
                outLog += " :: Not Registered";
            if (_interactable.Recievers.Count == 0)
                outLog += " :: No Recievers Requested";
            if (_interactable.Recievers.HasUnsetKeysOrValues())
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
        _spriteRenderer.color = Color.white;
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