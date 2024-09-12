using System.Collections;
using System.Collections.Generic;

using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Game.Grid;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Utility;

using Ink.Runtime;

using NaughtyAttributes;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour, IInteract
{
    private const string Prefix = "[Interactable] >> ";
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();

    // private references for DestinationPoints
    private GameObject Lupe;
    private GameObject Misra;

    // private access to knots for dropdown
    private List<string> _sceneKnots
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllKnots(storyObject.StoryValue);
        }
    }

    // private access to stitches for dropdown
    private List<string> _interactionStitches
    {
        get
        {
            List<string> names = new List<string>();
            InkyStoryObject storyObject = InkyStoryManager.GlobalStoryObject;
            if (storyObject == null) return names;
            return InkyStoryObject.GetAllStitchesInKnot(storyObject.StoryValue, _sceneKnot);
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    //[HorizontalLine(color: EColor.Gray)]
    [Header("Interactable")]
    [SerializeField, ShowAssetPreview] Sprite _sprite;
    [SerializeField] private bool onStart;
    public bool isSpawn;

    [Header("InkyStory")]
    [Tooltip("The parent InkyStoryObject that this interactable belongs to. This is equivalent to a 'Level' of the game.")]

    [DropdownAttribute("_sceneKnots")]
    public string _sceneKnot = "scene1_0";

    [DropdownAttribute("_interactionStitches")]
    public string _interactionStitch;

    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive; [ShowOnly, SerializeField] bool _isComplete;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    [Header("Outline")]
    [SerializeField] Material _outlineMaterial;

    [Header("Destination Points")]
    [SerializeField] List<float> destinationPointsRelativeX;
    private List<GameObject> _destinationPoints = new List<GameObject>();

    [Header("Grid Spawner")]
    public Grid2D_OverlapWeightSpawner iconGridSpawner;

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    public string interactionKey { get => _interactionStitch; private set => _interactionStitch = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }

    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ------------------- [[ PUBLIC METHODS ]] ------------------- >>
    public virtual void Awake()
    {
        // << ICON GRID SPAWNER >> --------------------------------------
        if (iconGridSpawner == null)
        {
            // Get the icon spawner
            iconGridSpawner = GetComponentInChildren<Grid2D_OverlapWeightSpawner>();
            if (iconGridSpawner == null)
            {
                if (MTR_UIManager.Instance.iconGridSpawnerPrefab == null)
                {
                    Debug.LogError($"{Prefix} Icon Grid Spawner Prefab is null in MTR_UIManager", MTR_UIManager.Instance);
                    return;
                }

                // Create the icon spawner as a child
                iconGridSpawner = ObjectUtility.InstantiatePrefabWithComponent<Grid2D_OverlapWeightSpawner>
                    (MTR_UIManager.Instance.iconGridSpawnerPrefab, Vector3.zero, Quaternion.identity, this.transform);
                iconGridSpawner.transform.localPosition = Vector3.zero;

                Debug.Log($"{Prefix} Created Icon Grid Spawner", this);
            }
            else
            {
                Debug.Log($"{Prefix} Found Icon Grid Spawner", this);
            }
        }

    }


    public virtual void Start()
    {
        this.Reset();

        // << SET THE INITIAL SPRITE >> ------------------------------------
        // Prioritize the initial sprite that is set in the sprite renderer
        // Its assumed that the sprite renderer has a null sprite when the interactable is first created
        if (_spriteRenderer.sprite == null)
            _spriteRenderer.sprite = _sprite;
        else
            _sprite = _spriteRenderer.sprite;
        _spriteRenderer.color = _defaultTint;

        if (Application.isPlaying)
        {
            Invoke(nameof(OnStart), 0.1f);
        }

        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        var tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
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
    }

    void OnStart()
    {
        if (onStart)
        {
            PlayerInteractor playerInteractor = FindFirstObjectByType<PlayerInteractor>();
            playerInteractor.ForceInteract(this);
        }
    }

    public void Update()
    {

    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        UpdateInteractIcon();
    }

    public void UpdateInteractIcon()
    {
        if (isTarget && iconGridSpawner != null)
        {
            Cell2D cell = iconGridSpawner.GetBestCell();
            cell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

            if (MTR_UIManager.Instance != null)
                MTR_UIManager.Instance.ShowInteractIcon(position, dimensions.y * 2);
        }


    }

    public virtual void TargetClear()
    {
        isTarget = false;

        if (MTR_UIManager.Instance != null)
            MTR_UIManager.Instance.RemoveInteractIcon();
    }

    // ====== [[ INTERACTION ]] ======================================
    public virtual void Interact()
    {
        InkyStoryIterator StoryIterator = InkyStoryManager.Iterator;

        // << FIRST INTERACTION >>
        if (!isActive)
        {
            TargetClear();

            // Set the active flags
            isActive = true;
            isComplete = false;

            // Subscribe to OnInteraction
            OnInteraction += (string text) =>
            {
                MTR_UIManager.Instance.CreateNewSpeechBubble(text);
                MTR_AudioManager.Instance.PlayContinuedInteractionEvent();
            };

            // Subscribe to OnComplete
            OnCompleted += () =>
            {
                // Destroy the speech bubble
                MTR_UIManager.Instance.DestroySpeechBubble();
                MTR_AudioManager.Instance.PlayEndInteractionEvent();
            };

            // Go To the Interaction Stitch
            StoryIterator.GoToKnotOrStitch(_interactionStitch);

            // Color Flash
            StartCoroutine(ColorChangeRoutine(_interactionTint, 0.25f));

            OnFirstInteraction?.Invoke();
            ChangeSpawnPoints();
            MTR_AudioManager.Instance.PlayStartInteractionEvent();
            Debug.Log($"INTERACTABLE :: {name} >> First Interaction");
        }

        // << CONTINUE INTERACTION >> ------------------------------------
        StoryIterator.ContinueStory();

        // << LAST INTERACTION >> ----------------------------------------
        if (StoryIterator.CurrentState == InkyStoryIterator.State.END)
        {
            Complete();
        }
        else if (StoryIterator.CurrentState == InkyStoryIterator.State.CHOICE)
        {
            List<Choice> choices = StoryIterator.GetCurrentChoices();
            MTR_UIManager.Instance.gameUIController.LoadChoices(choices);
            Debug.Log($"INTERACTABLE :: {name} >> Choices Found");
        }
        else
        {
            OnInteraction?.Invoke(StoryIterator.CurrentText);
            Debug.Log($"INTERACTABLE :: {name} >> Continue Interaction");
        }
    }

    public virtual void Complete()
    {
        OnCompleted?.Invoke(); // Invoke OnCompleted
        Debug.Log($"INTERACTABLE :: {name} >> Complete");

        // Set the flags to complete
        isActive = false;
        isTarget = false;
        isComplete = true;

        // Reset the interactable after 1 second
        Invoke(nameof(Reset), 0.7f);
    }

    /// <summary>
    /// Reset all flags and values
    /// </summary>
    public virtual void Reset()
    {
        isTarget = false;
        isActive = false;
        isComplete = false;

        _spriteRenderer.color = _defaultTint;
    }

    public virtual void OnDestroy()
    {
        OnFirstInteraction = delegate { }; // Reset OnFirstInteraction
        OnInteraction = delegate { }; // Reset OnInteraction
        OnCompleted = delegate { }; // Reset OnCompleted
    }

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }

    private void EnableOutline(bool enable)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.material = enable ? _outlineMaterial : null;
        }
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
        if (destinationPointsRelativeX == null) {  return; }

        foreach (var destinationPoint in destinationPointsRelativeX)
        {
            Gizmos.DrawLine(new Vector3(transform.position.x + destinationPoint, -5, transform.position.z), new Vector3(transform.position.x + destinationPoint, 5, transform.position.z));
        }
    }

    public void Initialize()
    {
        if (destinationPointsRelativeX == null || destinationPointsRelativeX.Count == 0)
        {
            destinationPointsRelativeX = new List<float> { -1, 1 };
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
            currentInteractable.GetComponent<Interactable>().isSpawn = false;
        }

        isSpawn = true;

        spawnHandler.SetSpawnPoints(interactables);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable), true)]
public class InteractableCustomEditor : Editor
{
    SerializedObject _serializedObject;
    Interactable _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (Interactable)target;
        _script.Awake();
    }

    // This is the method that is called when the inspector is drawn
    public override void OnInspectorGUI()
    {
        _serializedObject = new SerializedObject(target);
        _script = (Interactable)target;
        _serializedObject.Update();

        // Checking to see if any values have changed
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI(); // Draw the default inspector

        if (GUILayout.Button("Spawn Destination Points"))
        {
            _script.SpawnDestinationPoints();
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif