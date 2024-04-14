using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Darklight.Game.Grid2D
{
    /// <summary>
    /// A 2D Grid that can be used to store data classes that that inherit from Grid2DData.
    /// </summary>
    public class Grid2D : MonoBehaviour
    {
        #region [[ Private Properties ]] =============================== >>
        private int gridSizeX => preset.gridSizeX;
        private int gridSizeY => preset.gridSizeY;
        private float coordinateSize => preset.coordinateSize;
        private int originKeyX => preset.originKeyX;
        private int originKeyY => preset.originKeyY;
        private Vector2Int gridXAxis => new Vector2Int(1, 0); // create x Axis Vector
        private Vector2Int gridYAxis => new Vector2Int(0, 1); // create y Axis Vector
        private Vector2Int gridArea => new Vector2Int(gridSizeX, gridSizeY);
        private Transform gridParent => this.transform;
        private Vector2Int gridParentPositionKey => new Vector2Int(originKeyX, originKeyY);
        #endregion

        public Grid2DPreset preset; // The settings for the grid
        public Dictionary<Vector2Int, Grid2DData> data2DMap { get; private set; } // The data map for the grid

        #region [[ Public Methods ]] =============================== >>
        /// <summary>
        /// Initializes the grid by creating coordinates for each position in the grid.
        /// </summary>
        public void Initialize()
        {
            if (preset == null)
            {
                Debug.LogError("Grid2D settings is null. Please assign a Grid2DSettings asset to the settings property of this Grid2D.",
                    this.gridParent.gameObject);
                return;
            }

            // Create the grid
            data2DMap = new Dictionary<Vector2Int, Grid2DData>();
            for (int x = 0; x < gridArea.x; x++)
            {
                for (int y = 0; y < gridArea.y; y++)
                {
                    Vector2Int position = gridXAxis * x + gridYAxis * y;
                    data2DMap.Add(position, default);
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
            if (data2DMap.ContainsKey(position))
            {
                data2DMap[position] = data;
                return;
            }
            Debug.LogError("The position " + position + " is not in the grid.", this.gridParent.gameObject);
        }

        /// <summary>
        /// Removes the dataValue of a coordinate in the grid.
        /// </summary>
        /// <param name="position"></param>
        public void RemoveData(Vector2Int position)
        {
            if (data2DMap.ContainsKey(position))
            {
                data2DMap.Remove(position);
            }
        }

        /// <summary>
        /// Gets the dataValue of a coordinate in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Grid2DData GetData(Vector2Int position)
        {
            if (data2DMap.ContainsKey(position))
            {
                return data2DMap[position];
            }
            return default;
        }

        /// <summary>
        /// Gets a list of all position keys in the grid.
        /// </summary>
        /// <returns>A list of all position keys in the grid.</returns>
        public List<Vector2Int> GetPositionKeys()
        {
            if (data2DMap != null)
                return new List<Vector2Int>(data2DMap.Keys);
            return new List<Vector2Int>();
        }

        /// <summary>
        /// Gets a list of all data values in the grid.
        /// </summary>
        /// <returns></returns>
        public List<Grid2DData> GetAllData()
        {
            List<Grid2DData> values = new List<Grid2DData>();
            if (data2DMap != null && data2DMap.Count > 0)
            {
                foreach (Grid2DData data in data2DMap.Values)
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
            if (this.gridParent == null) { return Vector3.zero; }

            Vector2Int offsetPosition = positionKey - gridParentPositionKey;
            Vector3 vec3_position = new Vector3(offsetPosition.x, offsetPosition.y, 0);
            vec3_position *= coordinateSize;

            Vector3 worldSpacePosition = gridParent.TransformVector(gridParent.position + vec3_position);
            return worldSpacePosition;
        }
        #endregion
    }
}
