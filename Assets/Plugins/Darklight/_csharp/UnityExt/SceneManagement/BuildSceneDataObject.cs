using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// Base class for scene data objects.
    /// </summary>
    public class BuildSceneDataObject<TSceneData> : ScriptableObject
        where TSceneData : BuildSceneData, new()
    {
        protected string[] buildScenePaths = new string[0];
        [SerializeField] protected TSceneData[] buildSceneData = new TSceneData[0];

        public virtual void Initialize()
        {
            for (int i = 0; i < buildScenePaths.Length; i++)
            {
                string scenePath = buildScenePaths[i];

                // If the current data array is smaller than the build scene paths array, or the path at the current index is different, create a new scene data object.
                if (this.buildSceneData.Length <= i || this.buildSceneData[i].Path != scenePath)
                {
                    TSceneData sceneData = new TSceneData();
                    SaveSceneData(sceneData);
                }

                this.buildSceneData[i].InitializeData(scenePath);
            }
        }

        public virtual void SaveSceneData(TSceneData sceneData)
        {
            if (sceneData == null)
            {
                Debug.LogWarning(
                    $"{this.name} Cannot save null scene data."
                );
                return;
            }

            // Check if the scene data already exists.
            TSceneData existingData = buildSceneData.ToList().Find(x => x.Name == sceneData.Name);
            if (existingData != null)
            {
                // Update the existing scene data.
                existingData = sceneData;
                sceneData.InitializeData(sceneData.Path);
            }
            else
            {
                // Add the scene data to the list.
                List<TSceneData> sceneDataList = buildSceneData.ToList();
                sceneDataList.Add(sceneData);
                buildSceneData = sceneDataList.ToArray();
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            Debug.Log($"{this.name} Saved scene data for {sceneData.Name}.");
        }

        public virtual List<TSceneData> GetData()
        {
            return buildSceneData.ToList();
        }

        /// <summary>
        /// Retrieves the scene data for a given scene name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <returns>The scene data for the specified scene name.</returns>
        public TSceneData GetSceneData(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning(
                    $"{this.name} Cannot get scene data for null or empty scene name."
                );
                return null;
            }

            // Get the scene data of the specified data type.
            TSceneData data = this.buildSceneData.ToList().Find(x => x.Name == sceneName);
            return data;
        }

        /// <summary>
        /// Retrieves the scene data for a given scene.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns>The scene data for the specified scene.</returns>
        public TSceneData GetSceneData(Scene scene)
        {
            return GetSceneData(scene.name);
        }

        /// <summary>
        /// Retrieves the data for the active scene.
        /// </summary>
        public TSceneData GetActiveSceneData()
        {
            Scene scene = SceneManager.GetActiveScene();
            return GetSceneData(scene.name);
        }

        public void ClearBuildSceneData()
        {
            buildSceneData = new TSceneData[0];
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            Debug.Log($"{this.name} Cleared build scene data.");
        }
    }
}
