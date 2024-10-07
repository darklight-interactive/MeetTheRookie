using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;





#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    public interface IBuildSceneScriptableDataManager : IBuildSceneManager { }

    public abstract class BuildSceneScriptableDataManager<TData, TScriptObj> : BuildSceneManager<TData>, IBuildSceneScriptableDataManager
        where TData : BuildSceneData, new()
        where TScriptObj : BuildSceneScriptableData<TData>
    {
        ScriptableDataLibrary _library = new ScriptableDataLibrary();

        [Header("Scriptable Data Manager ---- >>")]
        [SerializeField, ShowOnly] string _assetPath;

        public List<ExpandableSceneScriptableData> expandableDataList = new List<ExpandableSceneScriptableData>();

        protected abstract string AssetPath { get; }

        public override void Initialize()
        {
            base.Initialize();
            _library.SetRequiredKeys(SceneNameList);
            _assetPath = AssetPath;

            RefreshScriptableData();
        }

        void RefreshScriptableData()
        {
            expandableDataList.Clear();
            foreach (string sceneName in SceneNameList)
            {
                CreateScriptableData(sceneName, out TScriptObj obj);

                if (obj != null)
                    expandableDataList.Add(new ExpandableSceneScriptableData(obj));
            }
        }

        void CreateScriptableData(string sceneName, out TScriptObj obj)
        {
            obj = null;

            // Check if the object already exists
            if (_library.ContainsKey(sceneName) && _library[sceneName] != null)
            {
                obj = _library[sceneName];
                return;
            }

            // Get the scene data
            TryGetSceneDataByName(sceneName, out TData sceneData);
            if (sceneData == null)
            {
                Debug.LogError($"{Prefix} Scene data for scene {sceneName} not found.");
                return;
            }

            // Create the object
            TScriptObj tempObj = ScriptableObjectUtility.CreateOrLoadScriptableObject<TScriptObj>(_assetPath, sceneName);
            tempObj.CopyData(sceneData); // Copy the scene data to the object
            _library[sceneName] = tempObj; // Set the object value in the library
            obj = tempObj;
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

        [Serializable]
        public class ExpandableSceneScriptableData
        {
            [SerializeField, ShowOnly] string _sceneName;
            [SerializeField, Expandable] TScriptObj _data;

            public ExpandableSceneScriptableData(TScriptObj data)
            {
                _data = data;
                _sceneName = data.name;
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
        }
    }
#endif

}
