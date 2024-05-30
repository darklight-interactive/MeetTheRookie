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
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    interface IBuildSceneManager
    {
        void LoadBuildScenes();
        void OnActiveSceneChanged(Scene oldScene, Scene newScene);
        void OnSceneLoaded(Scene scene, LoadSceneMode mode);
        void OnSceneUnloaded(Scene scene);
        void LoadScene(string sceneName);
        void LoadSceneAsync(string sceneName);
        void UnloadSceneAsync(string sceneName);
    }

    public abstract class BuildSceneManager : MonoBehaviourSingleton<BuildSceneManager>, IBuildSceneManager
    {
        public const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";
        [SerializeField] private BuildSceneData[] _buildSceneData = new BuildSceneData[0];
        public List<BuildSceneData> BuildSceneDataList => _buildSceneData.ToList();

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
        public void LoadBuildScenes()
        {
            // Get all unity scene paths in the specified directory.
            string[] scenePaths = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);
            Debug.Log($"{Prefix} Found {scenePaths.Length} scenes in the build directory {BUILD_SCENE_DIRECTORY}.");

            // Create an array of EditorBuildSettingsScene objects.
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenePath, true);
            }

            // Assign the array to the editor build settings.
            EditorBuildSettings.scenes = editorBuildSettingsScenes;
            EditorUtility.SetDirty(this);

            SaveBuildSceneData();
        }

        /// <summary>
        /// Saves the build scene data by updating the paths of the BuildSceneData objects
        /// based on the paths in the EditorBuildSettingsScene array.
        /// </summary>
        void SaveBuildSceneData()
        {
            // Get the EditorBuildSettingsScene array
            EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;
            
            // Create a new list of BuildSceneData objects from _buildSceneData
            BuildSceneData[] buildSceneData = new BuildSceneData[editorBuildSettingsScenes.Length];
            
            // Iterate through each EditorBuildSettingsScene
            for (int i = 0; i < editorBuildSettingsScenes.Length; i++)
            {
                EditorBuildSettingsScene editorBuildSettingsScene = editorBuildSettingsScenes[i];
                buildSceneData[i] = new BuildSceneData(editorBuildSettingsScene.path);
            }

            this._buildSceneData = buildSceneData;
            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Saved build scene data.");
        }

        public void ClearBuildScenes()
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
            this._buildSceneData = new BuildSceneData[0];
            EditorUtility.SetDirty(this);

            Debug.Log($"{Prefix} Cleared build scenes.");
        }

        public static bool IsSceneInBuild(string path)
        {
            bool result = EditorBuildSettings.scenes.ToList().Exists(x => x.path == path);
            if (result) Debug.Log($"{Prefix} {path} is a build scene.");
            else Debug.LogWarning($"{Prefix} {path} is not a build scene.");
            return result;
        }
#endif
    }
}
