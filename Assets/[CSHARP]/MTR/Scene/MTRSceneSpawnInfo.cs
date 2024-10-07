using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;

[Serializable]
public class MTRSceneSpawnInfo
{
    [SerializeField, ShowOnly] string _sceneName;
    [SerializeField, NonReorderable] List<float> _spawnPoints = new List<float>();

    public List<float> SpawnPoints { get => _spawnPoints; set => _spawnPoints = value; }

    public MTRSceneSpawnInfo(string sceneName)
    {
        this._sceneName = sceneName;
    }

    public void DrawInScene()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            DrawSpawnPoint(i, _spawnPoints[i]);
        }
    }

    List<float> FindSpawnPoints(List<GameObject> interactables)
    {
        List<float> spawnPoints = new List<float>();

        foreach (var interactable in interactables)
        {
            BaseInteractable script = interactable.GetComponent<BaseInteractable>();

            /*
            if (!script.isSpawn) { continue; }

            List<GameObject> destinationPoints = script.GetDestinationPoints();
            foreach (var destinationPoint in destinationPoints)
            {
                spawnPoints.Add(destinationPoint.transform.position.x);
            }
            */
        }
        return spawnPoints;
    }

    void DrawSpawnPoint(int index, float x_value)
    {
        // Draw a line at the spawn point
        Vector3 start = new Vector3(x_value, 0, 0);
        Vector3 end = new Vector3(x_value, 10, 0);


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(start, 0.05f);
        CustomGizmos.DrawLabel($"SpawnPoint{index}", start + (Vector3.up * 0.1f), new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.yellow
            }
        });
    }
}