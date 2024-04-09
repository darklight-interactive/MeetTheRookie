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
        public Grid2D<Collider2D[]> collider2DGrid;
        public LayerMask layerMask;

        public void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            if (collider2DGrid != null)
            {
                collider2DGrid.SetParent(this.transform);
                collider2DGrid.InitializeGridToSetValues();
            }

            Update();
        }

        public void Update()
        {
            foreach (Vector2Int vector2Int in collider2DGrid.GetPositionKeys())
            {
                // Get the world position of the coordinate
                Vector3 worldPosition = collider2DGrid.GetCoordinatePositionInWorldSpace(vector2Int);

                // Create an overlap box at the world position
                Collider2D[] colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * collider2DGrid.coordinateSize, 0, layerMask);

                // Store the list in the grid
                collider2DGrid.SetCoordinateValue(vector2Int, colliders, $"{vector2Int} :: {colliders.Length} colliders");

                Coordinate gridCoordinate = collider2DGrid.GetCoordinate(vector2Int);
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

        public Coordinate GetCoordinate(Vector2Int positionKey)
        {
            if (!collider2DGrid.GetPositionKeys().Contains(positionKey)) return null;

            return collider2DGrid.GetCoordinate(positionKey);
        }

        public Dictionary<int, List<Coordinate>> GetCoordinatesByColliderCount()
        {
            Dictionary<int, List<Coordinate>> coordinatesByColliderCount = new Dictionary<int, List<Coordinate>>();
            foreach (Vector2Int vector2Int in collider2DGrid.GetPositionKeys())
            {
                Coordinate coordinate = collider2DGrid.GetCoordinate(vector2Int);
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
        OverlapGrid2D overlapGridScript;
        private void OnEnable()
        {
            overlapGridScript = (OverlapGrid2D)target;
            overlapGridScript.Awake();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                overlapGridScript.Reset();
            }
        }

        private void OnSceneGUI()
        {
            overlapGridScript = (OverlapGrid2D)target;
            DisplayGrid2D(this.overlapGridScript.collider2DGrid);
        }

        public void DisplayGrid2D(Grid2D.Grid2D<Collider2D[]> grid2D)
        {
            List<Vector2Int> positionKeys = grid2D.GetPositionKeys();
            if (positionKeys != null && positionKeys.Count > 0)
            {
                foreach (Vector2Int vector2Int in positionKeys)
                {
                    Coordinate coordinate = grid2D.GetCoordinate(vector2Int);
                    Vector3 worldPosition = grid2D.GetCoordinatePositionInWorldSpace(vector2Int);
                    CustomGizmos.DrawWireSquare_withLabel(
                        $"{coordinate.label}",
                        worldPosition,
                        grid2D.coordinateSize,
                        Vector3.forward,
                        Color.grey,
                        CustomGUIStyles.RightAlignedStyle);

                    CustomGizmos.DrawButtonHandle(worldPosition, grid2D.coordinateSize * 0.75f, Vector3.forward, coordinate.color, () =>
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
