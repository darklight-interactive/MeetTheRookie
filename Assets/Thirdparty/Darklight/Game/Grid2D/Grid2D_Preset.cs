using System;
using System.Collections.Generic;
using Darklight.UnityExt.CustomEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Darklight/Grid2DPreset")]
/// <summary>
/// A scriptable object that contains the preset settings for a Grid2D instance.
/// </summary>
public class Grid2D_Preset : ScriptableObject
{

    #region << Create Unique ID >> ------------------------------------ >>
    [ShowOnly] public string uniqueID;
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

    [Header("Serialized Data Values")]
    [SerializeField] private List<Vector2Int> keys = new List<Vector2Int>();
    [SerializeField] private List<Grid2D_SerializedData> values = new List<Grid2D_SerializedData>();

    // Not serialized, rebuilt on load
    private Dictionary<Vector2Int, Grid2D_SerializedData> dataMap = new Dictionary<Vector2Int, Grid2D_SerializedData>();

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
    public void SaveData(Grid2D_Data data)
    {
        Vector2Int positionKey = data.positionKey;
        Grid2D_SerializedData serializedData = new Grid2D_SerializedData(data);

        if (dataMap.ContainsKey(positionKey))
        {
            dataMap[positionKey] = serializedData;
            int index = keys.IndexOf(positionKey);
            values[index] = serializedData;
        }
        else
        {
            dataMap.Add(positionKey, serializedData);
            keys.Add(positionKey);
            values.Add(serializedData);
        }
        MarkAsDirty();
    }

    public Grid2D_SerializedData LoadData(Vector2Int positionKey)
    {
        if (dataMap.ContainsKey(positionKey))
        {
            return dataMap[positionKey];
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

/// <summary>
/// Serialized version of the Grid2DData class. This class is used to store the data in a serialized format.
/// </summary>
[System.Serializable]
public class Grid2D_SerializedData
{
    [SerializeField] private Vector2Int _positionKey;
    [SerializeField] private bool _disabled;
    [SerializeField] private int _weight;

    public Grid2D_SerializedData(Grid2D_Data input_data)
    {
        _positionKey = input_data.positionKey;
        _disabled = input_data.disabled;
        _weight = input_data.weight;
    }

    public Grid2D_Data ToData()
    {
        return new Grid2D_Data(_positionKey, _disabled, _weight, Vector3.zero, 1);
    }
}