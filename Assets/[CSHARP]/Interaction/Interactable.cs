using Darklight.Game.Grid;
using UnityEngine;
using System.Collections;
using static Darklight.UnityExt.CustomInspectorGUI;


#if UNITY_EDITOR
using UnityEditor;
#endif



public interface IInteract
{
    /// <summary>
    /// Called when the player is targeting the interactable object.
    /// </summary>
    void TargetEnable();

    /// <summary>
    /// Called to disable the interactable object and hide any prompts.
    /// </summary>
    void TargetDisable();

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Reset the interactable object to its default state.
    /// </summary>
    void Reset();

    delegate void OnInteract();
    delegate void OnComplete();
}

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour, IInteract
{
    [ShowOnly] public bool isActive = false;
    [ShowOnly] public bool isComplete = false;
    [SerializeField] private Transform promptIconTarget;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color defaultColor;
    public event IInteract.OnInteract OnInteraction;

    public event IInteract.OnComplete OnCompleted;

    // ====== [[ INITIALIZATION ]] ================================
    protected abstract void Initialize();

    public virtual void Reset()
    {
        isComplete = false;
        spriteRenderer.color = defaultColor;
    }

    // ====== [[ TARGETING ]] ======================================

    public virtual void TargetEnable()
    {
        Initialize();
        isActive = true;

        if (promptIconTarget == null)
            promptIconTarget = transform;
        UIManager.InteractionUI.DisplayInteractPrompt(promptIconTarget.position);
    }
    public virtual void TargetDisable()
    {
        isActive = false;
        UIManager.InteractionUI.HideInteractPrompt();
    }

    // ====== [[ INTERACTION ]] ===================================
    public virtual void Interact()
    {
        OnInteraction?.Invoke();
        StartCoroutine(ColorChangeRoutine(Color.red,2.0f));
    }
    public virtual void Complete()
    {
        OnCompleted?.Invoke();
        isComplete = true;
    }

    // ====== [[ MONOBEHAVIOUR ]] ===================================
    public virtual void Awake()
    {
        Initialize();
        defaultColor = spriteRenderer.color;
    }

    public abstract void OnDestroy();

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;     
    }
}