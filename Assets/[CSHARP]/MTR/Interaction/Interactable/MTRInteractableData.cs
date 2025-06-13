using System;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class MTRInteractable
{
    [Serializable]
    public new class InternalData : InternalData<MTRInteractable>
    {
        const string DEFAULT_SCENE = "scene_default";
        const string DEFAULT_KEY = "interaction_default";
        const string DEFAULT_NAME = "default_interactable";

        [SerializeField, ShowOnly]
        string _scene = DEFAULT_SCENE;

        [SerializeField, ShowOnly]
        string _key = DEFAULT_KEY;

        [SerializeField, ShowOnly]
        string _name = DEFAULT_NAME;

        [SerializeField, ShowOnly]
        string _layer;

        [SerializeField, ShowOnly]
        Type _type = Type.BASE;

        [SerializeField, NaughtyAttributes.ReadOnly]
        Sprite _sprite;

        [SerializeField, ShowOnly]
        bool _isSpawnPoint = false;

        [SerializeField, ShowOnly]
        int _spawnIndex = 0;

        [SerializeField, ShowOnly]
        bool _spawnMisra = false;

        public override string Name => _name;
        public override string Key => _key;
        public override string Layer => _layer;

        public string SceneKnot => _scene;
        public Type Type => _type;
        public Sprite Sprite => _sprite;
        public bool IsSpawnPoint => _isSpawnPoint;
        public int SpawnIndex => _spawnIndex;
        public bool SpawnMisra => _spawnMisra;

        public InternalData(MTRInteractable interactable)
            : base(interactable)
        {
            LoadData(interactable);
        }

        /// <summary>
        /// Set the name of the interactable
        /// </summary>
        public void SetName(string name) => _name = name;

        /// <summary>
        /// Set the key value of the interactable. <br/>
        /// This is the value that will be used to identify the interactable in the story. <br/>
        /// If the interactable is a character, the key will be the character's name. <br/>
        /// /// Otherwise, the key will be the interactable's corresponding stitch.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(string key) => _key = key;

#if UNITY_EDITOR
        void CreateDataSO(
            MTRInteractable interactable,
            string sceneKnot,
            string interactionStitch,
            Sprite sprite
        )
        {
            // Create the directory if it doesn't exist
            string directoryPath = "Assets/Resources/MeetTheRookie/InteractableData";
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            // Find a DataSO with the same interaction stitch
            MTRInteractableDataSO existingDataSO =
                AssetDatabase.LoadAssetAtPath<MTRInteractableDataSO>(
                    $"{directoryPath}/{interactionStitch}.asset"
                );
            if (existingDataSO != null)
            {
                interactable._dataSO = existingDataSO;
                return;
            }

            // Create a new DataSO
            string assetPath = $"{directoryPath}/NewInteractableData.asset";

            // Create the ScriptableObject
            MTRInteractableDataSO newDataSO =
                ScriptableObject.CreateInstance<MTRInteractableDataSO>();

            // Save the asset
            AssetDatabase.CreateAsset(newDataSO, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Initialize the asset
            newDataSO.Initialize(sceneKnot, interactionStitch, sprite);
            interactable._dataSO = newDataSO;

            Debug.Log(
                $"Created new DataSO for {interactable.name} :: {assetPath}",
                interactable.gameObject
            );
        }
#else
        void CreateDataSO(
            MTRInteractable interactable,
            string sceneKnot,
            string interactionStitch,
            Sprite sprite
        )
        {
            Debug.LogWarning("CreateDataSO can only be called in the Unity Editor");
        }
#endif

        public override void LoadData(MTRInteractable interactable)
        {
            _type = interactable.TypeKey;

            // Handle player interactor
            if (interactable is MTRPlayerInteractor playerInteractor)
            {
                _key = playerInteractor.SpeakerTag.ToString();
                _layer = InteractionSystem.Settings.PlayerLayer;
                _name = $"PLAYER_{_key}";
                return;
            }

            // Handle character interactable
            if (interactable is MTRCharacterInteractable characterInteractable)
            {
                _key = characterInteractable.SpeakerTag.ToString();
                _layer = InteractionSystem.Settings.NPCLayer;
                string splitName = _key.Replace("Speaker.", "");
                _name = $"CHARACTER_{splitName}";
            }
            // Handle general interactable
            else if (interactable is MTRInteractable)
            {
                _layer = InteractionSystem.Settings.InteractableLayer;
                _name = $"INTRCT_{_key}";
            }
            // Handle unknown type
            else
            {
                _layer = DEFAULT_LAYER;
            }

            // Handle DataSO creation and assignment for non-player interactables
            if (interactable._dataSO == null)
            {
                // Create the DataSO with the current data values, which may be set already or if not, are set to default values
                CreateDataSO(interactable, _scene, _key, _sprite);
            }

            // Store the DataSO values
            _scene = interactable._dataSO.SceneKnot;
            _key = interactable._dataSO.InteractionStitch;
            _sprite = interactable._dataSO.Sprite;
            _isSpawnPoint = interactable._dataSO.IsSpawnPoint;
            _spawnIndex = interactable._dataSO.SpawnIndex;
            _spawnMisra = interactable._dataSO.SpawnMisra;

            // Set the layer
            interactable.gameObject.layer = LayerMask.NameToLayer(_layer);

            // Handle sprite assignment
            if (_sprite != null)
            {
                interactable.spriteRenderer.sprite = _sprite;
            }
            else if (interactable.spriteRenderer.sprite != null)
            {
                _sprite = interactable.spriteRenderer.sprite;
            }
        }
    }
}
