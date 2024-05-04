using System.Collections.Generic;
using System.Linq;

using Darklight.Game.Utility;
using Darklight.UnityExt.CustomEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;

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
    public class SceneManager : MonoBehaviourSingleton<SceneManager>
    {
        [SerializeField, ShowOnly] string sceneBuildDirectory = "Assets/Scenes/Build"; // Updated path to be relative to the Assets folder
        [SerializeField, ShowOnly] private List<SceneAsset> scenesInBuild = new List<SceneAsset>();
        public string SceneDirectory => sceneBuildDirectory;
        public List<SceneAsset> ScenesInBuild { get => scenesInBuild; set => scenesInBuild = value; }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SceneManager))]
    public class SceneManagerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        SceneManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (SceneManager)target;
            LoadBuildScenes();
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



