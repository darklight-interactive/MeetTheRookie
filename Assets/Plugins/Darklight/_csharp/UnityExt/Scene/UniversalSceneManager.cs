using System.Collections.Generic;
using System.Linq;

using Darklight.Utility;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Darklight.UnityExt.Scene
{
    /// <summary>
    /// Universal Scene Manager, used to manage scenes in all aspects
    /// </summary>
    public class UniversalSceneManager : MonoBehaviourSingleton<UniversalSceneManager>
    {
        [SerializeField, ShowOnly] string sceneBuildDirectory = "Assets/Scenes/Build"; // Path relative to Assets folder
        [SerializeField, ShowOnly] string currentScene;
        [SerializeField, ShowOnly] private List<string> scenesInBuild = new List<string>();

        public string SceneDirectory => sceneBuildDirectory;
        public List<string> ScenesInBuild { get => scenesInBuild; set => scenesInBuild = value; }
        public string CurrentScene { get => currentScene; set => currentScene = value; }

        public override void Initialize()
        {
            LoadBuildScenes();
            currentScene = SceneManager.GetActiveScene().name;
        }

        public bool IsSceneInBuild(string scenePath)
        {
            return scenesInBuild.Contains(scenePath);
        }

        public bool GoToScene(string sceneName)
        {
            if (scenesInBuild.Count == 0)
            {
                Debug.LogError("No scenes in build settings");
                return false;
            }

            string scenePath = scenesInBuild.Find(s => Path.GetFileNameWithoutExtension(s) == sceneName);
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError($"Scene {sceneName} not found in build settings");
                return false;
            }

            SceneManager.LoadScene(sceneName);
            currentScene = sceneName;
            return true;
        }

        private void LoadBuildScenes()
        {
#if UNITY_EDITOR
            string[] scenePaths = Directory.GetFiles(sceneBuildDirectory, "*.unity", SearchOption.AllDirectories);
            scenesInBuild = scenePaths.ToList();
#endif
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UniversalSceneManager))]
    public class SceneManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        UniversalSceneManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UniversalSceneManager)target;
            LoadBuildScenes();

            _script.CurrentScene = SceneManager.GetActiveScene().name;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                LoadBuildScenes();
            }


            // Save changes
            if (GUI.changed)
            {
                SceneSelector();
                EditorUtility.SetDirty(_script);
                EditorSceneManager.MarkSceneDirty(_script.gameObject.scene);
            }
        }

        public void SceneSelector()
        {
            // Get all scenes in the build settings
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            // Create an array of scene names to display in the dropdown
            string[] sceneNames = scenes
                .Select(s => System.IO.Path.GetFileNameWithoutExtension(s))
                .ToArray();

            // Current selected index
            int currentIndex = System.Array.IndexOf(scenes, _script.CurrentScene);
            if (currentIndex == -1) currentIndex = 0;

            // Dropdown for selecting scene
            int selectedIndex = EditorGUILayout.Popup("Select Scene", currentIndex, sceneNames);
            _script.CurrentScene = scenes[selectedIndex];
        }

        private void LoadBuildScenes()
        {
            string[] scenePaths = Directory.GetFiles(_script.SceneDirectory, "*.unity", SearchOption.AllDirectories);
            _script.ScenesInBuild = scenePaths.ToList();
        }
    }
#endif
}


