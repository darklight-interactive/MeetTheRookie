using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;
using UnityEngine.XR;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid2D
{
    /// <summary>
    /// A 2D Grid that can be used to store data classes that that inherit from Grid2DData.
    /// </summary>
    public class Grid2D : MonoBehaviour
    {
        public Grid2DPreset preset; // The settings for the grid

        #region [[ Private Properties ]] =============================== >>
        private Dictionary<Vector2Int, Grid2DData> dataMap = new(); // The data map for the grid
        private int gridSizeX => preset.gridSizeX;
        private int gridSizeY => preset.gridSizeY;
        private float coordinateSize => preset.coordinateSize;
        private Vector2Int gridXAxis => new Vector2Int(1, 0); // create x Axis Vector
        private Vector2Int gridYAxis => new Vector2Int(0, 1); // create y Axis Vector
        private Vector2Int gridArea => new Vector2Int(gridSizeX, gridSizeY); // used for grid creation
        private Vector2Int originKey => new Vector2Int(preset.originKeyX, preset.originKeyY); // used to set the origin coordinate of the grid
        #endregion

        public void Awake()
        {
            InitializeDataMap();
        }

        public void Update()
        {
            foreach (Grid2DData data in GetAllData())
            {
                data.UpdateData(this);
            }
        }

        #region [[ Public Methods ]] =============================== >>

        public void InitializeDataMap()
        {
            if (preset == null)
            {
                Debug.LogError("The Grid2D preset is not set.", this);
                return;
            }

            // Create the grid
            dataMap = new Dictionary<Vector2Int, Grid2DData>();
            for (int x = 0; x < gridArea.x; x++)
            {
                for (int y = 0; y < gridArea.y; y++)
                {
                    Vector2Int positionKey = gridXAxis * x + gridYAxis * y;

                    Grid2DData dataObject = preset.CreateNewData(this, positionKey);

                    // Subscribe to the data state change event
                    dataObject.OnDataStateChanged += (Grid2DData data) =>
                    {
                        SetData(positionKey, data);
                    };

                    dataObject.UpdateData(this);

                    // Add the data object to the map
                    dataMap[positionKey] = dataObject;
                }
            }
        }

        /// <summary>
        /// Sets the value of a coordinate in the grid.
        /// </summary>
        /// <param name="position">The position of the coordinate.</param>
        /// <param name="data">The value to set.</param>
        public void SetData(Vector2Int position, Grid2DData data)
        {
            // if the position is in the grid, set the data
            if (dataMap.ContainsKey(position))
            {
                //Debug.Log("Setting data at position " + position + $" with weight: {data.weight}");
                dataMap[position] = data;
                preset.SaveData(data);
                return;
            }
            Debug.LogError("The position " + position + " is not in the grid.", this);
        }

        /// <summary>
        /// Removes the dataValue of a coordinate in the grid.
        /// </summary>
        /// <param name="position"></param>
        public void RemoveData(Vector2Int position)
        {
            if (dataMap.ContainsKey(position))
            {
                dataMap.Remove(position);
            }
        }

        /// <summary>
        /// Gets the dataValue of a coordinate in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Grid2DData GetData(Vector2Int position)
        {
            if (dataMap.ContainsKey(position))
            {
                return dataMap[position];
            }
            return default;
        }

        /// <summary>
        /// Gets a list of all position keys in the grid.
        /// </summary>
        /// <returns>A list of all position keys in the grid.</returns>
        public List<Vector2Int> GetPositionKeys()
        {
            if (dataMap != null)
                return new List<Vector2Int>(dataMap.Keys);
            return new List<Vector2Int>();
        }

        /// <summary>
        /// Gets a list of all data values in the grid.
        /// </summary>
        /// <returns></returns>
        public List<Grid2DData> GetAllData()
        {
            List<Grid2DData> values = new List<Grid2DData>();
            if (dataMap != null && dataMap.Count > 0)
            {
                foreach (Grid2DData data in dataMap.Values)
                {
                    values.Add(data);
                }
            }
            return values;
        }

        /// <summary>
        /// Gets the world space position of the specified position key in the grid.
        /// </summary>
        /// <param name="positionKey">The position key in the grid.</param>
        /// <returns>The world space position of the specified position key.</returns>
        public Vector3 GetWorldSpacePosition(Vector2Int positionKey)
        {
            // Calculate the local position key offset from the origin in world space
            Vector2Int offsetPosition = positionKey - originKey;

            // Calculate the offset position in world space multiplied by the coordinate size
            Vector3 vec3_offset_position = new Vector3(offsetPosition.x, offsetPosition.y, 0);
            vec3_offset_position *= coordinateSize;

            // Calculate the world space position
            Vector3 worldSpacePosition = transform.TransformVector(transform.position + vec3_offset_position);
            return worldSpacePosition;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D))]
    public class Grid2DEditor : Editor
    {
        Grid2D grid2D;
        private void OnEnable()
        {
            grid2D = (Grid2D)target;
            grid2D.Awake();
        }

        private void OnSceneGUI()
        {
            if (grid2D == null) return;
            DrawGrid();
        }

        public void DrawGrid()
        {
            if (grid2D == null) return;

            foreach (Vector2Int positionKey in grid2D.GetPositionKeys())
            {
                Grid2DData data = grid2D.GetData(positionKey);
                Vector3 worldPosition = data.worldPosition;
                float size = data.coordinateSize;



                CustomGizmos.DrawWireSquare(worldPosition, size, Vector3.forward, data.GetColor());
                CustomGizmos.DrawLabel($"{positionKey}", worldPosition, CustomGUIStyles.CenteredStyle);
                CustomGizmos.DrawButtonHandle(worldPosition, size * 0.75f, Vector3.forward, data.GetColor(), () =>
                {
                    data.CycleDataState();
                }, Handles.RectangleHandleCap);
            }
        }
    }
#endif
}
