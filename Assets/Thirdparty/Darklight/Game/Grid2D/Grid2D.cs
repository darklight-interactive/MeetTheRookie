using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.CustomEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{

    /// <summary>
    /// An abstract 2D Grid class to be used as a base for different types of grids storing data inheriting from Grid2DData.
    /// </summary>
    public abstract class Grid2D<Data> : MonoBehaviour where Data : IGrid2D_Data, new()
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



    /// <summary>
    /// The most basic implementation of a Grid2D class. This class is used to store Grid2DData objects in a 2D grid.
    /// </summary>
    public class BasicGrid2D : Grid2D<Grid2D_Data>
    {
        public override void Awake()
        {
            base.Awake();
            InitializeDataMap();
        }

        protected override void InitializeDataMap()
        {
            if (preset == null)
            {
                Debug.LogError("The Grid2D preset is not set.", this);
                return;
            }

            // Create the grid data
            for (int x = 0; x < preset.gridSizeX; x++)
            {
                for (int y = 0; y < preset.gridSizeY; y++)
                {
                    Vector2Int positionKey = new Vector2Int(x, y);
                    Vector3 worldPosition = GetWorldSpacePosition(positionKey);

                    // Create the data object
                    Grid2D_Data newData = new Grid2D_Data();
                    Grid2D_SerializedData existingData = preset.LoadData(positionKey);
                    if (existingData != null)
                    {
                        // Initialize the data with the existing data values
                        newData.Initialize(existingData, worldPosition, preset.coordinateSize);
                    }
                    else
                    {
                        // Initialize the data with default values
                        newData.Initialize(positionKey, false, 1, worldPosition, preset.coordinateSize);
                    }

                    // Set the data in the map ------------- >>
                    if (DataMap.ContainsKey(positionKey))
                        DataMap[positionKey] = newData;
                    else
                        DataMap.Add(positionKey, newData);

                    // Notify listeners of the data change
                    newData.OnDataStateChanged += (data) =>
                    {
                        // Save the data when the data state changes
                        preset.SaveData(data);
                    };
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BasicGrid2D), true)]
    public class Grid2DEditor : Editor
    {

        private BasicGrid2D grid2D;
        private void OnEnable()
        {
            grid2D = target as BasicGrid2D;
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
                Grid2D_Data data = grid2D.GetData(positionKey);
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