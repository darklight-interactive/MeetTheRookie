using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Editor;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    public interface IBuildSceneScriptableDataManager
    {
        void CreateObjects();
    }

    public abstract class BuildSceneScriptableDataManager<TData, TScriptObj> : BuildSceneManager<TData>, IBuildSceneScriptableDataManager
        where TData : BuildSceneData, new()
        where TScriptObj : BuildSceneScriptableData<TData>
    {
        [Header("Scriptable Data Manager ---- >>")]
        [SerializeField, ShowOnly] string _assetPath;

        [Space(10)]
        [SerializeField] ScriptableDataLibrary _library = new ScriptableDataLibrary();

        protected abstract string AssetPath { get; }

        public override void Initialize()
        {
            base.Initialize();
            _library.SetRequiredKeys(SceneNameList);
            _assetPath = AssetPath;
        }

        public void CreateObjects()
        {
            List<string> sceneNameKeys = _library.Keys.ToList();
            foreach (string sceneName in SceneNameList)
            {
                CreateScriptableData(sceneName);
            }
        }

        void CreateScriptableData(string sceneName)
        {
            // Check if the object already exists
            if (_library.ContainsKey(sceneName) && _library[sceneName] != null) return;

            // Get the scene data
            TryGetSceneDataByName(sceneName, out TData sceneData);
            if (sceneData == null)
            {
                Debug.LogError($"{Prefix} Scene data for scene {sceneName} not found.");
                return;
            }

            // Create the object
            TScriptObj obj = ScriptableObjectUtility.CreateOrLoadScriptableObject<TScriptObj>(_assetPath, sceneName);
            obj.CopyData(sceneData); // Copy the scene data to the object
            _library[sceneName] = obj; // Set the object value in the library
        }

        //  ---------------- [ Internal Library Class ] -----------------------------
        [Serializable]
        public class ScriptableDataLibrary : Library<string, TScriptObj>
        {
            public ScriptableDataLibrary()
            {
                ReadOnlyKey = true;
            }
        }
    }

    /// <summary>
    /// A base class for managing the build scene data for a project using scriptable objects.
    /// </summary>
    public class BuildSceneScriptableDataManager :
        BuildSceneScriptableDataManager<BuildSceneData, BuildSceneScriptableData>
    {
        protected override string AssetPath => "Assets/Resources/Darklight/BuildSceneData";
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BuildSceneScriptableDataManager<,>), true)]
    public class BuildSceneScriptableDataManagerCustomEditor : BuildSceneManagerCustomEditor
    {
        IBuildSceneScriptableDataManager _script => target as IBuildSceneScriptableDataManager;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Scriptable Scene Data"))
            {
                _script.CreateObjects();
            }
        }
    }
#endif

}
