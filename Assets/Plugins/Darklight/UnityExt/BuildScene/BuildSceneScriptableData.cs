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

        public string Name { get => _name; set => _name = value; }
        public string Path { get => _path; set => _path = value; }
        public SceneObject SceneObject { get => _sceneObject; set => _sceneObject = value; }

        public abstract TData ToData();
        public abstract void Refresh();

        public virtual void CopyData(TData data)
        {
            Name = data.Name;
            Path = data.Path;

            _sceneObject = Name;
        }
    }

    public class BuildSceneScriptableData : BuildSceneScriptableData<BuildSceneData>
    {

        public override void CopyData(BuildSceneData data)
        {
            base.CopyData(data);
            SceneObject = Name;
        }

        public override BuildSceneData ToData()
        {
            return new BuildSceneData(Path);
        }

        public override void Refresh()
        {
            SceneObject = Name;
        }
    }
}
