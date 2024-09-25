using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Library;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Editor;



#if UNITY_EDITOR
using UnityEditor;
#endif

public enum InteractionTypeKey
{
    SIMPLE,
    TOGGLE,
    TARGET,
    DIALOGUE,
    CHOICE
}

[ExecuteAlways]
public partial class InteractionSystem : MonoBehaviourSingleton<InteractionSystem>, IUnityEditorListener
{
    [SerializeField, Expandable] SystemSettings _settings;
    [SerializeField, Expandable] InteractionRequestPreset _interactable_interactionRequestPreset;

    [HorizontalLine(4, color: EColor.Gray)]
    [SerializeField] Library<string, Interactable> _interactableRegistry = new Library<string, Interactable>();

    public static SystemSettings Settings { get => Instance._settings; }
    public static InteractionRequestPreset InteractableInteractionRequestPreset { get => Instance._interactable_interactionRequestPreset; }

    public void OnEditorReloaded()
    {
        Registry.ResetRegistry();
    }

    public override void Initialize()
    {
        // Confirm Settings are loaded
        if (_settings == null)
            _settings = Factory.CreateSettings();

        if (_interactable_interactionRequestPreset == null)
            _interactable_interactionRequestPreset = Factory.CreateOrLoadRequestPreset();
    }

    void Update()
    {
        _interactableRegistry = Registry.GetLibrary();
    }

    public static void Invoke(IInteractionCommand command)
    {
        Invoker.ExecuteCommand(command);
    }

    #region == INVOKER <STATIC_CLASS> == [[ Command Invoker ]] =========================== >>>>
    public static class Invoker
    {
        static IInteractionCommand _command;

        public static void SetCommand(IInteractionCommand command)
        {
            _command = command;
        }

        public static void ExecuteCommand()
        {
            _command.Execute();
        }

        public static void ExecuteCommand(IInteractionCommand command)
        {
            SetCommand(command);
            ExecuteCommand();
        }
    }
    #endregion

    #region == FACTORY <STATIC_CLASS> == [[ Factory Methods ]] =========================== >>>>
    public static class Factory
    {
        const string ASSET_PATH = "Assets/Resources/Darklight/InteractionSystem";
        const string SETTINGS_PATH = ASSET_PATH + "/Settings";
        const string REQUEST_PRESET_PATH = ASSET_PATH + "/RequestPreset";

        public static SystemSettings CreateSettings()
        {
            string defaultName = "InteractionSystemSettings";
            SystemSettings settings = ScriptableObjectUtility.CreateOrLoadScriptableObject<SystemSettings>(ASSET_PATH, defaultName);
            return settings;
        }

        public static InteractionRequestPreset CreateOrLoadRequestPreset(string name = "InteractionRequestPreset")
        {
            InteractionRequestPreset preset = ScriptableObjectUtility.CreateOrLoadScriptableObject<InteractionRequestPreset>(REQUEST_PRESET_PATH, name);
            return preset;
        }

        public static InteractionRequestPreset CreateOrLoadRequestPreset(out InteractionRequestPreset preset, string name = "InteractionRequestPreset")
        {
            preset = CreateOrLoadRequestPreset(name);
            return preset;
        }
    }
    #endregion

    #region == REGISTRY <STATIC_CLASS> == [[ Interactable Registry ]] =========================== >>>>
    public static class Registry
    {
        public static PlayerInteractor PlayerInteractor { get; private set; }
        public static Library<string, Interactable> Interactables = new Library<string, Interactable>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = true
        };



        static void RefreshInteractables()
        {
            Interactables.Clear();
            Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (Interactable interactable in interactables)
            {
                TryRegisterInteractable(interactable);
            }
        }

        /// <summary>
        /// Attempt to register an interactable. <br/>
        /// 1. If the interactable is not in the library, add it. <br/>
        /// 2. If the interactable is in the library and the same reference, return true. <br/>
        /// 3. If the interactable is in the library and not the same reference, overwrite if allowed. <br/>
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool TryRegisterInteractable(Interactable interactable, bool overwrite = false)
        {
            // If the interactable is not in the library, add it
            if (!Interactables.ContainsKey(interactable.Key))
            {
                Interactables.Add(interactable.Key, interactable);
                Debug.Log($"{Prefix} Registered Interactable {interactable.Key}", interactable);
                return true;
            }
            // If the interactable is in the library and the same reference, return true
            else if (Interactables[interactable.Key] == interactable)
            {
                Debug.LogWarning($"{Prefix} Interactable {interactable.Key} already registered", interactable);
                return true;
            }
            // If the interactable is in the library and not the same reference, overwrite if allowed
            else if (overwrite)
            {
                Debug.LogWarning($"{Prefix} Overwriting Interactable {interactable.Key}", interactable);
                Interactables[interactable.Key] = interactable;
                return true;
            }
            return false;
        }

        public static void ResetRegistry()
        {
            Interactables.Clear();
        }

        public static bool RegisterPlayerInteractor(PlayerInteractor player)
        {
            if (PlayerInteractor != null)
            {
                Debug.LogWarning($"{Prefix} Player Interactor already registered", player);
                return false;
            }
            PlayerInteractor = player;
            return true;
        }

        public static Library<string, Interactable> GetLibrary()
        {
            return Interactables;
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
                    Repaint();
                    EditorUtility.SetDirty(_script);
                }
            }
        }
#endif
    }
}