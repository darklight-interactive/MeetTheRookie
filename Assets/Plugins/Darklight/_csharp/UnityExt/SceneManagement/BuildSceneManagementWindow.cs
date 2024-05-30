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
        private BuildSceneManager buildSceneManager => BuildSceneManager.Instance;
        private Vector2 scrollPosition;
        private SerializedObject serializedObject;

        [MenuItem("DarklightExt/BuildScene Management")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagementWindow>("BuildScene Management");
        }

        void OnEnable()
        {
            buildSceneManager.LoadBuildScenes();
        }

        void OnGUI()
        {
            serializedObject = new SerializedObject(buildSceneManager);
            serializedObject.Update();

            GUILayout.Label("BuildScene Data", EditorStyles.boldLabel);


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                buildSceneManager.LoadBuildScenes();
            }

            if (GUILayout.Button("Clear"))
            {
                buildSceneManager.ClearBuildScenes();
            }

            if (GUILayout.Button("Open Build Settings"))
            {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }
            EditorGUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_buildSceneData"));

            GUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
