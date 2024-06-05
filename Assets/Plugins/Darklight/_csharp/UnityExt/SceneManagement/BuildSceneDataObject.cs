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

        public virtual void SaveData(BuildSceneData[] buildSceneData)
        {
            this.buildSceneData = buildSceneData.Cast<TSceneData>().ToArray();
            EditorUtility.SetDirty(this);
            Debug.Log($"{this.name} Saved build scene data.");
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
            EditorUtility.SetDirty(this);
            Debug.Log($"{this.name} Cleared build scene data.");
        }
    }
}
