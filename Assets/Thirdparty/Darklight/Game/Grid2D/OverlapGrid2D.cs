using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;

namespace Darklight.Game.Grid2D
{

    [ExecuteAlways]
    public class OverlapGrid2D : MonoBehaviour
    {
        public Grid2D<Collider2D[]> grid2D;
        public LayerMask layerMask;

        public virtual void Awake()
        {
            grid2D.SetParent(transform);
            grid2D.InitializeGridToSetValues();
        }

        public virtual void Update()
        {
            UpdateOverlapGrid();
        }

        public virtual void Reset()
        {
            grid2D.SetParent(transform);
            grid2D.InitializeGridToSetValues();
            UpdateOverlapGrid();
        }

        void UpdateOverlapGrid()
        {
            foreach (Vector2Int vector2Int in grid2D.GetPositionKeys())
            {
                // Get the world position of the coordinate
                Vector3 worldPosition = grid2D.GetCoordinatePositionInWorldSpace(vector2Int);

                // Create an overlap box at the world position
                Collider2D[] colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * grid2D.coordinateSize, 0, layerMask);

                // Store the list in the grid
                grid2D.SetCoordinateValue(vector2Int, colliders, $"{vector2Int} :: {colliders.Length} colliders");

                Coordinate gridCoordinate = grid2D.GetCoordinate(vector2Int);
                // Assign color
                switch (colliders.Length)
                {
                    case <= 0:
                        gridCoordinate.color = Color.white;
                        break;
                    case 1:
                        gridCoordinate.color = Color.yellow;
                        break;
                    case >= 2:
                        gridCoordinate.color = Color.red;
                        break;
                }
            }
        }

        public Dictionary<int, List<Coordinate>> GetCoordinatesByColliderCount()
        {
            Dictionary<int, List<Coordinate>> coordinatesByColliderCount = new Dictionary<int, List<Coordinate>>();
            foreach (Vector2Int vector2Int in grid2D.GetPositionKeys())
            {
                Coordinate coordinate = grid2D.GetCoordinate(vector2Int);
                if (coordinatesByColliderCount.ContainsKey(coordinate.typeValue.Length))
                {
                    coordinatesByColliderCount[coordinate.typeValue.Length].Add(coordinate);
                }
                else
                {
                    coordinatesByColliderCount.Add(coordinate.typeValue.Length, new List<Coordinate> { coordinate });
                }
            }
            return coordinatesByColliderCount;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OverlapGrid2D), true)]
    public class OverlapGrid2DEditor : Editor
    {
        private void OnEnable()
        {
            OverlapGrid2D overlapGridScript = (OverlapGrid2D)target;
            overlapGridScript.Awake();
        }

        private void OnSceneGUI()
        {
            OverlapGrid2D overlapGridScript = (OverlapGrid2D)target;
            overlapGridScript.Update();

            DisplayOverlapGrid2D(overlapGridScript);
        }

        public void DisplayOverlapGrid2D(OverlapGrid2D overlapGrid2D)
        {
            List<Vector2Int> positionKeys = overlapGrid2D.grid2D.GetPositionKeys();
            if (positionKeys != null && positionKeys.Count > 0)
            {
                foreach (Vector2Int vector2Int in positionKeys)
                {
                    Coordinate coordinate = overlapGrid2D.grid2D.GetCoordinate(vector2Int);
                    Vector3 worldPosition = overlapGrid2D.grid2D.GetCoordinatePositionInWorldSpace(vector2Int);
                    CustomGizmos.DrawWireSquare_withLabel(
                        $"{coordinate.label}",
                        worldPosition,
                        overlapGrid2D.grid2D.coordinateSize,
                        Vector3.forward,
                        Color.grey,
                        CustomGUIStyles.RightAlignedStyle);

                    CustomGizmos.DrawButtonHandle(worldPosition, overlapGrid2D.grid2D.coordinateSize * 0.75f, Vector3.forward, coordinate.color, () =>
                    {
                        string out_string = $"Clicked on {coordinate.label}";

                        Collider2D[] colliders = coordinate.typeValue;
                        foreach (Collider2D collider in colliders)
                        {
                            out_string += $"\n--->> Collider: {collider.name}";
                        }

                        Debug.Log(out_string);
                    }, Handles.RectangleHandleCap);
                }
            }
        }

    }

#endif
}
