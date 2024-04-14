using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;
using Darklight.Game.Grid;
using Unity.Android.Gradle.Manifest;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{


    /// <summary>
    /// The most basic implementation of a Grid2D class. This class is used to store Grid2DData objects in a 2D grid.
    /// </summary>
    public class Grid2D_BaseGrid : Grid2D_AbstractGrid<Grid2D_Data>
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

                    // Set the data in the map
                    if (DataMap.ContainsKey(positionKey))
                        DataMap[positionKey] = newData;
                    else
                        DataMap.Add(positionKey, newData);
                }
            }
        }

        protected override void OnDataChanged(Vector2Int position, Grid2D_Data data)
        {
            throw new NotImplementedException();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D_BaseGrid), true)]
    public class Grid2D_BaseGridEditor : Editor
    {

        private Grid2D_BaseGrid grid2D;
        private void OnEnable()
        {
            grid2D = target as Grid2D_BaseGrid;
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