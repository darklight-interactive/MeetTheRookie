using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Darklight.UnityExt.SceneManagement
{
#if UNITY_EDITOR

    public class BuildSceneManagerWindow : EditorWindow
    {
        private BuildSceneManager buildSceneManager => BuildSceneManager.Instance;

        private Vector2 scrollPosition;
        private SerializedObject buildSceneManagerObject;
        private SerializedObject dataObjectSerializedObject;

        [MenuItem("DarklightExt/BuildSceneManager")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagerWindow>("BuildSceneManager");
        }

        void OnEnable()
        {
            buildSceneManagerObject = new SerializedObject(buildSceneManager);


        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        void OnGUI()
        {
            buildSceneManagerObject.Update();

            GUILayout.Space(10);

            GUILayout.Label("BuildSceneData", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                buildSceneManager.Initialize();
            }

            if (GUILayout.Button("Clear All Data"))
            {
                buildSceneManager.ClearBuildScenes();
            }

            if (GUILayout.Button("Open Build Settings"))
            {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("BuildSceneDataObject", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(buildSceneManagerObject.FindProperty("sceneDataObject"));

            // Start the scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);


            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                buildSceneManagerObject.ApplyModifiedProperties();
                Repaint();
            }

        }
    }
#endif
}
