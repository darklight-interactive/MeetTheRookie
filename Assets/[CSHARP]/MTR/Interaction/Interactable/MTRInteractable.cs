using System.Collections;
using System.Collections.Generic;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;

using Ink.Runtime;

using NaughtyAttributes;

using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public partial class MTRInteractable : Interactable<MTRInteractable.InternalData, MTRInteractable.InternalStateMachine, MTRInteractable.State, MTRInteractable.Type>
{
    public const string DEFAULT_KNOT = "scene_default";
    public const string DEFAULT_STITCH = "interaction_default";

    protected readonly List<State> VALID_INTERACTION_STATES = new List<State>
    {
        State.TARGET,
        State.START,
        State.CONTINUE
    };

    [SerializeField, ShowOnly] bool _isPreloaded;
    [SerializeField, ShowOnly] bool _isRegistered;
    [SerializeField, ShowOnly] bool _isInitialized;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] InteractionRequestDataObject _request;
    [SerializeField] InteractionRecieverLibrary _recievers;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] InternalData _data;
    [SerializeField, ShowOnly] State _currentState;


    [HorizontalLine(color: EColor.Gray)]
    [Dropdown("dropdown_knotList"), SerializeField] string _sceneKnot = "scene_default";
    [Dropdown("dropdown_interactionStitchList"), SerializeField] string _interactionStitch = "interaction_default";

    [Header("Interactable")]
    [SerializeField] bool autoSizeCollider = true;

    protected InternalStateMachine stateMachine;
    protected SpriteRenderer spriteRenderer;
    protected new BoxCollider2D collider;



    #region ======== [[ PROPERTIES ]] ================================== >>>>
    protected List<string> dropdown_knotList
    {
        get
        {
            List<string> knots = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                knots = MTRStoryManager.Instance.KnotList;
            }
            return knots;
        }
    }
    List<string> dropdown_interactionStitchList
    {
        get
        {
            List<string> stitches = new List<string>(100);
            if (MTRStoryManager.Instance != null)
            {
                stitches = MTRStoryManager.GetAllStitchesInKnot(_sceneKnot);
            }
            return stitches;
        }
    }

    public override string Name
    {
        get
        {
            if (_data == null) return DEFAULT_NAME;
            return _data.Name;
        }
    }
    public override string Key
    {
        get
        {
            if (_data == null) return DEFAULT_KEY;
            return _data.Key;
        }
    }
    public override string Layer
    {
        get
        {
            if (_data == null) return DEFAULT_LAYER;
            return _data.Layer;
        }
    }
    public override InteractionRequestDataObject Request
    {
        get
        {
            return _request;
        }
        protected set => _request = value;
    }
    public override InteractionRecieverLibrary Recievers
    {
        get
        {
            return _recievers;
        }
        protected set => _recievers = value;
    }
    public override bool IsPreloaded
    {
        get
        {
            return _isPreloaded;
        }
        protected set => _isPreloaded = value;
    }
    public override bool IsRegistered
    {
        get
        {
            return _isRegistered;
        }
        protected set => _isRegistered = value;
    }
    public override bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
        protected set => _isInitialized = value;
    }
    public override InternalData Data => _data;
    public override InternalStateMachine StateMachine => stateMachine;
    public override State CurrentState
    {
        get
        {
            if (StateMachine == null) return State.NULL;
            return StateMachine.CurrentState;
        }
    }
    public override Type TypeKey => Type.BASE;
    public string InteractionStitch { get => _interactionStitch; set => _interactionStitch = value; }
    #endregion

    #region ======== <PRIVATE_METHODS> ================================== >>>>
    protected virtual void PreloadSpriteRenderer()
    {
        // << GET OR ADD SPRITE RENDERER >> ------------------------------------
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // << SET THE DEFAULT TINT >> ------------------------------------
        spriteRenderer.material = MTRGameManager.PrefabLibrary.spriteDefaultMaterial;
        spriteRenderer.color = Color.white;
    }
    protected virtual void PreloadBoxCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        // << SET THE COLLIDER SIZE >> ------------------------------------
        // Set the collider size to half the size of the transform scale
        if (autoSizeCollider)
            collider.size = Vector2.one * transform.localScale.x * 0.5f;
    }
    protected virtual void PreloadData()
    {
        if (_data == null)
            _data = new InternalData(this);
        else
            _data.LoadData(this);
    }
    protected virtual void PreloadStateMachine()
    {
        // << CREATE THE STATE MACHINE >> ------------------------------------
        stateMachine = new InternalStateMachine(this);
        stateMachine.OnStateChanged += (state) => _currentState = state;
    }
    protected virtual bool ValidatePreload()
    {
        bool spriteRendererValid = spriteRenderer != null;
        bool colliderValid = collider != null;
        bool dataValid = _data != null;
        bool stateMachineValid = stateMachine != null;
        bool isValid = spriteRendererValid && colliderValid && dataValid && stateMachineValid;

        if (!isValid)
        {
            string outLog = $"{PREFIX} {Name} :: Preload Validation Failed";
            if (!spriteRendererValid)
                outLog += " :: No Sprite Renderer Found";
            if (!colliderValid)
                outLog += " :: No Box Collider Found";
            if (!dataValid)
                outLog += " :: No Data Found";
            if (!stateMachineValid)
                outLog += " :: No State Machine Found";
            Debug.LogError(outLog, this);
        }

        return IsPreloaded = isValid;
    }
    protected virtual void GenerateRecievers()
    {
        Debug.Log($"{PREFIX} {Name} :: Recievers Generated", this);

        if (Request == null)
        {
            InteractionSystem.Factory.CreateOrLoadInteractionRequest(TypeKey.ToString(),
                out InteractionRequestDataObject newRequest, new List<InteractionType> { InteractionType.TARGET, InteractionType.DESTINATION });
            Request = newRequest;
        }

        InteractionSystem.Factory.GenerateInteractableRecievers(this);
    }
    protected virtual bool ValidateRegistration()
    {
        bool inRegistry = InteractionSystem.Registry.IsRegistered(this);
        bool requestValid = Request != null;
        bool recieversFound = Recievers != null && Recievers.Count > 0;
        bool recieversValid = Recievers.HasUnsetKeysOrValues() == false;
        bool isValid = inRegistry && requestValid && recieversFound && recieversValid;

        if (!isValid)
        {
            string outLog = $"{PREFIX} {Name} :: Preload Validation Failed";
            if (!inRegistry)
                outLog += " :: Not Registered";
            if (!requestValid)
                outLog += " :: No Request Found";
            if (!recieversFound)
                outLog += " :: No Recievers Found";
            if (!recieversValid)
                outLog += " :: Found Unset Recievers";
            Debug.LogError(outLog, this);
        }

        return IsRegistered = isValid;
    }

    protected virtual void InitializeStateMachine()
    {
        stateMachine = new InternalStateMachine(this);
        stateMachine.GoToState(State.READY);
    }

    protected virtual bool ValidateInitialization()
    {
        return IsInitialized = (CurrentState == State.READY);
    }
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IUnityEditorListener ]] ================================== >>>>
    public void OnEditorReloaded() => Preload();
    #endregion

    #region ======== <PUBLIC_METHODS> [[ IInteractable ]] ================================== >>>>
    public override void Preload()
    {
        _isPreloaded = false;

        //Debug.Log($"{PREFIX} {Name} :: Preload", this);

        // << RESET >> ------------------------------------
        Reset();

        // << PRELOAD COMPONENTS >> ------------------------------------
        PreloadSpriteRenderer();
        PreloadBoxCollider();
        PreloadData();
        PreloadStateMachine();

        // << VALIDATE PRELOAD >> ------------------------------------
        if (ValidatePreload())
        {
            Register();
        }
        else
        {
            Debug.LogError($"{PREFIX} {Name} :: Preload Failed.", this);
        }
    }

    public override void Register()
    {
        _isRegistered = false;

        //Debug.Log($"{PREFIX} {Name} :: Register", this);

        // << REGISTER INTERACTABLE >> ------------------------------------
        InteractionSystem.Registry.TryRegisterInteractable(this, out bool inRegistry);
        GenerateRecievers();

        // << VALIDATE REGISTRATION >> ------------------------------------
        if (ValidateRegistration())
        {
            Initialize();
        }
        else
        {
            Debug.LogError($"{PREFIX} {Name} :: Register Failed.", this);
        }
    }

    public override void Initialize()
    {
        _isInitialized = false;

        if (!IsPreloaded || !IsRegistered)
        {
            Debug.Log($"{PREFIX} {Name} :: Preload from Initialize", this);
            Preload();
            if (!IsPreloaded || !IsRegistered)
            {
                Debug.LogError($"{PREFIX} {Name} :: Initialize Failed.", this);
                return;
            }
        }

        Debug.Log($"{PREFIX} {Name} :: Initialize", this);

        InitializeStateMachine();

        // << SET TO READY STATE >> ------------------------------------
        ValidateInitialization();
    }

    public override void Refresh()
    {
        if (StateMachine != null)
            StateMachine.Step();
    }

    public override void Reset()
    {
        //_isPreloaded = false;
        //_isRegistered = false;
        _isInitialized = false;

        if (StateMachine == null)
            return;
        StateMachine.GoToState(State.READY);
    }

    public override bool AcceptTarget(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        // If not forced, check to make sure the interactable is in a valid state
        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (CurrentState != State.READY) return false;
        }

        // << ACCEPT TARGET >> ------------------------------------
        StateMachine.GoToState(State.TARGET);
        Debug.Log($"{PREFIX} {Name} :: AcceptTarget from {interactor}", this);
        return true;
    }

    public override bool AcceptInteraction(IInteractor interactor, bool force = false)
    {
        if (interactor == null) return false;

        if (!force)
        {
            // << CONFIRM VALIDITY >> ------------------------------------
            if (!VALID_INTERACTION_STATES.Contains(CurrentState)) return false;
        }

        if (interactor is MTRPlayerInteractor playerInteractor)
        {
            StartCoroutine(AcceptInteractionRoutine(playerInteractor, force));
        }

        return true;
    }
    #endregion

    IEnumerator AcceptInteractionRoutine(MTRPlayerInteractor player, bool force = false)
    {
        // << ACCEPT INTERACTION >> ------------------------------------
        Debug.Log($"{PREFIX} {Name} :: AcceptInteraction from {player}", this);
        MTRInteractionSystem.ResetAllInteractablesExcept(new List<MTRInteractable> { this, player });

        switch (CurrentState)
        {
            case State.START:
                StateMachine.GoToState(State.START, true);
                break;
            case State.CONTINUE:
                StateMachine.GoToState(State.CONTINUE, true);
                break;
            case State.COMPLETE:
            case State.DISABLED:
                //_destinations.TryRemoveOccupant(player);
                break;
            default:
                StateMachine.GoToState(State.START);
                break;
        }

        _currentState = CurrentState;
        yield return null;
    }

    // ====== [[ Enums ]] ======================================
    public enum Type
    {
        BASE,
        CHARACTER,
        PLAYER,
    }

    public enum State
    {
        NULL,
        READY,
        TARGET,
        START,
        CONTINUE,
        COMPLETE,
        DISABLED
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRInteractable), true)]
    public class InteractableCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        MTRInteractable _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRInteractable)target;

            if (!Application.isPlaying)
            {
                //_script.Preload();
            }
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // << BUTTONS >> ------------------------------------
            if (!_script.IsPreloaded)
            {
                if (GUILayout.Button("Preload"))
                {
                    _script.Preload();
                }
            }
            else if (!_script.IsRegistered)
            {
                if (GUILayout.Button("Register"))
                {
                    _script.Register();
                }
            }
            else if (!_script.IsInitialized)
            {
                if (GUILayout.Button("Initialize"))
                {
                    _script.Initialize();
                }
            }
            else
            {
                if (GUILayout.Button("Reset"))
                {
                    _script.Reset();
                }
            }

            // << DRAW DEFAULT INSPECTOR >> ------------------------------------
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}