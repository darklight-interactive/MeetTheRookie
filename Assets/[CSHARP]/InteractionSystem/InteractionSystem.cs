using System.Collections.Generic;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum InteractionType
{
    SIMPLE,
    TOGGLE,
    TARGET,
    DIALOGUE,
    CHOICE,
    DESTINATION
}

[ExecuteAlways]
public class InteractionSystem : MonoBehaviourSingleton<InteractionSystem>, IUnityEditorListener
{
    [SerializeField, Expandable]
    InteractionSystemSettings _settings;

    [HorizontalLine(4, color: EColor.Gray)]
    [SerializeField]
    Library<string, Interactable> _interactableRegistry = new Library<string, Interactable>();

    public static InteractionSystemSettings Settings
    {
        get => Instance._settings;
    }

    public void OnEditorReloaded()
    {
        Registry.ResetRegistry();
    }

    public override void Initialize()
    {
        // Confirm Settings are loaded
        if (_settings == null)
            _settings = Factory.CreateSettings();
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
        const string SETTINGS_NAME = "InteractionSystemSettings";

        const string REQUEST_PATH = ASSET_PATH + "/InteractionRequest";
        const string REQUEST_BASE_NAME = "InteractionRequest";

        public static InteractionSystemSettings CreateSettings()
        {
            InteractionSystemSettings settings =
                ScriptableObjectUtility.CreateOrLoadScriptableObject<InteractionSystemSettings>(
                    ASSET_PATH,
                    SETTINGS_NAME
                );
            return settings;
        }

        public static InteractionRequestDataObject CreateOrLoadInteractionRequest(
            string typeString,
            out InteractionRequestDataObject request,
            List<InteractionType> keys = null
        )
        {
            string name = $"{typeString} {REQUEST_BASE_NAME}";
            request =
                ScriptableObjectUtility.CreateOrLoadScriptableObject<InteractionRequestDataObject>(
                    REQUEST_PATH,
                    name
                );
            request.RequiredKeys = keys;
            request.Refresh();
            return request;
        }

        static void InstantiateInteractionReciever(
            Interactable interactable,
            InteractionType key,
            out GameObject gameObject
        )
        {
            GameObject prefab = interactable.Request.TryGetValue(key, out GameObject recieverPrefab)
                ? recieverPrefab
                : null;

            GameObject recieverGameObject = Instantiate(prefab, interactable.transform);
            if (recieverGameObject == null)
            {
                Debug.LogError(
                    $"CreateInteractionHandler failed for key {key}. GameObject is null.",
                    interactable
                );
                gameObject = null;
            }
            else
            {
                recieverGameObject.transform.localPosition = Vector3.zero;
                recieverGameObject.transform.localRotation = Quaternion.identity;
                recieverGameObject.transform.localScale = Vector3.one;
            }

            InteractionReciever reciever = recieverGameObject.GetComponent<InteractionReciever>();
            if (reciever == null)
            {
                Debug.LogError(
                    $"CreateInteractionHandler failed for key {key}. GameObject does not contain InteractionHandler.",
                    interactable
                );
                ObjectUtility.DestroyAlways(recieverGameObject);
                gameObject = null;
            }

            interactable.Recievers[key] = reciever;
            gameObject = recieverGameObject;
        }

        public static void GenerateInteractableRecievers(Interactable interactable)
        {
            //Debug.Log($"Generating recievers for {interactable.gameObject.name}");

            List<InteractionType> requestedKeys = interactable.Request.GetKeys();
            interactable.Recievers.Reset();

            foreach (InteractionType key in requestedKeys)
            {
                interactable.Recievers.TryGetValue(
                    key,
                    out InteractionReciever interactableReciever
                );
                if (interactableReciever == null)
                {
                    InteractionReciever recieverInChild = GetRecieverInChildren(interactable, key);
                    if (recieverInChild != null)
                    {
                        interactable.Recievers[key] = recieverInChild;
                        continue;
                    }

                    InstantiateInteractionReciever(
                        interactable,
                        key,
                        out GameObject recieverGameObject
                    );
                }
                else
                {
                    Debug.Log($"Reciever for {key} already exists", interactable);
                    //currRequestedReciever.transform.localPosition = Vector3.zero;
                }
            }

            RemoveUnusedRecievers(interactable);

            //Debug.Log($"Preloaded Interaction Handlers for {Name}. Count {_handlerLibrary.Count}", this);
        }

        public static void RemoveUnusedRecievers(Interactable interactable)
        {
            InteractionReciever[] allRecieversInChildren =
                interactable.GetComponentsInChildren<InteractionReciever>();
            foreach (InteractionReciever childReciever in allRecieversInChildren)
            {
                // If the reciever is not in the library, destroy it
                if (
                    !interactable.Recievers.ContainsKey(childReciever.InteractionType)
                    || interactable.Recievers[childReciever.InteractionType] != childReciever
                )
                {
                    ObjectUtility.DestroyAlways(childReciever.gameObject);
                }
            }
        }

        public static InteractionReciever GetRecieverInChildren(
            Interactable interactable,
            InteractionType key
        )
        {
            InteractionReciever[] recievers =
                interactable.GetComponentsInChildren<InteractionReciever>();
            foreach (InteractionReciever reciever in recievers)
            {
                if (reciever.InteractionType == key)
                    return reciever;
            }
            return null;
        }
    }
    #endregion

    #region == REGISTRY <STATIC_CLASS> == [[ Interactable Registry ]] =========================== >>>>
    public static class Registry
    {
        public static Library<string, Interactable> Interactables = new Library<
            string,
            Interactable
        >()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = true
        };

        static void ReloadInteractables()
        {
            Interactables.Clear();
            Interactable[] interactables = FindObjectsByType<Interactable>(
                FindObjectsSortMode.None
            );
            Debug.Log(
                $"{Prefix} Refreshing Interactable Registry : Found {interactables.Length} interactables",
                Instance
            );

            foreach (Interactable interactable in interactables)
            {
                if (interactable.IsPreloaded)
                    interactable.Preload();

                TryRegisterInteractable(interactable, out bool result);
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
        public static void TryRegisterInteractable(
            Interactable interactable,
            out bool result,
            bool overwrite = false
        )
        {
            result = false;
            if (Interactables.ContainsKey(interactable.Key))
            {
                // If the interactable is in the library and the same reference, return true
                if (Interactables[interactable.Key] == interactable)
                {
                    //Debug.Log($"{Prefix} Interactable {interactable.Data.BuildNameKey()} already registered", interactable);
                    result = true;
                }
                else if (Interactables[interactable.Key] == null)
                {
                    Debug.LogWarning(
                        $"{Prefix} Overwriting null value of Interactable {interactable.Print()}",
                        interactable
                    );
                    Interactables[interactable.Key] = interactable;
                    result = true;
                }
                else if (Interactables[interactable.Key] != null)
                {
                    if (overwrite)
                    {
                        Debug.Log(
                            $"{Prefix} Overwriting non-null value of Interactable {interactable.Print()}",
                            interactable
                        );
                        Interactables[interactable.Key] = interactable;
                        result = true;
                    }
                    else
                    {
                        Debug.LogError(
                            $"{Prefix} Interactable {interactable.Print()} already registered",
                            interactable
                        );
                        result = false;
                    }
                }
            }
            else
            {
                Interactables.Add(interactable.Key, interactable);
                Debug.Log($"{Prefix} Adding {interactable.Print()} to the Registry", interactable);
                result = true;
            }
        }

        public static bool IsRegistered(Interactable interactable)
        {
            return Interactables.ContainsKey(interactable.Key);
        }

        public static void ResetRegistry()
        {
            Interactables.Clear();
        }

        public static Library<string, Interactable> GetLibrary()
        {
            return Interactables;
        }

        public static void TryGetInteractable<TInteractable>(out TInteractable interactable)
            where TInteractable : Interactable
        {
            interactable = null;
            foreach (Interactable item in Interactables.Values)
            {
                if (item is TInteractable)
                {
                    interactable = (TInteractable)item;
                    break;
                }
            }
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

                if (GUILayout.Button("Refresh Registry"))
                {
                    Registry.ResetRegistry();
                    Registry.ReloadInteractables();
                }

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
