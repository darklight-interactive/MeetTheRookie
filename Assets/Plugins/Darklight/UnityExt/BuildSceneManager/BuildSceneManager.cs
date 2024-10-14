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
    public interface IBuildSceneData
    {
        bool IsValid { get; }
        Scene Scene { get; }
        string Name { get; }
        string Path { get; }

        IBuildSceneData BuildFromScene(Scene scene);
        IBuildSceneData BuildFromPath(string path);
        void Copy(IBuildSceneData data);
    }

    #region < PUBLIC_CLASS > [[ BuildSceneData ]] =========================
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData : IBuildSceneData
    {
        Scene _scene;
        [SerializeField, ShowOnly] string _name = "None";
        [SerializeField, ShowOnly] string _path = "None";
        [SerializeField, ShowOnly] bool _isValid;


        //  ---------------- [ PROPERTIES ] -----------------------------
        public Scene Scene => _scene;
        public string Name => _name;
        public string Path => _path;
        public bool IsValid => _isValid;


        // ---------------- [ CONSTRUCTORS ] -----------------------------
        public BuildSceneData() { } // Empty constructor for serialization.
        public BuildSceneData(string path) => BuildFromPath(path);
        public BuildSceneData(Scene scene) => BuildFromScene(scene);

        //  ---------------- [ Private Methods ] -----------------------------
        string FormatPath(string path) => path.Replace("\\", "/"); // Replace all backslashes with forward slashes
        string ExtractName(string path) => path.Split('/').Last().Split('.').First();

        //  ---------------- [ Public Abstract Methods ] -------------------------------
        public virtual IBuildSceneData BuildFromScene(Scene scene)
        {
            if (_scene.IsValid())
            {
                _scene = scene;
                _name = _scene.name;
                _path = FormatPath(_scene.path);
                _isValid = true;
            }
            return this;
        }

        public virtual IBuildSceneData BuildFromPath(string path)
        {
            _scene = SceneManager.GetSceneByPath(path);
            if (!_scene.IsValid())
            {
                _name = ExtractName(path);
                _path = FormatPath(path);
                _isValid = false;
            }
            return BuildFromScene(_scene);
        }

        public virtual void Copy(IBuildSceneData data)
        {
            _scene = data.Scene;
            _name = data.Name;
            _path = data.Path;
        }
    }
    #endregion

    public abstract class BuildSceneManager<TData> : MonoBehaviourSingleton<BuildSceneManager<TData>>, IUnityEditorListener
        where TData : IBuildSceneData, new()
    {
        const string BUILD_SCENE_DIRECTORY = "Assets/Scenes/Build";

        //  ================================ [[ STATIC PROPERTIES ]] =====================
        public static List<string> PathList => Instance._pathKeys.ToList();
        public static List<TData> DataList => Instance._dataValues.ToList();
        public static List<string> NameList => DataList.Select(x => x.Name).ToList();
        public static TData ActiveSceneData => Instance._activeSceneData;

        //  ================================ [[ FIELDS ]] ================================
        Scene _activeScene;
        string[] _pathKeys = new string[0];
        Dictionary<string, TData> _dataMap = new Dictionary<string, TData>();

        [Header("Build Scene Manager ---- >>")]
        [SerializeField, ShowOnly] string _directory;
        [SerializeField] TData _activeSceneData;
        [SerializeField, NonReorderable] TData[] _dataValues = new TData[0];


        //  ================================ [[ EVENTS ]] ================================
        public delegate void SceneChangeEvent(Scene oldScene, Scene newScene);
        public event SceneChangeEvent OnSceneChanged;

        //  ================================ [[ METHODS ]] ================================

        #region < PRIVATE_METHODS > [[ Unity Runtime ]] ============================= 
        void OnEnable()
        {
            SceneManager.activeSceneChanged += HandleActiveSceneChanged;
        }

        void OnDisable()
        {
            SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
        }
        #endregion

        #region < PRIVATE_METHODS > [[ Internal Handlers ]] =============================
        void LoadBuildScenesFromDirectory()
        {
#if UNITY_EDITOR
            //  ---- ( CREATE PATH KEYS ) ---- >>
            this._pathKeys = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);

            // << CREATE EDITOR BUILD SCENES >> -----------------------------------
            EditorBuildSettingsScene[] editorBuildSettingsScenes = new EditorBuildSettingsScene[_pathKeys.Length];
            for (int i = 0; i < _pathKeys.Length; i++)
            {
                // Replace all backslashes with forward slashes
                _pathKeys[i] = _pathKeys[i].Replace("\\", "/");

                // Create a new EditorBuildSettingsScene object and add it to the array.
                string scenePath = _pathKeys[i];
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


            //  ---- ( CREATE OR STORE DATA VALUES ) ---- >>
            _dataValues = new TData[_pathKeys.Length];
            for (int i = 0; i < _pathKeys.Length; i++)
            {
                if (_dataMap.ContainsKey(_pathKeys[i]))
                {
                    _dataValues[i] = _dataMap[_pathKeys[i]];
                }
                else
                {
                    _dataValues[i] = new TData();
                    _dataValues[i].BuildFromPath(_pathKeys[i]);
                    _dataMap.Add(_pathKeys[i], _dataValues[i]);
                }
            }

            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Loaded {_pathKeys.Length} build scenes from directory. {BUILD_SCENE_DIRECTORY}");
#endif
        }

        void UpdateActiveSceneData()
        {
            _activeScene = SceneManager.GetActiveScene();
            if (_dataValues == null || _dataValues.Length == 0)
                return;

            string activeScenePath = SceneManager.GetActiveScene().path;
            foreach (TData data in _dataValues)
            {
                if (data != null && data.Path == activeScenePath)
                {
                    _activeSceneData = data;
                    return;
                }
            }
        }

        void HandleActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            UpdateActiveSceneData();

            Debug.Log($"{Prefix} Active scene changed from {oldScene.name} to {newScene.name}.");
            OnSceneChanged?.Invoke(oldScene, newScene);
        }

        #endregion

        #region < PUBLIC_METHODS > ================================================================ 
        // ---- ( IUnityEditorListener ) ----
        public void OnEditorReloaded() => Initialize();

        // ---- ( MonoBehaviourSingleton ) ----
        public override void Initialize()
        {
            _directory = BUILD_SCENE_DIRECTORY;
            LoadBuildScenesFromDirectory();

            UpdateActiveSceneData();
        }

        public virtual void Clear()
        {
            _dataMap.Clear();
            _pathKeys = new string[0];
            _dataValues = new TData[0];

            _activeSceneData = default(TData);
        }

        // ---- ( Public Handlers ) ----
        public bool TryAddSceneData(Scene scene, out TData data)
        {
            data = default(TData);
            if (scene.IsValid())
            {
                data = new TData();

                // Check if the scene data already exists.
                string scenePath = scene.path;
                if (_dataMap.ContainsKey(scenePath))
                    data = _dataMap[scenePath];

                // Build the data from the given scene
                data.BuildFromScene(scene);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReplaceData(TData data)
        {
            if (data == null)
            {
                Debug.LogError("Scene data is null.");
                return;
            }

            if (_dataMap.ContainsKey(data.Path))
            {
                _dataMap[data.Path] = data;
            }
            else
            {
                _dataMap.Add(data.Path, data);
            }
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

        public void TryGetSceneDataByName(string sceneName, out TData sceneData)
        {
            if (_dataValues == null || _dataValues.Length == 0)
            {
                sceneData = default(TData);
                return;
            }

            sceneData = _dataValues.FirstOrDefault(x => x.Name == sceneName);
        }

        public void TryGetSceneDataByPath(string scenePath, out TData sceneData)
        {
            if (_dataValues == null || _dataValues.Length == 0)
            {
                sceneData = default(TData);
                return;
            }

            sceneData = _dataValues.FirstOrDefault(x => x.Path == scenePath);
        }
        #endregion
    }
}