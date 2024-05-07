using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : OverlapGrid2D, IInteract
{

    // << SERIALIZED VALUES >> //
    [Header("Interactable Properties")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;
    [ShowOnly, SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Color _interactColor = Color.yellow;
    [SerializeField] string _interactionKey;

    // << PUBLIC ACCESSORS >> //
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public string interactionKey { get => _interactionKey; set => _interactionKey = value; }

    // << EVENTS >> //
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ====== [[ MONOBEHAVIOUR METHODS ]] =========================
    public override void Awake()
    {
        base.Awake();
        Initialize();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // ====== [[ INITIALIZATION ]] ================================
    protected abstract void Initialize();

    public virtual void Reset()
    {
        isComplete = false;
        _spriteRenderer.color = _interactColor;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetEnable()
    {
        isTarget = true;

        OverlapGrid2D_Data data = this.GetBestData();
        if (data == null) return;
        //UIManager.Instance.interactionUI.DisplayInteractPrompt(data.worldPosition);
    }

    public virtual void TargetDisable()
    {
        isTarget = false;
        //UIManager.Instance.interactionUI.HideInteractPrompt();
    }

    public InkyKnotIterator knotIterator;
    public virtual void Interact()
    {
        // >> TEMPORARY COLOR CHANGE
        StartCoroutine(ColorChangeRoutine(Color.red, 2.0f));

        // >> CONTINUE KNOT
        if (knotIterator == null)
            knotIterator = new InkyKnotIterator(InkyStoryManager.Instance.currentStory, _interactionKey);
        ContinueKnot();
    }

    public virtual void ContinueKnot()
    {
        isTarget = false;
        isActive = true;
        isComplete = false;

        knotIterator.ContinueKnot();

        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
            return;
        }

        OnInteraction?.Invoke(knotIterator.currentText);
    }

    public virtual void Complete()
    {
        isComplete = true;
        isActive = false;
        knotIterator = null;

        OnCompleted?.Invoke();
    }

    public abstract void OnDestroy();

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
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

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif