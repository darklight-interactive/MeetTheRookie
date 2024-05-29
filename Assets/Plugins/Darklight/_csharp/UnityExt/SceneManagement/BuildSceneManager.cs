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

        protected TSceneData activeScene;
        [SerializeField] protected TSceneData[] buildScenes = new TSceneData[0];

        /// <summary>
        /// Delegate for handling scene changes.
        /// </summary>
        /// <param name="oldScene">The old active scene data.</param>
        /// <param name="newScene">The new active scene data.</param>
        public delegate void SceneChanged(TSceneData oldScene, TSceneData newScene);
        public event SceneChanged OnSceneChange;

        public override void Initialize()
        {
            OnSceneChange += (TSceneData oldScene, TSceneData newScene) => 
            {
                activeScene = newScene;
            };

#if UNITY_EDITOR
            LoadBuildScenesFromDirectory(SCENE_DIRECTORY);
#endif
        }

        public virtual void Reset()
        {
            activeScene = null;
            buildScenes = new TSceneData[0];
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
            return buildScenes.FirstOrDefault(scene => scene.name == sceneName);
        }

#if UNITY_EDITOR
        public void LoadBuildScenesFromDirectory(string directoryPath)
        {
            // Get all scene paths in the specified directory.
            string[] scenePaths = Directory.GetFiles(directoryPath, "*.unity", SearchOption.AllDirectories);
            // Store a copy of the build scenes array.
            List<TSceneData> buildScenes = new List<TSceneData>(this.buildScenes);

            // Remove any scenes that are not in the directory.
            for (int i = 0; i < buildScenes.Count; i++)
            {
                if (!scenePaths.Contains(buildScenes[i].path))
                {
                    buildScenes.RemoveAt(i);
                }
            }

            // Create a new editor build settings scenes array.
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenePaths.Length]; 

            // Iterate through the scene paths.
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                string sceneName = scenePath.Replace($"{directoryPath}\\", "").Replace(".unity", "");

                // If the scene data object does not exist, create a new one.
                if (buildScenes[i].path != scenePath)
                {
                    // Create a new scene data object and add it to the build scenes array.
                    buildScenes[i] = new TSceneData()
                    {
                        path = scenePath,
                        name = sceneName
                    };
                }

                // Create an editor build settings scene object and add it to the editor build settings scenes array.
                editorBuildSettingsScenes[i] = buildScenes[i].CreateEditorBuildSettingsScene();
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes; // Update the editor build settings scenes.
            this.buildScenes = buildScenes.ToArray(); // Update the build scenes array.
        }
#endif

    }
}
