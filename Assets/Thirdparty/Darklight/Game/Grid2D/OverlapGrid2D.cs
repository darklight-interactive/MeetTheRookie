using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Darklight.Game.Grid2D
{
    using OverlapData = OverlapGrid2D.OverlapData;
    [ExecuteAlways]
    public class OverlapGrid2D : MonoBehaviour
    {

        /// <summary>
        /// OverlapData class to store the collider data and manage the overlap box data.
        /// </summary>
        [System.Serializable]
        public class OverlapData
        {
            public bool active;
            public Vector2Int positionKey;
            public Collider2D[] colliders;
            public int colliderCount => colliders.Length;
            public string label => $"{positionKey} :: {colliders.Length} colliders";


            /// <summary>
            /// -1 < 0 < 1
            /// The weight value is set to 0 by default.
            /// </summary>
            [SerializeField, Range(-1, 1)] private float weightValue = 0;

            /// <summary>
            ///  This is intended to be used as a way to grade the availability of the overlap boxes;
            /// </summary>
            public float weightData
            {
                get
                {
                    return weightValue * colliderCount;
                }
            }

            public Color debugColor
            {
                get
                {
                    if (!active) return Color.clear;
                    Color lerpColor = Color.Lerp(Color.green, Color.red, Mathf.InverseLerp(0, 10, weightData));
                    return lerpColor;
                }
            }

            public OverlapData(Vector2Int positionKey, Collider2D[] colliders)
            {
                this.positionKey = positionKey;
                this.colliders = colliders;
            }
        }

        public Grid2D<OverlapData> dataGrid;
        public LayerMask layerMask;

        public void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            if (dataGrid != null)
            {
                dataGrid.SetParent(this.transform);
                dataGrid.InitializeGridToSetValues();
            }
        }

        public void Update()
        {
            UpdateOverlapData();
        }

        public OverlapData GetOverlapDataWithLowestWeightValue()
        {
            List<Vector2Int> positionKeys = GetCoordinatesByWeightValue();
            if (positionKeys.Count > 0)
            {
                return dataGrid.GetData(positionKeys[0]);
            }
            return null;
        }

        #region [[ Private Methods ]] =============================== >>
        void UpdateOverlapData()
        {
            if (dataGrid == null) return;
            foreach (Vector2Int vector2Int in dataGrid.GetPositionKeys())
            {
                // Get the world position of the coordinate
                Vector3 worldPosition = dataGrid.GetWorldSpacePosition(vector2Int);

                // Create an overlap box at the world position
                Collider2D[] colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * dataGrid.coordinateSize, 0, layerMask);

                // Create a new OverlapGrid2DData object
                OverlapData overlapData = new OverlapData(vector2Int, colliders);

                // Store the data in the grid
                dataGrid.SetCoordinateValue(vector2Int, overlapData);
            }
        }

        List<Vector2Int> GetCoordinatesByWeightValue()
        {
            List<Vector2Int> positionKeys = dataGrid.GetPositionKeys();
            positionKeys.Sort((Vector2Int a, Vector2Int b) =>
            {
                OverlapData dataA = dataGrid.GetData(a);
                OverlapData dataB = dataGrid.GetData(b);
                return dataB.weightData.CompareTo(dataA.weightData);
            });
            return positionKeys;
        }
        #endregion
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
            DisplayGrid2D(this.overlapGridScript.dataGrid);
        }

        public void DisplayGrid2D(Grid2D<OverlapData> grid2D)
        {
            List<Vector2Int> positionKeys = grid2D.GetPositionKeys();
            if (positionKeys != null && positionKeys.Count > 0)
            {
                foreach (Vector2Int vector2Int in positionKeys)
                {
                    OverlapData overlapData = grid2D.GetData(vector2Int);
                    Vector3 worldPosition = grid2D.GetWorldSpacePosition(vector2Int);
                    CustomGizmos.DrawWireSquare_withLabel(
                        $"{overlapData.label}",
                        worldPosition,
                        grid2D.coordinateSize,
                        Vector3.forward,
                        Color.grey,
                        CustomGUIStyles.RightAlignedStyle);

                    CustomGizmos.DrawButtonHandle(worldPosition, grid2D.coordinateSize * 0.75f, Vector3.forward, overlapData.debugColor, () =>
                    {
                        overlapData.active = !overlapData.active;
                        Debug.Log($"Toggle {overlapData.positionKey} active ({overlapData.active}.");
                    }, Handles.RectangleHandleCap);
                }
            }
        }

    }

#endif
}
