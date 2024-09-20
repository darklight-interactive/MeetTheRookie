using UnityEngine;

public interface IInteractable
{
    string Name { get; }
    string SceneKnot { get; }
    string InteractionStitch { get; }
    State CurrentState { get; }
    Sprite MainSprite { get; }
    IconInteractionHandler IconHandler { get; set; }

    // ===================== [[ EVENTS ]] =====================
    delegate void InteractionEvent();
    delegate void InteractionDialogueEvent(string text);


    // ===================== [[ METHODS ]] =====================
    void Initialize();
    bool AcceptTarget(IInteractor interactor);
    bool AcceptInteraction(IInteractor interactor);
    void Reset();

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