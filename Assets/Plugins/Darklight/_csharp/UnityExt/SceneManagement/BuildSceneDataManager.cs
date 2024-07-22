using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    public class BuildSceneDataManager<TSceneData> : BuildSceneManager
        where TSceneData : BuildSceneData, new()
    {
        public const string DATA_PATH = "Assets/Resources/BuildSceneData";
        public const string DATA_FILENAME = "BuildSceneDataObject";

        private BuildSceneDataObject<TSceneData> tempBuildSceneDataObject;
        private List<TSceneData> _buildSceneData = new List<TSceneData>();

        public override void Awake()
        {
            base.Awake();
        }

        public override void Initialize()
        {
#if UNITY_EDITOR
            tempBuildSceneDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<BuildSceneDataObject<TSceneData>>(
                DATA_PATH,
                DATA_FILENAME
            );

            if (tempBuildSceneDataObject == null)
            {
                Debug.LogError($"{this.name} Failed to create or load build scene data object.");
                return;
            }

            base.LoadBuildScenes();
            SaveBuildSceneData(buildScenePaths);
#endif
        }

        /// <summary>
        /// Saves the build scene data by updating the paths of the BuildSceneData objects
        /// based on the paths in the EditorBuildSettingsScene array.
        /// </summary>
        public virtual void SaveBuildSceneData(string[] buildScenePaths)
        {

#if UNITY_EDITOR
            this.buildScenePaths = buildScenePaths;

            for (int i = 0; i < buildScenePaths.Length; i++)
            {
                string scenePath = buildScenePaths[i];

                // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
                if (this._buildSceneData.Count <= i || this._buildSceneData[i].Path != scenePath)
                {
                    _buildSceneData.Add(new TSceneData());
                    Debug.Log($"{this.name} -> Added new scene data object. {typeof(TSceneData).Name}");
                }

                // Initialize the scene data.
                _buildSceneData[i].InitializeData(scenePath);
                tempBuildSceneDataObject.SaveSceneData(_buildSceneData[i]);
            }


            EditorUtility.SetDirty(this);
            Debug.Log($"{this.name} Saved build scene data. {typeof(TSceneData).Name}");
#endif
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

#if UNITY_EDITOR

        public override void ClearBuildScenes()
        {
            tempBuildSceneDataObject.ClearBuildSceneData();
            base.ClearBuildScenes();
        }
#endif
    }
}
