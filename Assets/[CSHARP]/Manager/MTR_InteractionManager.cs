using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Utility;
using UnityEditor;
using UnityEngine;

public class MTR_InteractionManager : MonoBehaviourSingleton<MTR_InteractionManager>
{
    [Header("Interaction Handler Prefabs")]
    [SerializeField] GameObject _iconInteractionHandlerPrefab;
    [SerializeField] GameObject _dialogueInteractionHandlerPrefab;
    [SerializeField] GameObject _choiceInteractionHandlerPrefab;

    PlayerInteractor _playerInteractor;
    List<NPC_Interactable> _npcInteractables = new List<NPC_Interactable>();
    List<Interactable> _interactables = new List<Interactable>();

    public PlayerInteractor PlayerInteractor
    {
        get
        {
            if (_playerInteractor == null)
                _playerInteractor = FindObjectsByType<PlayerInteractor>(FindObjectsSortMode.None)[0];
            return _playerInteractor;
        }
    }
    public List<NPC_Interactable> NPCInteractables
    {
        get
        {
            if (_npcInteractables.Count == 0)
                _npcInteractables = FindObjectsByType<NPC_Interactable>(FindObjectsSortMode.None).ToList();
            return _npcInteractables;
        }
    }
    public List<Interactable> Interactables
    {
        get
        {
            if (_interactables.Count == 0)
                _interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None).ToList();
            return _interactables;
        }
    }

    public override void Initialize() { }

    #region ======== <STATIC_METHODS> [[ INITIALIZE INTERACTION HANDLER ]] ================================== >>>>
    public static IconInteractionHandler InitializeIconInteractionHandler(Interactable interactable)
    {
        if (interactable.IconHandler != null)
            return interactable.IconHandler;
        IconInteractionHandler iconHandler = ObjectUtility.InstantiatePrefabAsComponent<IconInteractionHandler>(Instance._iconInteractionHandlerPrefab, interactable.transform);
        Debug.Log($"{Prefix} Instantiated Icon Interaction Handler for {interactable.name}", interactable);
        return iconHandler;
    }

    public static DialogueInteractionHandler InitializeDialogueInteractionHandler(NPC_Interactable npc)
    {
        if (npc.DialogueHandler != null)
            return npc.DialogueHandler;
        DialogueInteractionHandler dialogueHandler = ObjectUtility.InstantiatePrefabAsComponent<DialogueInteractionHandler>(Instance._dialogueInteractionHandlerPrefab, npc.transform);
        Debug.Log($"{Prefix} Instantiated Dialogue Interaction Handler for {npc.name}", npc);
        return dialogueHandler;
    }

    public static DialogueInteractionHandler InitializeDialogueInteractionHandler(PlayerInteractor player)
    {
        if (player.DialogueHandler != null)
            return player.DialogueHandler;
        DialogueInteractionHandler dialogueHandler = ObjectUtility.InstantiatePrefabAsComponent<DialogueInteractionHandler>(Instance._dialogueInteractionHandlerPrefab, player.transform);
        Debug.Log($"{Prefix} Instantiated Dialogue Interaction Handler for {player.name}", player);
        return dialogueHandler;
    }

    public static ChoiceInteractionHandler InitializeChoiceInteractionHandler(PlayerInteractor player)
    {
        if (player.ChoiceHandler != null)
            return player.ChoiceHandler;
        ChoiceInteractionHandler choiceHandler = ObjectUtility.InstantiatePrefabAsComponent<ChoiceInteractionHandler>(Instance._choiceInteractionHandlerPrefab, player.transform);
        Debug.Log($"{Prefix} Instantiated Choice Interaction Handler for {player.name}", player);
        return choiceHandler;
    }
    #endregion

    #region ======== <STATIC_METHODS> [[ VISIT INTERACTABLES ]] ================================== >>>>



    #endregion
}