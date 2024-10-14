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
    public abstract class BuildSceneScriptableDataManager<TScriptObj> : BuildSceneManager<TScriptObj>
        where TScriptObj : ScriptableObject, IBuildSceneData, new()
    {
        ScriptableSceneDataLibrary _objLibrary = new ScriptableSceneDataLibrary();

        [Header("Scriptable Data Manager ---- >>")]
        [SerializeField, ShowOnly] string _assetPath;
        [SerializeField, Expandable] TScriptObj _activeSceneScriptableData;
        [SerializeField] List<ExpandableSceneScriptableData> _fullSceneDataList = new List<ExpandableSceneScriptableData>();

        protected abstract string AssetPath { get; }

        protected override TScriptObj CreateData(string path)
        {
            string objName = IBuildSceneData.ExtractNameFromPath(path);
            TScriptObj obj = ScriptableObjectUtility.CreateOrLoadScriptableObject<TScriptObj>(AssetPath, objName);

            if (!obj.IsValid())
                obj.Initialize(path);
            else
                obj.Refresh();

            return obj;
        }

        public override void Initialize()
        {
            base.Initialize();
            _assetPath = AssetPath;
            _objLibrary = new ScriptableSceneDataLibrary()
            {
                ReadOnlyKey = true,
                RequiredKeys = NameList
            };

            RefreshScriptableData();
        }

        public override void Clear()
        {
            base.Clear();
            _fullSceneDataList.Clear();
        }

        /// <summary>
        /// Refreshes the scriptable data to match the scene data. <br/>
        /// Also recreates the expandable data list.
        /// </summary>
        public void RefreshScriptableData()
        {
            string debugStr = $"{Prefix} Loading scriptable objects from {AssetPath}";
            _fullSceneDataList.Clear();

            // Load all the scriptable objects
            foreach (string path in PathList)
            {
                // Load the scriptable object if it is not already loaded
                string name = IBuildSceneData.ExtractNameFromPath(path);
                _objLibrary.TryGetValue(name, out TScriptObj obj);
                if (obj == null)
                {
                    obj = _objLibrary[name] = CreateData(path);
                    debugStr += $"\n -- Loaded {obj.Name}";
                }

                // Add to the expandable list
                _fullSceneDataList.Add(new ExpandableSceneScriptableData(obj));
            }
            Debug.Log(debugStr);

            // Set the active scene scriptable data
            _activeSceneScriptableData = _objLibrary[ActiveSceneData.Name];
        }

        public virtual void SaveModifiedData(TScriptObj scriptObj)
        {
            if (scriptObj == null || scriptObj.name == null || scriptObj.name == string.Empty)
            {
                Debug.LogError($"{Prefix} >> Scriptable object is null or has no name.");
                return;
            }
            ReplaceData(scriptObj);
        }

        //  ---------------- [ Internal Library Class ] -----------------------------
        [Serializable]
        public class ScriptableSceneDataLibrary : Library<string, TScriptObj>
        {
            public ScriptableSceneDataLibrary()
            {
                ReadOnlyKey = true;
            }
        }

        [Serializable]
        public class ExpandableSceneScriptableData
        {
            [SerializeField, ShowOnly] string _sceneName;
            [SerializeField, Expandable] TScriptObj _data;

            public ExpandableSceneScriptableData(TScriptObj obj)
            {
                _data = obj;
                _sceneName = obj.name;
            }
        }
    }

    /// <summary>
    /// A base class for managing the build scene data for a project using scriptable objects.
    /// </summary>
    public class BuildSceneScriptableDataManager :
        BuildSceneScriptableDataManager<BuildSceneScriptableData>
    {
        protected override string AssetPath => "Assets/Resources/Darklight/BuildSceneData";
    }
}
