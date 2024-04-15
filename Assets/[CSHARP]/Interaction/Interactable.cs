using Darklight.Game.Grid;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteract
{
    /// <summary>
    /// Initialize the interactable object with any necessary data.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Called when the player is targeting the interactable object.
    /// </summary>
    void Target();

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Called to disable the interactable object and hide any prompts.
    /// </summary>
    void Disable();

    /// <summary>
    /// Reset the interactable object to its default state.
    /// </summary>
    void Reset();
}

public abstract class Interactable : MonoBehaviour, IInteract
{
    [SerializeField] private Transform promptIconTarget;

    public abstract void Initialize();
    public virtual void Target()
    {
        UXML_InteractionUI.Instance.DisplayInteractPrompt(promptIconTarget.position);
    }

    public abstract void Interact();

    public virtual void Disable()
    {
        UXML_InteractionUI.Instance.HideInteractPrompt();
    }

    public abstract void Reset();
}