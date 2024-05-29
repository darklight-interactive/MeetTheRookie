

#if UNITY_EDITOR
using UnityEditor;
#endif

using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.SceneManagement
{
        /// <summary>
    /// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
    /// </summary>
    [System.Serializable]
    public class BuildSceneData
    {
        [ShowOnly] public string name;
        [ShowOnly] public string path;

        public BuildSceneData() { }

        #if UNITY_EDITOR
        /// <summary>
        /// Creates an EditorBuildSettingsScene object from the scene data.
        /// </summary>
        /// <returns>The created EditorBuildSettingsScene object.</returns>
        public EditorBuildSettingsScene CreateEditorBuildSettingsScene()
        {
            return new EditorBuildSettingsScene(path, true);
        }
        #endif
    }
}