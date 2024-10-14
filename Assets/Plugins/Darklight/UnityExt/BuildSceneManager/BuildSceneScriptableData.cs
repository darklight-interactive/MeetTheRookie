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
        Scene _scene;
        [SerializeField] SceneObject _sceneObject;
        [SerializeField, ShowOnly] string _name;
        [SerializeField, ShowOnly] string _path;
        [SerializeField, ShowOnly] bool _isValid;

        public Scene Scene => _scene;
        public SceneObject SceneObject => _sceneObject;
        public string Name { get => _name; set => _name = value; }
        public string Path { get => _path; set => _path = value; }

        public virtual void Copy(IBuildSceneData data)
        {
            _name = data.Name;
            _path = data.Path;

            Refresh();
        }

        public virtual void Refresh()
        {
            _scene = SceneManager.GetSceneByPath(_path);
            _sceneObject = Name;
        }
    }
}
