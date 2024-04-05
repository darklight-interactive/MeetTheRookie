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

    public class OverlapGrid2D : MonoBehaviour
    {
        [SerializeField] public Grid2D<Collider2D[]> grid2D;

        public void Awake()
        {
            grid2D = new Grid2D<Collider2D[]>(this.transform, new Vector2Int(3, 3), 1);
        }

        public void Update()
        {
            UpdateOverlapGrid();
        }

        public void UpdateOverlapGrid()
        {
            foreach (Vector2Int vector2Int in grid2D.GetPositionKeys())
            {
                // Get the world position of the coordinate
                Vector3 worldPosition = grid2D.GetCoordinatePositionInWorldSpace(vector2Int);

                // Create an overlap box at the world position
                Collider2D[] colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * grid2D.coordinateSize, 0);

                // Store the list in the grid
                grid2D.SetCoordinateValue(vector2Int, colliders, $"{colliders.Length} colliders");

            }
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OverlapGrid2D))]
    public class OverlapGrid2DEditor : Editor
    {
        private void OnEnable()
        {
            OverlapGrid2D grid2D = (OverlapGrid2D)target;
            grid2D.Awake();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            OverlapGrid2D grid2D = (OverlapGrid2D)target;

            if (GUILayout.Button("Update Grid"))
            {
                grid2D.UpdateOverlapGrid();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }

        }

        private void OnSceneGUI()
        {
            OverlapGrid2D targetGrid2D = (OverlapGrid2D)target;
            foreach (Vector2Int vector2Int in targetGrid2D.grid2D.GetPositionKeys())
            {
                Coordinate coordinate = targetGrid2D.grid2D.GetCoordinate(vector2Int);
                Vector3 worldPosition = targetGrid2D.grid2D.GetCoordinatePositionInWorldSpace(vector2Int);
                CustomGizmos.DrawWireSquare_withLabel(
                    $"{coordinate.label}",
                    worldPosition,
                    targetGrid2D.grid2D.coordinateSize,
                    Vector3.forward,
                    Color.red,
                    CustomGUIStyles.RightAlignedStyle);
            }
        }
    }

#endif
}
