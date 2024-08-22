using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnHandler : MonoBehaviour
{
    Scene currentScene;
    Dictionary<string, SceneInteractableInfo> scenes = new Dictionary<string, SceneInteractableInfo>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SceneChanged()
    {
        currentScene = SceneManager.GetActiveScene();

        Interactable[] sceneInteractables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (var interactable in sceneInteractables)
        {
            gameObjects.Add(interactable.gameObject);
        }

        if (!scenes.ContainsKey(currentScene.name))
        {
            var sceneData = new SceneInteractableInfo(currentScene.name, gameObjects);
            scenes[currentScene.name] = sceneData;
        }


        // NEED TO FINISH AND CALL WHEN SCENE CHANGES
    }

    public void UpdateSpawnPoints()
    {
        scenes[currentScene.name].FindSpawnPoints();
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