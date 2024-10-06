

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Darklight.UnityExt.BuildScene
{
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData
    {
        Scene _scene;

        [SerializeField, ShowOnly] string _name = "Null Scene";
        [SerializeField, ShowOnly] string _path;

        public string Name => _name;
        public string Path => _path;

        // ------------------- [[ CONSTRUCTORS ]] -------------------
        public BuildSceneData(string path)
        {
            _path = path.Replace("\\", "/"); // Replace all backslashes with forward slashes
            _name = Path.Split('/').Last().Split('.').First(); // Get the name of the scene from the path
        }
    }
}