using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Library;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public partial class InteractionSystem : MonoBehaviourSingleton<InteractionSystem>
{
    [SerializeField, Expandable] SystemSettings _settings;
    [SerializeField] Library<string, Interactable> _registry = new Library<string, Interactable>();


    public static SystemSettings Settings { get => Instance._settings; }

    public override void Initialize()
    {
        // Confirm Settings are loaded
        if (_settings == null)
            _settings = Factory.CreateSettings();
    }

    void Update()
    {
        _registry = Registry.GetLibrary();
    }

    public static class Factory
    {
        const string ASSET_PATH = "Assets/Resources/Darklight/InteractionSystem";
        public static SystemSettings CreateSettings()
        {
            SystemSettings settings = ScriptableObjectUtility.CreateOrLoadScriptableObject<SystemSettings>(ASSET_PATH, "InteractionSystemSettings");
            return settings;
        }
    }

    public static class Registry
    {
        static Library<string, Interactable> _library = new Library<string, Interactable>();

        #region ======== <PRIVATE_STATIC_METHODS > [[ Internal Methods ]] =========================== >>>>
        static void InitializeInteractable(Interactable interactable)
        {

        }

        static void RefreshInteractables()
        {
            _library.Clear();
            Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (Interactable interactable in interactables)
            {
                TryRegister(interactable);
            }
        }
        #endregion

        #region ======== <PUBLIC_STATIC_METHODS> [[ Public Handler Methods ]] =========================== >>>>
        public static bool TryRegister(Interactable interactable, bool overwrite = false)
        {
            if (!_library.ContainsKey(interactable.Key))
            {
                _library.Add(interactable.Key, interactable);
                return true;
            }
            else if (overwrite)
            {
                _library[interactable.Key] = interactable;
                return true;
            }
            return false;
        }

        public static Library<string, Interactable> GetLibrary()
        {
            return _library;
        }

        #endregion


        #region ==== (OLD_METHODS) ======== ))
        /*
        public static void RegisterInteractable(Interactable interactable)
        {
            // << ADD INTERACTABLE TO REGISTRY >>
            if (!Instance._registry.ContainsKey(interactable.name))
                Instance._registry.Add(interactable.name, interactable);

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
            Instance._registry.Clear();
            Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (Interactable interactable in interactables)
            {
                RegisterInteractable(interactable);
            }
            Debug.Log($"{Prefix} Refreshed Interactable Registry : Found {Instance._registry.Count} interactables", Instance);
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
        */
        #endregion


#if UNITY_EDITOR
        [CustomEditor(typeof(InteractionSystem))]
        public class InteractionSystemCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            InteractionSystem _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (InteractionSystem)target;
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
    }
}