using Darklight.UnityExt.SceneManagement;
using UnityEngine;

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// Base class for scene data objects.
    /// </summary>
    public class BuildSceneDataObject<T> : ScriptableObject where T : BuildSceneData
    {
        public T[] buildSceneData = new T[0];
    }
}
