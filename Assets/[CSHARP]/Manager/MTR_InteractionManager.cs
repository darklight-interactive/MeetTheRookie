using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class MTR_InteractionManager : MonoBehaviourSingleton<MTR_InteractionManager>
{
    [Header("Interaction Layers")]
    [SerializeField, Layer] string _playerLayer = "Player";
    [SerializeField, Layer] string _npcLayer = "NPC";
    [SerializeField, Layer] string _interactableLayer = "Interactable";

    [Header("Interactors")]
    [SerializeField] PlayerInteractor _playerInteractor;

    [Header("Interactables")]
    [SerializeField] Library<string, Interactable> _interactableRegistry = new Library<string, Interactable>();

    [Header("Interaction Handler Prefabs")]
    [SerializeField] GameObject _iconInteractionHandlerPrefab;
    [SerializeField] GameObject _dialogueInteractionHandlerPrefab;
    [SerializeField] GameObject _choiceInteractionHandlerPrefab;

    public override void Initialize()
    {
        // Refresh InteractableRegistry if in Editor
        if (!Application.isPlaying)
        {
            RefreshInteractables();
            RefreshInteractors();
        }
    }

    #region ======== <PUBLIC_STATIC_METHODS> [[ INTERACTABLE REGISTRY ]] ================================== >>>>
    public static void RegisterInteractable(Interactable interactable)
    {
        // << ADD INTERACTABLE TO REGISTRY >>
        if (!Instance._interactableRegistry.ContainsKey(interactable.name))
            Instance._interactableRegistry.Add(interactable.name, interactable);

        // << BASE INTERACTABLE >>
        InitializeIconInteractionHandler(interactable);

        // << NPC INTERACTABLE >>
        if (interactable is NPC_Interactable npc)
        {
            InitializeDialogueInteractionHandler(npc);
            npc.gameObject.layer = LayerMask.NameToLayer(Instance._npcLayer);
        }
        else
        {
            interactable.gameObject.layer = LayerMask.NameToLayer(Instance._interactableLayer);
        }
    }

    static void RefreshInteractables()
    {
        Instance._interactableRegistry.Clear();
        Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (Interactable interactable in interactables)
        {
            RegisterInteractable(interactable);
        }
        Debug.Log($"{Prefix} Refreshed Interactable Registry : Found {Instance._interactableRegistry.Count} interactables", Instance);
    }

    static void RefreshInteractors()
    {
        Instance._playerInteractor = FindObjectsByType<PlayerInteractor>(FindObjectsSortMode.None).FirstOrDefault();
        if (Instance._playerInteractor == null)
        {
            Debug.LogWarning($"{Prefix} Player Interactor not found in scene", Instance);
        }
        else
        {
            RegisterPlayerInteractor(Instance._playerInteractor);
        }
    }

    #endregion

    #region ======== <PUBLIC_STATIC_METHODS> [[ INTERACTIOR REGISTRY ]] =========================== >>>>
    public static void RegisterPlayerInteractor(PlayerInteractor player)
    {
        if (Instance._playerInteractor != null)
        {
            Debug.LogWarning($"{Prefix} Player Interactor already registered", player);
            return;
        }
        Instance._playerInteractor = player;
        InitializeDialogueInteractionHandler(player);
        InitializeChoiceInteractionHandler(player);
        Debug.Log($"{Prefix} Registered Player Interactor {player.name}", player);
    }
    #endregion

    #region ======== <PRIVATE_STATIC_METHODS> [[ INITIALIZE INTERACTION HANDLER ]] =========================== >>>>
    static void InitializeIconInteractionHandler(Interactable interactable)
    {
        if (interactable.IconHandler != null) return;
        interactable.IconHandler = ObjectUtility.InstantiatePrefabAsComponent<IconInteractionHandler>(Instance._iconInteractionHandlerPrefab, interactable.transform);
    }

    static void InitializeDialogueInteractionHandler(PlayerInteractor player)
    {
        if (player.DialogueHandler != null) return;
        player.DialogueHandler = ObjectUtility.InstantiatePrefabAsComponent<DialogueInteractionHandler>(Instance._dialogueInteractionHandlerPrefab, player.transform);
        Debug.Log($"{Prefix} Instantiated Dialogue Interaction Handler for {player.name}", player);
    }

    static void InitializeDialogueInteractionHandler(NPC_Interactable npc)
    {
        if (npc.DialogueHandler != null) return;
        npc.DialogueHandler = ObjectUtility.InstantiatePrefabAsComponent<DialogueInteractionHandler>(Instance._dialogueInteractionHandlerPrefab, npc.transform);
        Debug.Log($"{Prefix} Instantiated Dialogue Interaction Handler for {npc.name}", npc);
    }

    static void InitializeChoiceInteractionHandler(PlayerInteractor player)
    {
        if (player.ChoiceHandler != null) return;
        player.ChoiceHandler = ObjectUtility.InstantiatePrefabAsComponent<ChoiceInteractionHandler>(Instance._choiceInteractionHandlerPrefab, player.transform);
        Debug.Log($"{Prefix} Instantiated Choice Interaction Handler for {player.name}", player);
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTR_InteractionManager))]
public class MTR_InteractionManagerCustomEditor : UnityEditor.Editor
{
    SerializedObject _serializedObject;
    MTR_InteractionManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTR_InteractionManager)target;
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

