using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt
{
    public static class SceneBuilderUtility
    {
        public static List<string> BuildScenesFromDirectory(string directoryPath)
        {
#if UNITY_EDITOR
            string[] scenePaths = Directory.GetFiles(directoryPath, "*.unity", SearchOption.AllDirectories);

            // Update the build settings with the found scenes
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (string path in scenePaths)
            {
                if (File.Exists(path))
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(path, true));
                }
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

            return scenePaths.ToList();
#endif
        }
    }
}

