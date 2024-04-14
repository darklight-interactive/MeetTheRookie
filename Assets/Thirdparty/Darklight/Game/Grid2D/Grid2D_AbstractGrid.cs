using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{

    /// <summary>
    /// An abstract 2D Grid class to be used as a base for different types of grids storing data inheriting from Grid2DData.
    /// </summary>
    public abstract class Grid2D_AbstractGrid<Data> : MonoBehaviour where Data : IGrid2D_Data, new()
    {
        [SerializeField] protected Grid2D_Preset preset; // The settings for the grid
        protected Dictionary<Vector2Int, Data> DataMap { get; private set; } = new Dictionary<Vector2Int, Data>();
        protected Vector2Int GridArea => new Vector2Int(preset.gridSizeX, preset.gridSizeY);
        protected Vector2Int OriginKey => new Vector2Int(preset.originKeyX, preset.originKeyY);

        public virtual void Awake()
        {
            InitializeDataMap();
        }

        /// <summary>
        /// Initializes the data map with default grid data values
        /// </summary>
        protected abstract void InitializeDataMap();

        /// <summary>
        /// Calculates the world space position of the specified position key in the grid.
        /// </summary>
        /// <param name="positionKey">The position key in the grid.</param>
        /// <returns>The world space position of the specified position key.</returns>
        public Vector3 GetWorldSpacePosition(Vector2Int positionKey)
        {

            // Calculate the world space position
            Vector2Int offsetPosition = positionKey - OriginKey;
            Vector3 vec3_position = new Vector3(offsetPosition.x, offsetPosition.y, 0);
            vec3_position *= preset.coordinateSize;

            // Transform the position to world space using this transform as the parent
            Vector3 worldSpacePosition = transform.TransformVector(transform.position + vec3_position);
            return worldSpacePosition;
        }

        /// <summary>
        /// Sets the data at a given position in the grid.
        /// </summary>
        /// <param name="position">The grid position.</param>
        /// <param name="data">The data to set at the position.</param>
        public virtual void SetData(Vector2Int position, Data data)
        {
            if (DataMap.ContainsKey(position))
            {
                DataMap[position] = data;
                OnDataChanged(position, data); // Notify change
                return;
            }
            Debug.LogError($"The position {position} is not in the grid.", this);
        }

        protected abstract void OnDataChanged(Vector2Int position, Data data);

        /// <summary>
        /// Retrieves the data at a given position in the grid.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual Data GetData(Vector2Int position)
        {
            DataMap.TryGetValue(position, out Data data);
            return data;
        }

        public IEnumerable<Vector2Int> GetPositionKeys()
        {
            return DataMap.Keys;
        }
    }


}
