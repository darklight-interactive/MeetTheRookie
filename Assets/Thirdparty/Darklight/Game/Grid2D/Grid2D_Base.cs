using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt;
using Darklight.Game.Grid;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{


    /// <summary>
    /// The most basic implementation of a Grid2D class. This class is used to store Grid2DData objects in a 2D grid.
    /// </summary>
    public class Grid2D_Base : Grid2D_Abstract<Grid2DData>
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
                    Vector2Int position = new Vector2Int(x, y);
                    Vector3 worldPosition = GetWorldSpacePosition(position);
                    Grid2DData data = new Grid2DData();
                    data.Initialize(position, false, 0, worldPosition, preset.coordinateSize);
                    DataMap.Add(position, data);
                }
            }
        }

        protected override void OnDataChanged(Vector2Int position, Grid2DData data)
        {
            throw new NotImplementedException();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D_Abstract<Grid2DData>), true)]
    public class Grid2DEditor : Editor
    {
        Grid2D_Base grid2D;
        private void OnEnable()
        {
            grid2D = (Grid2D_Base)target;
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