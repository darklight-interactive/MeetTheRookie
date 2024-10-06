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

    //  ================================ [[ BUILD SCENE DATA ]] ================================
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData
    {
        [SerializeField, ShowOnly] string _name = "Null Scene";
        [SerializeField, ShowOnly] string _path;

        public string Name => _name;
        public string Path
        {
            get => _path;
            set
            {
                _path = FormatPath(value);
                _name = FormatNameFromPath(_path);
            }
        }

        public BuildSceneData() { } // Empty constructor for serialization.
        public BuildSceneData(string path)
        {
            _path = path.Replace("\\", "/"); // Replace all backslashes with forward slashes
            _name = Path.Split('/').Last().Split('.').First(); // Get the name of the scene from the path
        }

        string FormatPath(string path) => path.Replace("\\", "/"); // Replace all backslashes with forward slashes
        string FormatNameFromPath(string path) => path.Split('/').Last().Split('.').First(); // Get the name of the scene from the path
    }

    //  ================================ [[ BUILD SCENE MANAGER ]] ================================
    public interface IBuildSceneManager
    {
        void Initialize();
        void LoadScene(string sceneName);
    }

    public abstract class BuildSceneManager<TData>
        : MonoBehaviourSingleton<BuildSceneManager<TData>>, IBuildSceneManager, IUnityEditorListener
            where TData : BuildSceneData, new()
    {
        const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";

        //  ================================ [[ FIELDS ]] ================================        
        Dictionary<string, TData> _sceneDataDict = new Dictionary<string, TData>();
        string[] _paths = new string[0];

        [SerializeField, ShowOnly] string _directory; // Serialized field for debugging purposes only.
        [SerializeField, NonReorderable] TData[] _data = new TData[0];

        //  ================================ [[ PROPERTIES ]] ================================
        public List<string> ScenePathList { get => _paths.ToList(); protected set => _paths = value.ToArray(); }
        public List<TData> SceneDataList { get => _data.ToList(); protected set => _data = value.ToArray(); }
        public List<string> SceneNameList => _data.Select(x => x.Name).ToList();

        //  ================================ [[ EVENTS ]] ================================
        public delegate void SceneChangeEvent(Scene oldScene, Scene newScene);
        public event SceneChangeEvent OnSceneChanged;

        //  ================================ [[ METHODS ]] ================================
        //  ---- ( Private Unity Methods ) ---- >>
        void OnEnable()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        //  ---- ( Protected Internal Handlers ) ---- >>
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
            TData[] tempData = new TData[_paths.Length];
            for (int i = 0; i < _paths.Length; i++)
            {
                if (_sceneDataDict.ContainsKey(_paths[i]))
                {
                    tempData[i] = _sceneDataDict[_paths[i]];
                }
                else
                {
                    tempData[i] = new BuildSceneData(_paths[i]) as TData;
                    _sceneDataDict.Add(_paths[i], tempData[i]);
                }
            }
            _data = tempData;

            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Loaded {_paths.Length} build scenes from directory. {BUILD_SCENE_DIRECTORY}");
#endif
        }

        /// <summary>
        /// Handles the active scene change event.
        /// </summary>
        /// <param name="oldScene">The old active scene.</param>
        /// <param name="newScene">The new active scene.</param>
        protected void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Debug.Log($"{Prefix} Active scene changed from {oldScene.name} to {newScene.name}.");
            OnSceneChanged?.Invoke(oldScene, newScene);
        }

        //  ---------------- [ Public Methods ] -----------------------------        
        // ---- ( IUnityEditorListener ) ----
        public virtual void OnEditorReloaded() => Initialize();

        // ---- ( MonoBehaviourSingleton ) ----
        public override void Initialize()
        {
            _directory = BUILD_SCENE_DIRECTORY;
            LoadBuildScenesFromDirectory();
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

        public void TryGetSceneDataByName<T>(string sceneName, out T sceneData)
            where T : TData
        {
            sceneData = _data.FirstOrDefault(x => x.Name == sceneName) as T;
        }

        public void TryGetSceneDataByPath<T>(string scenePath, out T sceneData)
            where T : TData
        {
            sceneData = _data.FirstOrDefault(x => x.Path == scenePath) as T;
        }
    }

    /// <summary>
    /// A generic version of the BuildSceneManager class that uses the BuildSceneData class as the data type.
    /// </summary>
    public class BuildSceneManager : BuildSceneManager<BuildSceneData> { }

    // ==================== [[ EDITOR ]] ====================================
#if UNITY_EDITOR
    [CustomEditor(typeof(BuildSceneManager<>), true)]
    public class BuildSceneManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        IBuildSceneManager _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (IBuildSceneManager)target;
            _script.Initialize();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
