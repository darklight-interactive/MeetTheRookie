using Darklight.UnityExt.Editor;
using UnityEngine;

public interface IInteractable
{
    string Name { get; }
    string Key { get; }
    string Layer { get; set; }

    // ===================== [[ EVENTS ]] =====================
    delegate void InteractionEvent();
    delegate void InteractionDialogueEvent(string text);

    event InteractionEvent OnReadyEvent;
    event InteractionEvent OnTargetEvent;
    event InteractionEvent OnStartEvent;
    event InteractionEvent OnContinueEvent;
    event InteractionEvent OnCompleteEvent;
    event InteractionEvent OnDisabledEvent;

    // ===================== [[ METHODS ]] =====================
    /// <summary>
    /// Preload the interactable with core data & subscriptions
    /// </summary>
    void Preload();

    /// <summary>
    /// Initialize the interactable within the scene by 
    /// storing scene specific references and data.
    /// </summary>
    void Initialize();
    void Refresh();
    void Reset();

    bool AcceptTarget(IInteractor interactor, bool force = false);
    bool AcceptInteraction(IInteractor interactor, bool force = false);



    // ===================== [[ NESTED TYPES ]] =====================
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
}