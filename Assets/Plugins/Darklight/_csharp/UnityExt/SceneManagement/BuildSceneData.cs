

#if UNITY_EDITOR
using UnityEditor;

#endif

using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Darklight.UnityExt.SceneManagement
{
    /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData
    {
        [SerializeField] private Scene _scene;
        [SerializeField, ShowOnly] private string _name;
        [SerializeField, ShowOnly] private string _path;
        public Scene UnityScene
        {
            get { return _scene; }
            set { _scene = value; }
        }
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _scene.name;
                }
                return _name;
            }
        }
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    _path = _scene.path;
                }
                return _path;
            }
        }

        public BuildSceneData(){}
        public BuildSceneData(string path)
        {
            this._scene = SceneManager.GetSceneByPath(path);
        }
        public BuildSceneData(Scene scene)
        {
            this._scene = scene;
        }
    }
}