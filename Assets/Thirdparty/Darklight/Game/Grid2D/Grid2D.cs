using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Darklight.Game.Grid2D
{


    /// <summary>
    /// A 2D Grid that can be used to store any type of data.
    /// </summary>
    /// <typeparam name="IGrid2DData">The type of data to be stored in the grid.</typeparam>
    public class Grid2D<T> where T : IGrid2DData
    {
        public SO_Grid2DSettings settings;
        #region [[ Private Properties ]] =============================== >>
        private int gridSizeX => settings.gridSizeX;
        private int gridSizeY => settings.gridSizeY;
        private float coordinateSize => settings.coordinateSize;
        private int originKeyX => settings.originKeyX;
        private int originKeyY => settings.originKeyY;
        private Dictionary<Vector2Int, IGrid2DData> dataMap = new Dictionary<Vector2Int, IGrid2DData>();
        private Vector2Int gridXAxis => new Vector2Int(1, 0); // create x Axis Vector
        private Vector2Int gridYAxis => new Vector2Int(0, 1); // create y Axis Vector
        private Vector2Int gridArea => new Vector2Int(gridSizeX, gridSizeY);
        private Transform gridParent = null;
        private Vector2Int gridParentPositionKey => new Vector2Int(originKeyX, originKeyY);
        #endregion

        public Grid2D(Transform parent, SO_Grid2DSettings settings)
        {
            this.gridParent = parent;
            this.settings = settings;
            Initialize();
        }

        #region [[ Public Methods ]] =============================== >>
        /// <summary>
        /// Initializes the grid by creating coordinates for each position in the grid.
        /// </summary>
        public void Initialize()
        {
            if (settings == null)
            {
                Debug.LogError("Grid2D settings is null. Please assign a Grid2DSettings asset to the settings property of this Grid2D.",
                    this.gridParent.gameObject);
                return;
            }

            // Create the grid
            dataMap = new Dictionary<Vector2Int, IGrid2DData>();
            for (int x = 0; x < gridArea.x; x++)
            {
                for (int y = 0; y < gridArea.y; y++)
                {
                    Vector2Int position = gridXAxis * x + gridYAxis * y;
                    dataMap.Add(position, default);
                }
            }
        }

        /// <summary>
        /// Sets the value of a coordinate in the grid.
        /// </summary>
        /// <param name="position">The position of the coordinate.</param>
        /// <param name="data">The value to set.</param>
        public void SetData(Vector2Int position, IGrid2DData data)
        {
            // if the position is in the grid, set the data
            if (dataMap.ContainsKey(position))
            {
                dataMap[position] = data;
                return;
            }
            Debug.LogError("The position " + position + " is not in the grid.", this.gridParent.gameObject);
        }


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
        public IGrid2DData GetData(Vector2Int position)
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

        public List<IGrid2DData> GetAllData()
        {
            List<IGrid2DData> values = new List<IGrid2DData>();
            if (dataMap != null && dataMap.Count > 0)
            {
                foreach (IGrid2DData data in dataMap.Values)
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
