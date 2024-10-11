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
        ScriptableDataLibrary _scriptableDataLibrary = new ScriptableDataLibrary();

        [Header("Scriptable Data Manager ---- >>")]
        [SerializeField, ShowOnly] string _assetPath;
        [SerializeField, Expandable] TScriptObj _activeSceneScriptableData;
        [SerializeField] List<ExpandableSceneScriptableData> _fullSceneDataList = new List<ExpandableSceneScriptableData>();

        protected abstract string AssetPath { get; }

        public TScriptObj ActiveSceneScriptableData => _activeSceneScriptableData;

        protected virtual void CreateOrLoadScriptableData(string sceneName, out TScriptObj obj)
        {
            TScriptObj tempObj;
            obj = null;

            // Check if the object already exists
            if (_scriptableDataLibrary.ContainsKey(sceneName) && _scriptableDataLibrary[sceneName] != null)
            {
                tempObj = _scriptableDataLibrary[sceneName];
            }
            else
            {
                // Create the object
                tempObj = ScriptableObjectUtility.CreateOrLoadScriptableObject<TScriptObj>(_assetPath, sceneName);
                _scriptableDataLibrary[sceneName] = tempObj; // Set the object value in the library

                // Get the scene data
                /*
                TryGetSceneDataByName(sceneName, out TData sceneData);
                if (sceneData != null)
                {
                    tempObj.CopyData(sceneData); // Copy the scene data to the object
                }
                */
            }


            obj = tempObj;
        }

        public override void Initialize()
        {
            base.Initialize();
            _scriptableDataLibrary.SetRequiredKeys(SceneNameList);
            _assetPath = AssetPath;
            RefreshScriptableData();

            string activeSceneName = SceneManager.GetActiveScene().name;
            _activeSceneScriptableData = _scriptableDataLibrary.ContainsKey(activeSceneName) ? _scriptableDataLibrary[activeSceneName] : null;
        }

        /// <summary>
        /// Refreshes the scriptable data to match the scene data. <br/>
        /// Also recreates the expandable data list.
        /// </summary>
        public void RefreshScriptableData()
        {
            _fullSceneDataList.Clear();
            foreach (string sceneName in SceneNameList)
            {
                CreateOrLoadScriptableData(sceneName, out TScriptObj obj);

                if (obj != null)
                {
                    TryGetSceneDataByName(sceneName, out TData sceneData);
                    //obj.CopyData(sceneData);

                    _fullSceneDataList.Add(new ExpandableSceneScriptableData(obj));
                }
            }
        }

        public virtual void SaveModifiedData(TScriptObj scriptObj)
        {
            if (scriptObj == null || scriptObj.name == null || scriptObj.name == string.Empty)
            {
                Debug.LogError($"{Prefix} >> Scriptable object is null or has no name.");
                return;
            }

            SetSceneData(scriptObj.ToData());
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
