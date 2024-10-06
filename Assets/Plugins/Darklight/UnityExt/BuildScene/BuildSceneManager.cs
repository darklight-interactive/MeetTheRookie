using System.Collections.Generic;
using System.IO;
using System.Linq;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    public class BuildSceneManager : MonoBehaviourSingleton<BuildSceneManager>, IUnityEditorListener
    {
        const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";

        // ==================== [[ FIELDS ]] ====================
        Dictionary<string, BuildSceneData> _sceneDataDict = new Dictionary<string, BuildSceneData>();


        [SerializeField, ShowOnly] string _directory; // Serialized field for debugging purposes only.
        [SerializeField, NonReorderable, ShowOnly] string[] _paths = new string[0];
        [SerializeField, NonReorderable] BuildSceneData[] _data = new BuildSceneData[0];

        public List<string> Paths { get => _paths.ToList(); protected set => _paths = value.ToArray(); }


        public delegate void SceneChangeEvent(Scene oldScene, Scene newScene);
        public event SceneChangeEvent OnSceneChanged;

        // ==================== [[ METHODS ]] ====================
        //  ---- ( Internal Handlers ) ---- >>
        protected void LoadBuildScenesFromDirectory()
        {
#if UNITY_EDITOR
            // Get all unity scene paths in the specified directory.
            this._paths = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);

            // << CREATE EDITOR BUILD SETTING SCENES >> -----------------------------------
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[_paths.Length];
            for (int i = 0; i < _paths.Length; i++)
            {
                // Replace all backslashes with forward slashes
                _paths[i] = _paths[i].Replace("\\", "/");

                // Create a new EditorBuildSettingsScene object and add it to the array.
                string scenePath = _paths[i];
                editorBuildSettingsScenes[i] = new EditorBuildSettingsScene(scenePath, true);
                EditorBuildSettingsScene newEditorBuildSettingsScene = editorBuildSettingsScenes[i];

                /* Debug.Log($"{Prefix} Found scene {scenePath} and added it to the build settings." +
                    $"\nIndex: {i}" +
                    $"\nPath: {newEditorBuildSettingsScene.path}" +
                    $"\nEnabled: {newEditorBuildSettingsScene.enabled}" +
                    $"\nGUID: {newEditorBuildSettingsScene.guid}");
                */
            }
            EditorBuildSettings.scenes = editorBuildSettingsScenes;

            // << CREATE BUILD SCENE DATA >> -----------------------------------
            BuildSceneData[] tempData = new BuildSceneData[_paths.Length];
            for (int i = 0; i < _paths.Length; i++)
            {
                if (_sceneDataDict.ContainsKey(_paths[i]))
                {
                    tempData[i] = _sceneDataDict[_paths[i]];
                }
                else
                {
                    tempData[i] = new BuildSceneData(_paths[i]);
                    _sceneDataDict.Add(_paths[i], tempData[i]);
                }
            }
            _data = tempData;

            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Loaded {_paths.Length} build scenes from directory. {BUILD_SCENE_DIRECTORY}");
#endif
        }

        // ---- ( IUnityEditorListener ) ----
        public void OnEditorReloaded() => Initialize();

        // ---- ( MonoBehaviourSingleton ) ----
        public override void Initialize()
        {
            _directory = BUILD_SCENE_DIRECTORY;
            LoadBuildScenesFromDirectory();
        }

        /// <summary>
        /// Handles the active scene change event.
        /// </summary>
        /// <param name="oldScene">The old active scene.</param>
        /// <param name="newScene">The new active scene.</param>
        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
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



        //  ---------------- [[ Static Methods ]] ----------------------------- >>
        public static bool IsSceneInBuildSettings(string path)
        {
            bool result = false;
#if UNITY_EDITOR
            result = EditorBuildSettings.scenes.ToList().Exists(x => x.path == path);
#endif
            return result;
        }

        public static Scene GetSceneByPath(string path)
        {
            Scene result = default;
            if (IsSceneInBuildSettings(path))
            {
                result = SceneManager.GetSceneByPath(path);
            }
            return result;
        }


        // ==================== [[ EDITOR ]] ====================================
#if UNITY_EDITOR
        [CustomEditor(typeof(BuildSceneManager))]
        public class BuildSceneManagerCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            BuildSceneManager _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (BuildSceneManager)target;
                _script.Initialize();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button("Initialize"))
                {
                    _script.Initialize();
                }

                base.OnInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif


    }



}
