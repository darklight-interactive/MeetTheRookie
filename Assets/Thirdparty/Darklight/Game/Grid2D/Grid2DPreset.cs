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

[CreateAssetMenu(menuName = "Darklight/Grid2DPreset")]
/// <summary>
/// A scriptable object that contains the preset settings for a Grid2D instance.
/// </summary>
public class Grid2DPreset : ScriptableObject
{

    #region << Create Unique ID >> ------------------------------------ >>
    [ShowOnly] public string uniqueID;
    private void OnValidate()
    {
        EditorUtility.SetDirty(this);

#if UNITY_EDITOR
        if (uniqueID == "")
        {
            uniqueID = GUID.Generate().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
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

    [Serializable]
    public class S_CoordinatePresetData
    {
        public Vector2Int positionKey;
        public bool disabled;
        public int weight;
        public S_CoordinatePresetData(Vector2Int vector2Int, bool active, int weight)
        {
            this.positionKey = vector2Int;
            this.disabled = active;
            this.weight = weight;
        }

        public S_CoordinatePresetData(Grid2DData grid2DData)
        {
            this.positionKey = grid2DData.positionKey;
            this.disabled = grid2DData.disabled;
            this.weight = grid2DData.weight;
        }
    }

    // For serialization
    [SerializeField] private List<Vector2Int> keys = new List<Vector2Int>();
    [SerializeField] private List<S_CoordinatePresetData> values = new List<S_CoordinatePresetData>();

    // Not serialized, rebuilt on load
    private Dictionary<Vector2Int, S_CoordinatePresetData> dataMap = new Dictionary<Vector2Int, S_CoordinatePresetData>();

    private void OnEnable()
    {
        // Rebuild the dictionary from the serialized lists
        dataMap.Clear();
        for (int i = 0; i < keys.Count && i < values.Count; i++)
        {
            dataMap[keys[i]] = values[i];
        }
    }

    public void SaveData(S_CoordinatePresetData data)
    {
        if (dataMap.ContainsKey(data.positionKey))
        {
            dataMap[data.positionKey] = data;
            int index = keys.IndexOf(data.positionKey);
            values[index] = data;
        }
        else
        {
            dataMap.Add(data.positionKey, data);
            keys.Add(data.positionKey);
            values.Add(data);
        }
        MarkAsDirty();
    }

    public void SaveData(Grid2DData data)
    {
        SaveData(new S_CoordinatePresetData(data));
    }

    private void MarkAsDirty()
    {
        EditorUtility.SetDirty(this);
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
    }

    /*
        public void DisplayGrid2DSettings()
        {
            foreach (Vector2Int vector2Int in dataMap.Keys)
            {
                IGrid2DData data = dataMap[vector2Int];
                bool active = data.active;
                Vector3 worldPosition = data.worldPosition;
                float size = data.coordinateSize;
                Vector3 direction = Vector3.forward;
                Color color = data.debugColor;

                CustomGizmos.DrawLabel($"{active}", worldPosition, CustomGUIStyles.CenteredStyle);
                CustomGizmos.DrawWireSquare(worldPosition, size, direction, color);
                CustomGizmos.DrawButtonHandle(data.worldPosition, size * 0.75f, direction, data.debugColor, () =>
                {
                    data.CycleDataState();
                    EditorUtility.SetDirty(this);

                }, Handles.RectangleHandleCap);
            }
        }
    */


}

#if UNITY_EDITOR
[CustomEditor(typeof(Grid2DPreset))]
public class Grid2DPresetEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Grid2DPreset gridPreset = (Grid2DPreset)target;
    }

    private void OnSceneGUI()
    {
        Grid2DPreset gridPreset = (Grid2DPreset)target;

        // Ensures the inspector updates and reflects changes when manipulating handles
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }


}
#endif