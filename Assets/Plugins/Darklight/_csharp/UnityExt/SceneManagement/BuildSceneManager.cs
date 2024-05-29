using System.Collections.Generic;
using System.Linq;

using Darklight.Utility;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// An abstract class for managing build scenes and related custom game data.
    /// </summary>
    /// <typeparam name="TSceneData"></typeparam>
    public abstract class BuildSceneManager<TSceneData> : MonoBehaviourSingleton<BuildSceneManager<TSceneData>> where TSceneData : BuildSceneData, new()
    {
        const string SCENE_DIRECTORY = "Assets/Scenes/Build";

        #region [[ STATIC METHODS ]] ====================== >>>>        
#if UNITY_EDITOR
        public void LoadBuildScenesFromDirectory(string directoryPath)
        {
            string[] scenePaths = Directory.GetFiles(directoryPath, "*.unity", SearchOption.AllDirectories);
            TSceneData[] buildScenes = new TSceneData[scenePaths.Length];
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenePaths.Length];

            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                string sceneName = scenePath.Replace($"{directoryPath}\\", "").Replace(".unity", "");
                buildScenes[i] = new TSceneData()
                {
                    path = scenePath,
                    name = sceneName
                };
                editorBuildSettingsScenes[i] = buildScenes[i].CreateEditorBuildSettingsScene();
            }

            _buildScenes = buildScenes;
        }
#endif
        #endregion

        [SerializeField, ShowOnly] TSceneData _activeScene;
        [SerializeField] TSceneData[] _buildScenes = new TSceneData[0];

        /// <summary>
        /// Delegate for handling scene changes.
        /// </summary>
        /// <param name="oldScene">The old active scene data.</param>
        /// <param name="newScene">The new active scene data.</param>
        public delegate void SceneChanged(TSceneData oldScene, TSceneData newScene);
        public event SceneChanged OnSceneChange;

        public override void Initialize()
        {
#if UNITY_EDITOR
            LoadBuildScenesFromDirectory(SCENE_DIRECTORY);
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

        /// <summary>
        /// Handles the active scene change event.
        /// </summary>
        /// <param name="oldScene">The old active scene.</param>
        /// <param name="newScene">The new active scene.</param>
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            TSceneData oldSceneData = GetSceneData(oldScene.name);
            TSceneData newSceneData = GetSceneData(newScene.name);
            OnSceneChange?.Invoke(oldSceneData, newSceneData);
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The load scene mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"Scene {scene.name} loaded.");
        }

        /// <summary>
        /// Handles the scene unloaded event.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"Scene {scene.name} unloaded.");
        }

        /// <summary>
        /// Loads a scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Loads a scene asynchronously by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load asynchronously.</param>
        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Unloads a scene asynchronously by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload asynchronously.</param>
        public void UnloadSceneAsync(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// Retrieves the scene data for a given scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The scene data for the specified scene name.</returns>
        private TSceneData GetSceneData(string sceneName)
        {
            return _buildScenes.FirstOrDefault(scene => scene.name == sceneName);
        }
    }
}
