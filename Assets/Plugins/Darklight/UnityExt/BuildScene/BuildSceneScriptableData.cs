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
    public abstract class BuildSceneScriptableData<TData> : ScriptableObject
        where TData : BuildSceneData, new()
    {
        [SerializeField, ShowOnly] string _name;
        [SerializeField, ShowOnly] string _path;
        [SerializeField, ReadOnly] SceneObject _sceneObject;

        public string Name { get => _name; protected set => _name = value; }
        public string Path { get => _path; protected set => _path = value; }

        public virtual void CopyData(TData data)
        {
            _name = data.Name;
            _path = data.Path;
            _sceneObject = _name;
        }

        public virtual TData ToData()
        {
            return new TData()
            {
                Path = _path,
            };
        }
    }

    public class BuildSceneScriptableData : BuildSceneScriptableData<BuildSceneData>
    {


    }
}
