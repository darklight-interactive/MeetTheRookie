using System;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using Darklight.UnityExt;
using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2DSettings")]
[CanEditMultipleObjects]
public class SO_Grid2DSettings : ScriptableObject
{
    const int MIN = 1;
    const int MAX = 10;
    [Range(MIN, MAX)] public int gridSizeX = 3;
    [Range(MIN, MAX)] public int gridSizeY = 3;
    [Range(0.1f, 1f)] public float coordinateSize = 1;
    [Range(-MAX, MAX)] public int originKeyX = 1;
    [Range(-MAX, MAX)] public int originKeyY = 1;
    public Dictionary<Vector2Int, (bool, int)> spawnWeightMap = new Dictionary<Vector2Int, (bool, int)>();

    [ShowOnly] public bool loaded = false;
    [ShowOnly] public int spawnWeightMapCount = 0;

    [Serializable]
    public class SerializableSpawnWeight
    {
        public Vector2Int positionKey;
        public bool active;
        public int weight;
    }

    const string saveKey = "Grid2DSettings";
    public List<SerializableSpawnWeight> serializableSpawnWeights = new List<SerializableSpawnWeight>();

    // File path for JSON storage
    private string FilePath => $"{Application.persistentDataPath}/grid2DSettings.json";

    public void OnEnable()
    {
        LoadSpawnWeightMap();
    }

    public void OnDisable()
    {
        SaveSpawnWeightMap();
    }

    public void SetSpawnWeight(Vector2Int positionKey, bool active, int weight)
    {
        if (spawnWeightMap.ContainsKey(positionKey))
        {
            spawnWeightMap[positionKey] = (active, weight);
        }
        else
        {
            spawnWeightMap.Add(positionKey, (active, weight));
        }
    }

    public void SaveSpawnWeightMap()
    {
        // Convert dictionary to serializable list
        serializableSpawnWeights.Clear();
        foreach (KeyValuePair<Vector2Int, (bool, int)> item in spawnWeightMap)
        {
            serializableSpawnWeights.Add(new SerializableSpawnWeight { positionKey = item.Key, active = item.Value.Item1, weight = item.Value.Item2 });
        }

        // Save to PlayerPrefs
        string jsonState = JsonUtility.ToJson(this, true);
        PlayerPrefs.SetString(saveKey, jsonState);
        PlayerPrefs.Save();

        // Save to JSON file
        File.WriteAllText(FilePath, jsonState);
    }

    public void LoadSpawnWeightMap()
    {
        // Load from PlayerPrefs
        if (PlayerPrefs.HasKey(saveKey))
        {
            string jsonState = PlayerPrefs.GetString(saveKey);
            JsonUtility.FromJsonOverwrite(jsonState, this);

            // Convert list back to dictionary
            spawnWeightMap.Clear();
            foreach (SerializableSpawnWeight item in serializableSpawnWeights)
            {
                spawnWeightMap[item.positionKey] = (item.active, item.weight);
            }
        }
        else // Optional: Load from JSON file if PlayerPrefs does not exist
        {
            if (File.Exists(FilePath))
            {
                string jsonState = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(jsonState, this);

                // Convert list back to dictionary
                spawnWeightMap.Clear();
                foreach (SerializableSpawnWeight item in serializableSpawnWeights)
                {
                    spawnWeightMap[item.positionKey] = (item.active, item.weight);
                }
            }
        }
    }
    public static void DisplayGrid2D(Grid2D<IGrid2DData> grid2D)
    {
        if (grid2D == null)
        {
            Debug.LogError("Grid2D is null");
            return;
        }

        List<Vector2Int> positionKeys = grid2D.GetPositionKeys();
        if (positionKeys != null && positionKeys.Count > 0)
        {
            foreach (Vector2Int vector2Int in positionKeys)
            {
                IGrid2DData data = grid2D.GetData(vector2Int);
                if (data == null) continue;

                //float weightValue = overlapData.weight;
                float size = grid2D.settings.coordinateSize;
                Vector3 direction = Vector3.forward;

                //CustomGizmos.DrawLabel(data.label, data.worldPosition, CustomGUIStyles.BoldStyle);
                CustomGizmos.DrawWireSquare(data.worldPosition, size, direction, data.debugColor);
                CustomGizmos.DrawButtonHandle(data.worldPosition, size * 0.75f, direction, data.debugColor, () =>
                {
                    data.CycleDataState();
                    EditorUtility.SetDirty(grid2D.settings);
                }, Handles.RectangleHandleCap);
            }
        }
    }

}