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
    public abstract class BuildSceneManager<TData> : MonoBehaviourSingleton<BuildSceneManager<TData>>, IUnityEditorListener
        where TData : BuildSceneData, new()
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
        [SerializeField, NonReorderable] TData[] _dataValues = new TData[0]; // Copy of the dictionary values.

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
            // Get all unity scene paths in the specified directory.
            this._pathKeys = Directory.GetFiles(BUILD_SCENE_DIRECTORY, "*.unity", SearchOption.AllDirectories);

            // << CREATE EDITOR BUILD SETTING SCENES >> -----------------------------------
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

            // << CREATE BUILD SCENE DATA >> -----------------------------------
            TData[] tempData = new TData[_pathKeys.Length];
            for (int i = 0; i < _pathKeys.Length; i++)
            {
                if (_dataMap.ContainsKey(_pathKeys[i]))
                {
                    tempData[i] = _dataMap[_pathKeys[i]];
                }
                else
                {
                    tempData[i] = new TData()
                    {
                        Path = _pathKeys[i]
                    };

                    _dataMap.Add(_pathKeys[i], tempData[i]);
                }

                tempData[i].Refresh();
            }
            _dataValues = tempData;

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

        // ---- ( Public Handlers ) ----
        public void TryAddScene(Scene scene, out bool result)
        {
            result = false;
            if (scene.IsValid())
            {
                string scenePath = scene.path;
                if (!_dataMap.ContainsKey(scenePath))
                {
                    TData data = new TData(scenePath);
                }
                else
                {
                    _dataMap[scenePath] = new TData();
                }
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