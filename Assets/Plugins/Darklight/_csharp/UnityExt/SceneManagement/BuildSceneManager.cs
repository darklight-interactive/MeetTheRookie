using System.IO;
using System.Linq;

using Darklight.UnityExt.Utility;

using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Editor;


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

    public abstract class BuildSceneManager : MonoBehaviourSingleton<BuildSceneManager>,
        IBuildSceneManager
    {
        public const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";
        public delegate void ActiveSceneChanged(Scene oldScene, Scene newScene);
        public event ActiveSceneChanged OnSceneChanged;

        [SerializeField] protected string[] buildScenePaths = new string[0];

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
        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("Scene name is empty or null.");
            }
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



        public virtual void LoadBuildScenes()
        {
#if UNITY_EDITOR
            // Get all unity scene paths in the specified directory.
            string[] buildScenePaths = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);

            // Create an array of EditorBuildSettingsScene objects.
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[buildScenePaths.Length];
            for (int i = 0; i < buildScenePaths.Length; i++)
            {
                buildScenePaths[i] = buildScenePaths[i].Replace("\\", "/"); // Replace all backslashes with forward slashes

                string scenePath = buildScenePaths[i];
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenePath, true);
            }

            // Assign the array to the editor build settings.
            EditorBuildSettings.scenes = editorBuildSettingsScenes;
            EditorUtility.SetDirty(this);

            this.buildScenePaths = buildScenePaths;
            //Debug.Log($"{Prefix} Found {buildScenePaths.Length} scenes in the build directory {BUILD_SCENE_DIRECTORY}.");
#endif
        }

        public virtual void ClearBuildScenes()
        {
            buildScenePaths = new string[0];
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
            EditorUtility.SetDirty(this);

            Debug.Log($"{Prefix} Cleared build scenes.");
        }

        public static bool IsSceneInBuild(string path)
        {
            bool result = EditorBuildSettings.scenes.ToList().Exists(x => x.path == path);
            return result;
        }
    }
}
