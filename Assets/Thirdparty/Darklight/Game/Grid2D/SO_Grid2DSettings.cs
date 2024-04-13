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

    public List<SerializableSpawnWeight> serializableSpawnWeights = new List<SerializableSpawnWeight>();
    [ShowOnly] public string uniqueID;
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (uniqueID == "")
        {
            uniqueID = GUID.Generate().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    // File path for JSON storage
    private string FilePath => $"{Application.persistentDataPath}/grid2Dsettings_{uniqueID}.json";

    private void OnEnable() => LoadSpawnWeightMap();
    private void OnDisable() => SaveSpawnWeightMap();

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
        ConvertDictionaryToList();
        string jsonState = JsonUtility.ToJson(this, true);

        try
        {
            File.WriteAllText(FilePath, jsonState);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save settings for {uniqueID}: {ex.Message}");
        }
    }

    public void LoadSpawnWeightMap()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                string jsonState = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(jsonState, this);
                ConvertListToDictionary();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load settings for {uniqueID}: {ex.Message}");
            }
        }
    }

    private void ConvertDictionaryToList()
    {
        serializableSpawnWeights.Clear();
        foreach (KeyValuePair<Vector2Int, (bool, int)> item in spawnWeightMap)
        {
            serializableSpawnWeights.Add(new SerializableSpawnWeight { positionKey = item.Key, active = item.Value.Item1, weight = item.Value.Item2 });
        }
    }

    private void ConvertListToDictionary()
    {
        spawnWeightMap.Clear();
        foreach (var item in serializableSpawnWeights)
        {
            spawnWeightMap[item.positionKey] = (item.active, item.weight);
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