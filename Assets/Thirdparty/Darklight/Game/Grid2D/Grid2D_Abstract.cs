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
    public abstract class Grid2D_Abstract<Data> : MonoBehaviour where Data : IGrid2DData, new()
    {
        [SerializeField] protected Grid2DPreset preset; // The settings for the grid
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
        /// Calculates the world space position of a coordinate in the grid. This can be overridden by derived classes.
        /// </summary>
        /// <param name="positionKey"></param>
        /// <returns></returns>
        public virtual Vector3 GetWorldSpacePosition(Vector2Int positionKey)
        {
            return new Vector3(positionKey.x * preset.coordinateSize, positionKey.y * preset.coordinateSize, 0);
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
