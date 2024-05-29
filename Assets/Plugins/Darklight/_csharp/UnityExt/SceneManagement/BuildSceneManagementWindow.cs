using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Darklight.UnityExt.SceneManagement;
using System.Reflection;
using System;

namespace Darklight.UnityExt.Editor
{
#if UNITY_EDITOR

    public class BuildSceneManagementWindow : EditorWindow
    {
        private Type sceneManagerType;
        private object sceneManagerInstance;
        private FieldInfo buildScenesField;
        private MethodInfo loadBuildScenesFromDirectoryMethod;
        private MethodInfo loadSceneMethod;
        private MethodInfo unloadSceneAsyncMethod;

        [MenuItem("DarklightExt/Build Scene Management")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagementWindow>("Scene Management");
        }

        private void OnEnable()
        {

            sceneManagerInstance = FindFirstObjectByType(typeof(BuildSceneDataManager<>));
            if (sceneManagerInstance != null)
            {
                buildScenesField = sceneManagerType.GetField("buildScenes", BindingFlags.NonPublic | BindingFlags.Instance);
                loadBuildScenesFromDirectoryMethod = sceneManagerType.GetMethod("LoadBuildScenesFromDirectory", BindingFlags.Public | BindingFlags.Instance);
                loadSceneMethod = sceneManagerType.GetMethod("LoadScene", BindingFlags.Public | BindingFlags.Instance);
                unloadSceneAsyncMethod = sceneManagerType.GetMethod("UnloadSceneAsync", BindingFlags.Public | BindingFlags.Instance);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Management", EditorStyles.boldLabel);

            if (sceneManagerInstance == null)
            {
                GUILayout.Label("Scene Manager not found.");
                return;
            }

            if (GUILayout.Button("Load Scenes from Directory"))
            {
                string path = EditorUtility.OpenFolderPanel("Select Scene Directory", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    loadBuildScenesFromDirectoryMethod.Invoke(sceneManagerInstance, new object[] { path });
                }
            }

            Array buildScenes = (Array)buildScenesField.GetValue(sceneManagerInstance);
            if (buildScenes.Length > 0)
            {
                GUILayout.Label("Scenes in Build Settings:", EditorStyles.boldLabel);

                foreach (var sceneData in buildScenes)
                {
                    var sceneName = sceneData.GetType().GetField("sceneName").GetValue(sceneData).ToString();
                    var scenePath = sceneData.GetType().GetField("scenePath").GetValue(sceneData).ToString();

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"Scene Name: {sceneName}", GUILayout.Width(200));
                    GUILayout.Label($"Path: {scenePath}", GUILayout.Width(400));
                    if (GUILayout.Button("Load", GUILayout.Width(50)))
                    {
                        loadSceneMethod.Invoke(sceneManagerInstance, new object[] { sceneName });
                    }
                    if (GUILayout.Button("Unload", GUILayout.Width(50)))
                    {
                        unloadSceneAsyncMethod.Invoke(sceneManagerInstance, new object[] { sceneName });
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No scenes loaded.");
            }
        }
    }
#endif

}

