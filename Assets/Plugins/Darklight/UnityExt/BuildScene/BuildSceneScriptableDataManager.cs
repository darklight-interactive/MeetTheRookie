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

        public TScriptObj GetActiveSceneScriptableData()
        {
            string scenePath = SceneManager.GetActiveScene().path;
            if (_scriptableDataLibrary.ContainsKey(scenePath))
            {
                return _scriptableDataLibrary[scenePath];
            }
            return null;
        }

        protected virtual void CreateOrLoadScriptableData(string scenePath, out TScriptObj obj)
        {
            TScriptObj tempObj;
            obj = null;

            // Create the object
            TData data = SceneDataDict.ContainsKey(scenePath) ? SceneDataDict[scenePath] : new TData();
            data.Path = scenePath;

            // Check if the object already exists
            if (_scriptableDataLibrary.ContainsKey(scenePath) && _scriptableDataLibrary[scenePath] != null)
            {
                tempObj = _scriptableDataLibrary[scenePath];
            }
            else
            {
                tempObj = ScriptableObjectUtility.CreateOrLoadScriptableObject<TScriptObj>(_assetPath, data.Name);
                _scriptableDataLibrary[scenePath] = tempObj; // Set the object value in the library
            }

            tempObj.Name = data.Name;
            tempObj.Path = data.Path;
            tempObj.SceneObject = data.Name;

            obj = tempObj;
        }

        public override void Initialize()
        {
            base.Initialize();

            _scriptableDataLibrary = new ScriptableDataLibrary();
            _scriptableDataLibrary.SetRequiredKeys(ScenePathList);
            _assetPath = AssetPath;
            RefreshScriptableData();
        }

        /// <summary>
        /// Refreshes the scriptable data to match the scene data. <br/>
        /// Also recreates the expandable data list.
        /// </summary>
        public void RefreshScriptableData()
        {
            _fullSceneDataList.Clear();
            foreach (string scenePath in ScenePathList)
            {
                CreateOrLoadScriptableData(scenePath, out TScriptObj obj);
                if (obj != null)
                {
                    _fullSceneDataList.Add(new ExpandableSceneScriptableData(obj));
                    SaveModifiedData(obj);
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
