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
    /// <summary>
    /// An abstract class for managing build scenes and related custom game data.
    /// </summary>
    /// <typeparam name="TSceneData"></typeparam>
    public abstract class BuildSceneManager<TSceneData> : MonoBehaviourSingleton<BuildSceneManager<TSceneData>> where TSceneData : BuildSceneData, new()
    {
        const string SCENE_DIRECTORY = "Assets/Scenes/Build";
        protected TSceneData activeScene;
        protected TSceneData[] buildScenes = new TSceneData[0];

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

            // Log the active scene change.
            if (oldSceneData != null)
            {
                Debug.Log($"{Prefix} Active scene changed from {oldScene.name} to {newScene.name}.");
            }
            else
            {
                Debug.Log($"{Prefix} Active scene changed to {newScene.name}.");
            }
        }

        /// <summary>
        /// Handles the scene loaded event.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The load scene mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{Prefix} Scene {scene.name} loaded.");
        }

        /// <summary>
        /// Handles the scene unloaded event.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"{Prefix} Scene {scene.name} unloaded.");
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
        public TSceneData GetSceneData(string sceneName)
        {
            TSceneData data = buildScenes.FirstOrDefault(scene => scene.name == sceneName);
            if (data == null)
            {
                // If the scene name is not null or empty, log an error.
                if (sceneName != null && sceneName != "")
                {
                    return null;
                }

                // If the scene name is null or empty, log a warning.
                Debug.LogWarning($"{Prefix} Cannot get scene data for null or empty scene name.");
                return null;
            }
            return data;
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
                if (buildScenes[i] == null || buildScenes[i].path != scenePath)
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

#if UNITY_EDITOR

public class BuildSceneManagementWindow : EditorWindow
{
    private Type sceneManagerType;
    private object sceneManagerInstance;
    private FieldInfo buildScenesField;
    private MethodInfo loadBuildScenesFromDirectoryMethod;
    private MethodInfo loadSceneMethod;
    private MethodInfo unloadSceneAsyncMethod;

    [MenuItem("Darklight/BuildSceneManagement")]
    public static void ShowWindow()
    {
        GetWindow<BuildSceneManagementWindow>("Scene Management");
    }

    private void OnEnable()
    {
        FindSceneManagerType();
        if (sceneManagerType != null)
        {
            sceneManagerInstance = FindFirstObjectByType(sceneManagerType);
            if (sceneManagerInstance != null)
            {
                buildScenesField = sceneManagerType.GetField("_buildScenes", BindingFlags.NonPublic | BindingFlags.Instance);
                loadBuildScenesFromDirectoryMethod = sceneManagerType.GetMethod("LoadBuildScenesFromDirectory", BindingFlags.Public | BindingFlags.Instance);
                loadSceneMethod = sceneManagerType.GetMethod("LoadScene", BindingFlags.Public | BindingFlags.Instance);
                unloadSceneAsyncMethod = sceneManagerType.GetMethod("UnloadSceneAsync", BindingFlags.Public | BindingFlags.Instance);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Management", EditorStyles.boldLabel);

        if (sceneManagerInstance == null)
        {
            GUILayout.Label("Scene Manager not found.");
            return;
        }

        if (GUILayout.Button("Load Scenes from Directory"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Scene Directory", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                loadBuildScenesFromDirectoryMethod.Invoke(sceneManagerInstance, new object[] { path });
            }
        }

        var buildScenes = (Array)buildScenesField.GetValue(sceneManagerInstance);
        if (buildScenes.Length > 0)
        {
            GUILayout.Label("Scenes in Build Settings:", EditorStyles.boldLabel);

            foreach (object sceneData in buildScenes)
            {
                string sceneName = sceneData.GetType().GetField("sceneName").GetValue(sceneData).ToString();
                string scenePath = sceneData.GetType().GetField("scenePath").GetValue(sceneData).ToString();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Scene Name: {sceneName}", GUILayout.Width(200));
                GUILayout.Label($"Path: {scenePath}", GUILayout.Width(400));
                if (GUILayout.Button("Load", GUILayout.Width(50)))
                {
                    loadSceneMethod.Invoke(sceneManagerInstance, new object[] { sceneName });
                }
                if (GUILayout.Button("Unload", GUILayout.Width(50)))
                {
                    unloadSceneAsyncMethod.Invoke(sceneManagerInstance, new object[] { sceneName });
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No scenes loaded.");
        }
    }

    /// <summary>
    /// Finds the type of the scene manager by scanning all loaded assemblies and identifying a subclass of BuildSceneManager.
    /// </summary>
    private void FindSceneManagerType()
    {
        // Get all loaded assemblies in the current application domain.
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Iterate through each assembly.
        foreach (Assembly assembly in assemblies)
        {
            // Get all types defined in the current assembly.
            Type[] types = assembly.GetTypes();

            // Iterate through each type.
            foreach (Type type in types)
            {
                // Check if the type is a subclass of BuildSceneManager<T> by using a generic type definition.
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BuildSceneManager<>))
                {
                    // Set the found type to the sceneManagerType variable.
                    sceneManagerType = type;
                    return;
                }
            }
        }
    }

}

#endif

}
