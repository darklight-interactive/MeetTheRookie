
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darklight.UnityExt.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildSceneDataManager<TSceneData> : BuildSceneManager where TSceneData : BuildSceneData, new()
{
    protected TSceneData[] buildSceneData = new TSceneData[0];

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {

#if UNITY_EDITOR
        LoadBuildScenesFromDirectory();
#endif
    }

    /// <summary>
    /// Subscribes to SceneManager events.
    /// </summary>
    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    /// <summary>
    /// Unsubscribes from SceneManager events.
    /// </summary>
    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    /// <summary>
    /// Retrieves the scene data for a given scene name.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    /// <returns>The scene data for the specified scene name.</returns>
    public TSceneData GetSceneData(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"{Prefix} Cannot get scene data for null or empty scene name.");
            return null;
        }

        return buildSceneData.FirstOrDefault(scene => scene.Name == sceneName);
    }

    /// <summary>
    /// Retrieves the scene data for a given scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns>The scene data for the specified scene.</returns>
    public TSceneData GetSceneData(Scene scene)
    {
        return GetSceneData(scene.name);
    }

    /// <summary>
    /// Retrieves the data for the active scene.
    /// </summary>
    public TSceneData GetActiveSceneData()
    {
        Scene scene = SceneManager.GetActiveScene();
        return GetSceneData(scene.name);
    }

#if UNITY_EDITOR
    public override void LoadBuildScenesFromDirectory()
    {
        base.LoadBuildScenesFromDirectory();

        List<Scene> buildScenes = this.buildScenes.ToList();
        List<TSceneData> sceneData = new List<TSceneData>();
        foreach (Scene scene in buildScenes)
        {
            TSceneData data = new TSceneData
            {
                UnityScene = scene
            };
            sceneData.Add(data);
        }
    }

#endif
}