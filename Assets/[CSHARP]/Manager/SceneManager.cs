using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    [SerializeField]
    private string sceneDirectory = "Assets/Scenes";
    [SerializeField, ShowOnly] private List<SceneAsset> scenesInBuild = new List<SceneAsset>();
    public void SetScenes(List<SceneAsset> scenes)
    {
        this.scenesInBuild = scenes;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneManager))]
public class SceneManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    SceneManager _script;
    List<SceneAsset> m_SceneAssets = new List<SceneAsset>();
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (SceneManager)target;

        m_SceneAssets = LoadScenesFromBuildSettings();
        _script.SetScenes(m_SceneAssets);
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }

    List<SceneAsset> LoadScenesFromBuildSettings()
    {
        m_SceneAssets.Clear();
        //Debug.Log($"Loading {EditorBuildSettings.scenes.Count()} scenes from build settings...");

        List<SceneAsset> sceneAssets = new List<SceneAsset>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string path = scene.path;
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (sceneAsset != null)
                    sceneAssets.Add(sceneAsset);
            }
        }
        return sceneAssets;
    }
}
#endif
