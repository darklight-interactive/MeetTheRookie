using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Darklight.UnityExt.SceneManagement;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Darklight.UnityExt.Editor
{
#if UNITY_EDITOR

    public class BuildSceneManagementWindow : EditorWindow
    {
        private BuildSceneManager buildSceneManager => BuildSceneManager.Instance;
        private Type _dataManagerType;
        private Type _dataType;

        private Vector2 scrollPosition;
        private SerializedObject serializedObject;

        private MethodInfo getAllScenesMethod;

        [MenuItem("DarklightExt/BuildScene Management")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagementWindow>("BuildScene Management");
        }

        void OnEnable()
        {
            serializedObject = new SerializedObject(buildSceneManager);

            // Get the manager type
            _dataManagerType = buildSceneManager.GetType();

            // Get the data type
            bool result = IsSubclassOfRawGeneric(typeof(BuildSceneDataManager<>), _dataManagerType);
            if (result)
            {
                _dataType = _dataManagerType.BaseType.GetGenericArguments()[0];
            }

            // Load the build scenes
            buildSceneManager.LoadBuildScenes();
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
            serializedObject = new SerializedObject(buildSceneManager);
            serializedObject.Update();

            if (_dataType == null)
            {
                GUILayout.Label("No BuildSceneDataManager found.");
                return;
            }

            GUILayout.Label("DataManager Type: " + _dataManagerType.Name);
            GUILayout.Label("Data Type: " + _dataType.Name);

            GUILayout.Space(10);

            GUILayout.Label("BuildScene Data", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

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

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buildSceneData"), true);
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
