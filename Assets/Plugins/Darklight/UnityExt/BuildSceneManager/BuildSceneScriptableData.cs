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
        public string Name = "None";
        public string Path = "None";
        public SceneObject SceneObject;

        public abstract TData ToData();
        public abstract void Refresh();
    }

    public class BuildSceneScriptableData : BuildSceneScriptableData<BuildSceneData>
    {
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
