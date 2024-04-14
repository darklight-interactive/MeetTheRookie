using System;
using System.Collections.Generic;
using Darklight.Game.Grid;
using Darklight.UnityExt;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2DPreset")]
/// <summary>
/// A scriptable object that contains the preset settings for a Grid2D instance.
/// </summary>
public class Grid2DPreset : ScriptableObject
{

    #region << Create Unique ID >> ------------------------------------ >>
    [CustomInspectorGUI.ShowOnly] public string uniqueID;
    private void OnValidate()
    {

#if UNITY_EDITOR
        if (uniqueID == "")
        {
            uniqueID = GUID.Generate().ToString();
        }
        EditorUtility.SetDirty(this);
#endif
    }
    #endregion

    #region << Grid Layout Settings >> -------------------------------- >>
    const int MIN = 1;
    const int MAX = 10;

    [Header("Grid Layout Settings")]
    [Range(MIN, MAX)] public int gridSizeX = 3;
    [Range(MIN, MAX)] public int gridSizeY = 3;
    [Range(0.1f, 1f)] public float coordinateSize = 1;
    [Range(-MAX, MAX)] public int originKeyX = 1;
    [Range(-MAX, MAX)] public int originKeyY = 1;
    #endregion

    // For serialization
    [SerializeField] private List<Vector2Int> keys = new List<Vector2Int>();
    [SerializeField] private List<Serialized_Grid2DData> values = new List<Serialized_Grid2DData>();

    // Not serialized, rebuilt on load
    private Dictionary<Vector2Int, Serialized_Grid2DData> dataMap = new Dictionary<Vector2Int, Serialized_Grid2DData>();

    private void OnEnable()
    {
        // Rebuild the dictionary from the serialized lists
        dataMap.Clear();
        for (int i = 0; i < keys.Count && i < values.Count; i++)
        {
            dataMap[keys[i]] = values[i];
        }
    }

    /// <summary>
    /// Save the data to the dictionary and serialized lists
    /// </summary>
    public void SaveData(Grid2DData data)
    {
        Serialized_Grid2DData serializedData = new Serialized_Grid2DData();
        serializedData.Initialize(data.positionKey, data.disabled, data.weight, data.worldPosition, data.coordinateSize);

        if (dataMap.ContainsKey(serializedData.positionKey))
        {
            dataMap[serializedData.positionKey] = serializedData;
            int index = keys.IndexOf(serializedData.positionKey);
            values[index] = serializedData;
        }
        else
        {
            dataMap.Add(serializedData.positionKey, serializedData);
            keys.Add(serializedData.positionKey);
            values.Add(serializedData);
        }
        MarkAsDirty();
    }

    public Serialized_Grid2DData GetData(Vector2Int position)
    {
        if (dataMap.ContainsKey(position))
        {
            return dataMap[position];
        }
        return null;
    }

    private void MarkAsDirty()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}