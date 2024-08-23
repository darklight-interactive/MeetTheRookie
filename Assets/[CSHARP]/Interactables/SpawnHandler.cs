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
            var sceneData = new SceneInteractableInfo(currentScene.name, interactableObjects);
            scenes[currentScene.name] = sceneData;
        }


        UpdateSpawnPoints();

        // Change locations of Lupe and Misra to Spawn Points
        SceneInteractableInfo sceneInfo = scenes[currentScene.name];

        var tempLupe = FindFirstObjectByType<PlayerController>();
        if (tempLupe != null)
        {
            Lupe = tempLupe.gameObject;
        }

        var tempMisra = FindFirstObjectByType<MTR_Misra_Controller>();
        if (tempMisra != null)
        {
            Misra = tempMisra.gameObject;
        }

        if (Lupe != null && Lupe.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count == 0)
            {
                Debug.LogError("Cannot spawn Lupe. No Spawn Points");
                return;
            }
            Lupe.transform.position = new Vector3(sceneInfo.spawnPoints[0].transform.position.x, Lupe.transform.position.y, Lupe.transform.position.z);
        }

        if (Misra != null && Misra.GetComponent<SpriteRenderer>().enabled)
        {
            if (sceneInfo.spawnPoints.Count <= 1)
            {
                Debug.LogError("Cannot spawn Misra. No available Spawn Points");
                return;
            }
            Misra.transform.position = new Vector3(sceneInfo.spawnPoints[1].transform.position.x, Misra.transform.position.y, Misra.transform.position.z);
        }
    }

    public void UpdateSpawnPoints()
    {
        scenes[currentScene.name].FindSpawnPoints();
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
}

public class SceneInteractableInfo
{
    public string sceneName;
    public List<GameObject> interactables;
    public List<GameObject> spawnPoints = new List<GameObject>();

    public SceneInteractableInfo(string sceneName, List<GameObject> interactables)
    {
        this.sceneName = sceneName;
        this.interactables = interactables;

    }

    public void FindSpawnPoints()
    {
        if (interactables.Count == 0)
        {
            return;
        }

        spawnPoints.Clear();

        foreach (var interactableObject in interactables)
        {
            Interactable interactable = interactableObject.GetComponent<Interactable>();
            if (!interactable.isSpawn) { continue; }

            spawnPoints.AddRange(interactable.GetDestinationPoints());
        }
    }
}