using Darklight.UnityExt.Behaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnHandler : MonoBehaviourSingleton<SpawnHandler>
{
    public static MTR_SceneManager GameSceneManager => MTR_SceneManager.Instance as MTR_SceneManager;

    Scene currentScene;
    Dictionary<string, SceneInteractableInfo> scenes = new Dictionary<string, SceneInteractableInfo>();

    private GameObject Lupe;
    private GameObject Misra;

    public override void Initialize()
    {
        GameSceneManager.OnSceneChanged += SceneChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SceneChanged(Scene oldScene, Scene newScene)
    {
        // Fill data in Dictionary with the spawn points
        currentScene = newScene;

        List<GameObject> interactableObjects = GetAllInteractables();

        foreach (GameObject interactable in interactableObjects)
        {
            // ensure the interactable knows where its DestinationPoints are
            interactable.GetComponent<Interactable>().FindDestinationPoints();
        }

        if (!scenes.ContainsKey(currentScene.name))
        {
            var sceneData = new SceneInteractableInfo(currentScene.name);
            scenes[currentScene.name] = sceneData;
            SetSpawnPoints(interactableObjects);
        }

        // Change locations of Lupe and Misra to Spawn Points
        SceneInteractableInfo sceneInfo = scenes[currentScene.name];

        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        MTR_Misra_Controller tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        if (Lupe != null && Lupe.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count == 0)
            {
                Debug.LogError("Cannot spawn Lupe. No Spawn Points", this);
                return;
            }
            Lupe.transform.position = new Vector3(sceneInfo.spawnPoints[0], Lupe.transform.position.y, Lupe.transform.position.z);
        }

        if (Misra != null && Misra.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count <= 1)
            {
                Debug.LogError("Cannot spawn Misra. No available Spawn Points");
                return;
            }
            Misra.transform.position = new Vector3(sceneInfo.spawnPoints[1], Misra.transform.position.y, Misra.transform.position.z);
        }
    }

    public void SetSpawnPoints(List<GameObject> interactables)
    {
        scenes[currentScene.name].SetSpawnPoints(interactables);
    }

    public List<GameObject> GetAllInteractables()
    {
        Interactable[] sceneInteractables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        List<GameObject> interactableObjects = new List<GameObject>();

        foreach (var interactable in sceneInteractables)
        {
            interactableObjects.Add(interactable.gameObject);
        }

        return interactableObjects;
    }

    public void LogErrorFormat()
    {
        Debug.LogError("\n----------");
        foreach (var data in scenes)
        {
            Debug.LogError(scenes[data.Key].sceneName);
            Debug.LogError(scenes[data.Key].spawnPoints.Count);
            Debug.LogError("\n----------");
        }
    }
}

public class SceneInteractableInfo
{
    public string sceneName;
    public List<float> spawnPoints = new List<float>();

    public SceneInteractableInfo(string sceneName)
    {
        this.sceneName = sceneName;

    }

    public void SetSpawnPoints(List<GameObject> interactables)
    {
        spawnPoints.Clear();

        spawnPoints.AddRange(FindSpawnPoints(interactables));
    }

    private List<float> FindSpawnPoints(List<GameObject> interactables)
    {
        List<float> spawnPoints = new List<float>();

        foreach (var interactable in interactables)
        {
            Interactable script = interactable.GetComponent<Interactable>();

            if (!script.isSpawn) { continue; }

            List<GameObject> destinationPoints = script.GetDestinationPoints();
            foreach (var destinationPoint in destinationPoints)
            {
                spawnPoints.Add(destinationPoint.transform.position.x);
            }
        }
        return spawnPoints;
    }
}