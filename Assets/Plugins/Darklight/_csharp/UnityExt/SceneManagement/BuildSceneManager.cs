using System;
using System.Reflection;
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
    interface IBuildSceneManager
    {
        void LoadBuildScenesFromDirectory();
        void OnActiveSceneChanged(Scene oldScene, Scene newScene);
        void OnSceneLoaded(Scene scene, LoadSceneMode mode);
        void OnSceneUnloaded(Scene scene);
        void LoadScene(string sceneName);
        void LoadSceneAsync(string sceneName);
        void UnloadSceneAsync(string sceneName);
    }

    public abstract class BuildSceneManager : MonoBehaviourSingleton<BuildSceneManager>, IBuildSceneManager
    {
        protected const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";
        protected Scene[] buildScenes = new Scene[0]; // Unity Scenes

        public Scene ActiveScene => SceneManager.GetActiveScene();

        public delegate void ActiveSceneChanged(Scene oldScene, Scene newScene);
        public event ActiveSceneChanged OnSceneChanged;

        /// <summary>
        /// Handles the active scene change event.
        /// </summary>
        /// <param name="oldScene">The old active scene.</param>
        /// <param name="newScene">The new active scene.</param>
        public virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Debug.Log($"{Prefix} Active scene changed from {oldScene.name} to {newScene.name}.");
            OnSceneChanged?.Invoke(oldScene, newScene);
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The load scene mode.</param>
        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{Prefix} Scene {scene.name} loaded.");
        }

        /// <summary>
        /// Handles the scene unloaded event.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        public virtual void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"{Prefix} Scene {scene.name} unloaded.");
        }

        /// <summary>
        /// Loads a scene by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public virtual void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Loads a scene asynchronously by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load asynchronously.</param>
        public virtual void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Unloads a scene asynchronously by name.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload asynchronously.</param>
        public virtual void UnloadSceneAsync(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }


#if UNITY_EDITOR
        public virtual void LoadBuildScenesFromDirectory()
        {
            // Get all scene paths in the specified directory.
            string[] scenePaths = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);

            // Create arrays for the build scenes and the editor build settings scenes.
            Scene[] buildScenes = new Scene[scenePaths.Length];
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenePaths.Length];

            // Iterate through the scene paths.
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                buildScenes[i] = SceneManager.GetSceneByPath(scenePath);
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenePath, true);   
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes; // Update the editor build settings scenes.
            this.buildScenes = buildScenes; // Update the build scenes array.
        }
#endif
    }
}
