using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.BuildScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Darklight.UnityExt.Editor;


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

        public virtual void CopyData(TData data)
        {
            _name = data.Name;
            _path = data.Path;
        }
    }

    public class BuildSceneScriptableData : BuildSceneScriptableData<BuildSceneData>
    {
    }
}
