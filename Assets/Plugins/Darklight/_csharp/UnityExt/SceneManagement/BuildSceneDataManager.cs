using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    public class BuildSceneDataManager<TSceneData> : BuildSceneManager
        where TSceneData : BuildSceneData, new()
    {
        public const string DATA_PATH = "Assets/Resources/BuildSceneData";
        public const string DATA_FILENAME = "BuildSceneDataObject";

        [SerializeField]
        protected TSceneData[] buildSceneData = new TSceneData[0];

        public override void Awake()
        {
            base.Awake();
        }

        public override void Initialize()
        {
#if UNITY_EDITOR
            LoadBuildScenes();

            Type dataObjectType = typeof(BuildSceneDataObject<TSceneData>);
            BuildSceneDataObject<TSceneData> buildSceneDataSO =
                ScriptableObjectUtility.CreateOrLoadScriptableObject(
                    dataObjectType,
                    DATA_PATH,
                    DATA_FILENAME
                ) as BuildSceneDataObject<TSceneData>;
#endif
        }

        /// <summary>
        /// Subscribes to SceneManager events.
        /// </summary>
        void OnEnable()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <summary>
        /// Unsubscribes from SceneManager events.
        /// </summary>
        void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public virtual List<TSceneData> GetAllBuildSceneData()
        {
            return buildSceneData.ToList();
        }

        /// <summary>
        /// Retrieves the scene data for a given scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The scene data for the specified scene name.</returns>
        public TSceneData GetSceneData(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"{Prefix} Cannot get scene data for null or empty scene name.");
                return null;
            }

            // Get the scene data of the specified data type.
            TSceneData data = buildSceneData.ToList().Find(x => x.Name == sceneName);
            return data;
        }

        /// <summary>
        /// Retrieves the scene data for a given scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns>The scene data for the specified scene.</returns>
        public TSceneData GetSceneData(Scene scene)
        {
            return GetSceneData(scene.name);
        }

        /// <summary>
        /// Retrieves the data for the active scene.
        /// </summary>
        public TSceneData GetActiveSceneData()
        {
            Scene scene = SceneManager.GetActiveScene();
            return GetSceneData(scene.name);
        }

#if UNITY_EDITOR
        public override void LoadBuildScenes()
        {
            base.LoadBuildScenes();
            SaveBuildSceneData();
        }

        public override void ClearBuildScenes()
        {
            base.ClearBuildScenes();

            buildSceneData = new TSceneData[0];
        }

        /// <summary>
        /// Saves the build scene data by updating the paths of the BuildSceneData objects
        /// based on the paths in the EditorBuildSettingsScene array.
        /// </summary>
        void SaveBuildSceneData()
        {
            string[] buildScenePaths = this.buildScenePaths;
            for (int i = 0; i < buildScenePaths.Length; i++)
            {
                string scenePath = buildScenePaths[i];

                // If the build scene data array is null or the index is out of range, resize the array and initialize the data.
                if (this.buildSceneData.Length < i + 1)
                {
                    Array.Resize(ref this.buildSceneData, i + 1);
                    this.buildSceneData[i] = new TSceneData();
                    this.buildSceneData[i].InitializeData(scenePath);
                    continue;
                }

                // If the scene path is different from the path in the build scene data, initialize the data.
                if (this.buildSceneData[i].Path != scenePath)
                {
                    this.buildSceneData[i].InitializeData(scenePath);
                    continue;
                }
            }

            // Overwrite the build scene data.
            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Saved build scene data.");
        }

#endif
    }
}
