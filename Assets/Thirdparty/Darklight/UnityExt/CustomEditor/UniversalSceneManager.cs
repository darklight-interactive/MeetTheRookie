using System.Collections.Generic;
using System.Linq;

using Darklight.Game.Utility;
using Darklight.UnityExt.CustomEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;



#if UNITY_EDITOR
using UnityEditor;
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
        [SerializeField, ShowOnly] string sceneBuildDirectory = "Assets/Scenes/Build"; // Updated path to be relative to the Assets folder
        [SerializeField, ShowOnly] string currentScene;
        [SerializeField, ShowOnly] private List<SceneAsset> scenesInBuild = new List<SceneAsset>();
        public string SceneDirectory => sceneBuildDirectory;
        public List<SceneAsset> ScenesInBuild { get => scenesInBuild; set => scenesInBuild = value; }
        public void SetSceneAsCurrent(string name)
        {
            SceneAsset scene = scenesInBuild.Find(s => s.name == name);
            if (scene != null)
            {
                currentScene = scene.name;
            }
            else
            {
                currentScene = "ERROR : Current Scene not in Build Settings";
            }
        }

        public bool IsSceneInBuild(SceneAsset scene)
        {
            return scenesInBuild.Contains(scene);
        }

        public bool GoToScene(string sceneName)
        {
            if (scenesInBuild.Count == 0)
            {
                Debug.LogError("No scenes in build settings");
                return false;
            }

            SceneAsset scene = scenesInBuild.Find(s => s.name == sceneName);
            if (scene == null)
            {
                Debug.LogError($"Scene {sceneName} not found in build settings");
                return false;
            }

            //Addressables.LoadScene(sceneName);
            SceneManager.LoadScene(sceneName);
            currentScene = sceneName;
            return true;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UniversalSceneManager))]
    public class SceneManagerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        UniversalSceneManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UniversalSceneManager)target;
            LoadBuildScenes();

            _script.SetSceneAsCurrent(EditorSceneManager.GetActiveScene().name);
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
        }

        private void LoadBuildScenes()
        {
            string[] scenePaths = Directory.GetFiles(_script.SceneDirectory, "*.unity", SearchOption.AllDirectories);
            List<SceneAsset> scenes = new List<SceneAsset>();
            foreach (string scenePath in scenePaths)
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                scenes.Add(scene);
            }
            _script.ScenesInBuild = scenes;
        }
    }
#endif
}


