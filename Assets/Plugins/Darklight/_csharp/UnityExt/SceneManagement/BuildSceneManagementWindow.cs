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

        [MenuItem("DarklightExt/Build Scene Management")]
        public static void ShowWindow()
        {
            GetWindow<BuildSceneManagementWindow>("Scene Management");
        }

        void OnEnable()
        {
            buildSceneManager.LoadBuildScenes();
        }

        void OnGUI()
        {
            GUILayout.Label("Build Scenes", EditorStyles.boldLabel);
            

        }
    }
#endif

}

