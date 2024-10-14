using System.Collections.Generic;
using System.IO;
using System.Linq;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    public interface IBuildSceneData
    {
        string Name { get; set; }
        string Path { get; set; }

        void Initialize(string path);
        void Refresh();
        void Copy(IBuildSceneData data);
        bool IsValid();

        public static string FormatPath(string path) => path.Replace("\\", "/");
        public static string ExtractNameFromPath(string path) => path.Split('/').Last().Split('.').First();
    }

    #region < PUBLIC_CLASS > [[ BuildSceneData ]] =========================
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData : IBuildSceneData
    {
        [SerializeField, ShowOnly] string _name = "None";
        [SerializeField, ShowOnly] string _path = "None";

        //  ---------------- [ PROPERTIES ] -----------------------------
        public string Name { get => _name; set => _name = value; }
        public string Path { get => _path; set => _path = value; }

        // ---------------- [ METHODS ] -----------------------------
        public virtual void Initialize(string path)
        {
            _name = IBuildSceneData.ExtractNameFromPath(path);
            _path = IBuildSceneData.FormatPath(path);
            Refresh();
        }

        public virtual void Copy(IBuildSceneData data)
        {
            _name = data.Name;
            _path = data.Path;
        }

        public virtual void Refresh() { }

        public virtual bool IsValid()
        {
            bool isPathValid = !string.IsNullOrEmpty(_path);
            bool isNameValid = !string.IsNullOrEmpty(_name);
            return isPathValid && isNameValid;
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
        [SerializeField, ReadOnly, AllowNesting] TData _activeSceneData;
        [SerializeField, ReadOnly, AllowNesting] TData[] _dataValues = new TData[0];


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
                if (_dataMap.ContainsKey(_pathKeys[i]) && _dataMap[_pathKeys[i]] != null)
                {
                    if (!_dataMap[_pathKeys[i]].IsValid())
                        _dataMap[_pathKeys[i]] = CreateData(_pathKeys[i]);
                    _dataValues[i] = _dataMap[_pathKeys[i]];
                }
                else
                {
                    _dataValues[i] = CreateData(_pathKeys[i]);
                    _dataMap.Add(_pathKeys[i], _dataValues[i]);
                }
            }

            EditorUtility.SetDirty(this);
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

        #region < PROTECTED_METHODS > [[ Data Creation ]] =============================
        protected virtual TData CreateData(string path)
        {
            TData data = new TData();
            data.Initialize(path);
            return data;
        }

        protected virtual void RefreshData()
        {
            foreach (TData data in _dataValues)
            {
                if (data != null)
                {
                    data.Refresh();
                }
            }
            UpdateActiveSceneData();
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
            RefreshData();

            Debug.Log($"{Prefix} Loaded {_pathKeys.Length} build scenes from directory.");
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
                data = CreateData(scenePath);
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