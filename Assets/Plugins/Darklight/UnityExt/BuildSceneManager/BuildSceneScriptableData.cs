using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.BuildScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.BuildScene
{
    /// <summary>
    /// Base class for scene data objects.
    /// </summary>
    public class BuildSceneScriptableData : ScriptableObject, IBuildSceneData
    {
        bool _isValid;

        [Header("Build Scene Data")]
        [SerializeField, ReadOnly] SceneObject _sceneObject;
        [SerializeField, ShowOnly, HideIf("_isValid")] string _name;
        [SerializeField, ShowOnly, HideIf("_isValid")] string _path;

        public SceneObject SceneObject => _sceneObject;
        public string Name { get => _name; set => _name = value; }
        public string Path { get => _path; set => _path = value; }

        public virtual void Initialize(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (!IsValid())
            {
                _name = IBuildSceneData.ExtractNameFromPath(path);
                _path = IBuildSceneData.FormatPath(path);
            }

            Refresh();
        }

        public virtual void Copy(IBuildSceneData data)
        {
            _name = data.Name;
            _path = data.Path;
            Refresh();
        }

        public virtual void Refresh()
        {
            _sceneObject = Name;
        }

        public virtual bool IsValid()
        {
            bool isPathValid = !string.IsNullOrEmpty(_path);
            bool isNameValid = !string.IsNullOrEmpty(_name);
            return _isValid = isPathValid && isNameValid;
        }

    }
}
