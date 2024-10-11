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
        string[] _pathKeys = new string[0];

        [SerializeField, ShowOnly] string _directory; // Serialized field for debugging purposes only.
        [SerializeField, NonReorderable] TData[] _dataValues = new TData[0];

        //  ================================ [[ PROPERTIES ]] ================================
        public List<string> ScenePathList { get => _pathKeys.ToList(); protected set => _pathKeys = value.ToArray(); }
        public List<TData> SceneDataList { get => _dataValues.ToList(); protected set => _dataValues = value.ToArray(); }
        public List<string> SceneNameList
        {
            get
            {
                List<string> names = new List<string>();
                if (_dataValues != null && _dataValues.Length > 0)
                {
                    foreach (TData data in _dataValues)
                    {
                        if (data != null)
                            names.Add(data.Name);
                    }
                }
                return names;
            }
        }

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
                if (_sceneDataDict.ContainsKey(_pathKeys[i]))
                {
                    tempData[i] = _sceneDataDict[_pathKeys[i]];
                }
                else
                {
                    tempData[i] = new TData()
                    {
                        Path = _pathKeys[i]
                    };

                    _sceneDataDict.Add(_pathKeys[i], tempData[i]);
                }

                tempData[i].Refresh();
            }
            _dataValues = tempData;

            EditorUtility.SetDirty(this);
            Debug.Log($"{Prefix} Loaded {_pathKeys.Length} build scenes from directory. {BUILD_SCENE_DIRECTORY}");
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

        public void SetSceneData(TData data)
        {
            if (data == null)
            {
                Debug.LogError("Scene data is null.");
                return;
            }

            if (_sceneDataDict.ContainsKey(data.Path))
            {
                _sceneDataDict[data.Path] = data;
            }
            else
            {
                _sceneDataDict.Add(data.Path, data);
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

        public void TryGetSceneDataByName<T>(string sceneName, out T sceneData)
            where T : TData
        {
            if (_dataValues == null || _dataValues.Length == 0)
            {
                sceneData = null;
                return;
            }

            sceneData = _dataValues.FirstOrDefault(x => x.Name == sceneName) as T;
        }

        public void TryGetSceneDataByPath<T>(string scenePath, out T sceneData)
            where T : TData
        {
            if (_dataValues == null || _dataValues.Length == 0)
            {
                sceneData = null;
                return;
            }

            sceneData = _dataValues.FirstOrDefault(x => x.Path == scenePath) as T;
        }

        public void TryGetActiveSceneData<T>(out T sceneData)
            where T : TData
        {
            sceneData = null;
            if (_dataValues == null || _dataValues.Length == 0)
            {
                return;
            }

            //sceneData = _data.FirstOrDefault(x => x.Path == SceneManager.GetActiveScene().path) as T;
            string activeScenePath = SceneManager.GetActiveScene().path;
            foreach (TData data in _dataValues)
            {
                if (data != null && data.Path == activeScenePath)
                {
                    sceneData = data as T;
                    return;
                }
            }
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

            if (GUILayout.Button("Initialize")) { _script.Initialize(); }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
