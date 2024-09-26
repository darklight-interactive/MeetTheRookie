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

    public enum Type
    {
        BASE_INTERACTABLE,
        CHARACTER_INTERACTABLE,
        PLAYER_INTERACTOR,
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

    protected readonly List<State> VALID_INTERACTION_STATES = new List<State>
    {
        State.TARGET,
        State.START,
        State.CONTINUE
    };

    // -- (( DESTINATION POINTS )) -------- >>
    GameObject Lupe;
    GameObject Misra;

    [SerializeField, ShowOnly] bool _isPreloaded;
    [SerializeField, ShowOnly] bool _isRegistered;
    [SerializeField, ShowOnly] bool _isInitialized;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] InteractionRequestDataObject _request;
    [SerializeField] InteractionRecieverLibrary _recievers;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] InternalData _data;


    [HorizontalLine(color: EColor.Gray)]
    [Dropdown("dropdown_sceneKnotList"), SerializeField] string _sceneKnot = "scene_default";
    [Dropdown("dropdown_interactionStitchList"), SerializeField] string _interactionStitch = "interaction_default";

    [Header("Interactable")]
    [SerializeField] bool onStart;
    public bool isSpawn;



    [Header("Outline")]
    [SerializeField] Material _outlineMaterial;

    [Header("Destination Points")]
    [SerializeField] List<float> destinationPointsRelativeX;
    private List<GameObject> _destinationPoints = new List<GameObject>();

    protected InternalStateMachine stateMachine;
    protected SpriteRenderer spriteRenderer;
    protected new BoxCollider2D collider;
    #region ======== [[ PROPERTIES ]] ================================== >>>>
    protected List<string> dropdown_sceneKnotList
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }

    List<string> dropdown_interactionStitchList
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllStitchesInKnot(storyObject.StoryValue, _sceneKnot);
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
    public override Type TypeKey => Type.BASE_INTERACTABLE;

    #endregion

    #region ======== <PRIVATE_METHODS> ================================== >>>>
    protected virtual void PreloadSpriteRenderer()
    {
        // << GET OR ADD SPRITE RENDERER >> ------------------------------------
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // << SET THE DEFAULT TINT >> ------------------------------------
        spriteRenderer.color = Color.white;
    }
    protected virtual void PreloadBoxCollider()
    {
        collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        // << SET THE COLLIDER SIZE >> ------------------------------------
        // Set the collider size to half the size of the transform scale
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
        InteractionSystem.Factory.CreateOrLoadInteractionRequest(TypeKey.ToString(),
            out InteractionRequestDataObject newRequest, new List<InteractionType> { InteractionType.TARGET });
        Request = newRequest;
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
        // << RESET >> ------------------------------------
        Reset();

        // << PRELOAD COMPONENTS >> ------------------------------------
        PreloadSpriteRenderer();
        PreloadBoxCollider();
        PreloadData();
        PreloadStateMachine();

        // << VALIDATE PRELOAD >> ------------------------------------
        if (ValidatePreload())
            Register();
    }

    public override void Register()
    {
        // << REGISTER INTERACTABLE >> ------------------------------------
        InteractionSystem.Registry.TryRegisterInteractable(this, out bool inRegistry);
        GenerateRecievers();

        // << VALIDATE REGISTRATION >> ------------------------------------
        if (ValidateRegistration())
            Initialize();
    }

    public override void Initialize()
    {
        // << SET TO READY STATE >> ------------------------------------
        stateMachine.GoToState(State.READY);
        ValidateInitialization();

        // << DESTINATION POINTS >> ------------------------------------
        PlayerController tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        MTR_Misra_Controller tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        _destinationPoints.Clear();
        DestinationPoint[] childrenDestinationPoints = GetComponentsInChildren<DestinationPoint>();
        foreach (var destinationPoint in childrenDestinationPoints)
        {
            _destinationPoints.Add(destinationPoint.gameObject);
        }

        if (destinationPointsRelativeX == null || destinationPointsRelativeX.Count == 0)
        {
            destinationPointsRelativeX = new List<float> { -1, 1 };
        }
    }

    public override void Refresh()
    {
        if (StateMachine != null)
            StateMachine.Step();
    }

    public override void Reset()
    {
        _isPreloaded = false;
        _isRegistered = false;
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

        // << ACCEPT INTERACTION >> ------------------------------------
        Debug.Log($"{PREFIX} {Name} :: AcceptInteraction from {interactor}", this);
        switch (CurrentState)
        {
            case State.START:
            case State.CONTINUE:
                StateMachine.GoToState(State.CONTINUE, true);
                break;
            case State.COMPLETE:
            case State.DISABLED:
                break;
            default:
                StateMachine.GoToState(State.START);
                break;
        }

        return true;
    }
    #endregion

    void OnStart()
    {
        if (onStart)
        {
            MTRPlayerInteractor playerInteractor = FindFirstObjectByType<MTRPlayerInteractor>();
            playerInteractor.InteractWith(this, true);
        }
    }

    private void EnableOutline(bool enable)
    {
        /*
        if (_spriteRenderer != null)
        {
            _spriteRenderer.material = enable ? _outlineMaterial : null;
        }
        */
    }

    private IEnumerator FlashOutlineRoutine()
    {
        EnableOutline(true);
        yield return new WaitForSeconds(0.25f);
        EnableOutline(false);
    }

    // ====== [[ Destination Points ]] ======================================

    private void OnDrawGizmosSelected()
    {
        if (destinationPointsRelativeX == null) { return; }

        foreach (var destinationPoint in destinationPointsRelativeX)
        {
            Gizmos.DrawLine(new Vector3(transform.position.x + destinationPoint, -5, transform.position.z), new Vector3(transform.position.x + destinationPoint, 5, transform.position.z));
        }
    }

    public void SpawnDestinationPoints()
    {

        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        float lupeY = gameObject.transform.position.y;
        if (Lupe != null)
        {
            lupeY = Lupe.transform.position.y;
        }

        foreach (var point in _destinationPoints)
        {
            DestroyImmediate(point);
        }
        _destinationPoints.Clear();

        // Create new destination points
        for (int i = 0; i < destinationPointsRelativeX.Count; i++)
        {
            GameObject destinationPoint = new GameObject("Destination Point");
            destinationPoint.AddComponent<DestinationPoint>();
            destinationPoint.transform.position = new Vector3(gameObject.transform.position.x + destinationPointsRelativeX[i], lupeY, gameObject.transform.position.z);
            destinationPoint.transform.SetParent(this.transform);
            _destinationPoints.Add(destinationPoint);
        }
    }

    public void FindDestinationPoints()
    {
        _destinationPoints.Clear();


        DestinationPoint[] childrenDestinationPoints = GetComponentsInChildren<DestinationPoint>();
        foreach (var destinationPoint in childrenDestinationPoints)
        {
            _destinationPoints.Add(destinationPoint.gameObject);
        }
    }

    public List<GameObject> GetDestinationPoints()
    {
        return _destinationPoints;
    }

    private void ChangeSpawnPoints()
    {
        SpawnHandler spawnHandler = SpawnHandler.Instance;
        List<GameObject> interactables = spawnHandler.GetAllInteractables();

        foreach (var currentInteractable in interactables)
        {
            //currentInteractable.GetComponent<Interactable>().isSpawn = false;
        }

        isSpawn = true;

        spawnHandler.SetSpawnPoints(interactables);
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

            _script.Preload();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // << DRAW DEFAULT INSPECTOR >> ------------------------------------
            base.OnInspectorGUI();

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

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}