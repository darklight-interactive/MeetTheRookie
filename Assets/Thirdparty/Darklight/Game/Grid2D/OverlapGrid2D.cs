using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        #region ============== [[ DATA CLASS ]] =============================== >>
        /// <summary>
        /// OverlapData class to store the collider data and manage the overlap box data.
        /// </summary>
        [System.Serializable]
        public class OverlapData
        {
            public bool active = true;
            public Vector2Int positionKey;
            public Vector3 worldPosition;
            public Collider2D[] colliders = new Collider2D[0];
            public int colliderCount => colliders.Length;
            public string label => $"weight: {weightData}";
            [SerializeField, Range(0, 5)] private float weightValue = 1;

            /// <summary>
            ///  This is intended to be used as a way to grade the availability of the overlap boxes;
            /// </summary>
            public float weightData => weightValue * colliderCount;

            public void CycleWeightValue()
            {
                weightValue = weightValue < 5 ? weightValue + 1 : 0;
                if (weightValue == 0)
                {
                    active = false;
                    Debug.Log($"Weight value reset to 0 for {positionKey}.");
                }
                else
                {
                    Debug.Log($"Weight value set to {weightValue} for {positionKey}.");
                    active = true;
                }
            }

            public Color GetColor()
            {
                if (!active)
                    return Color.black;
                if (colliderCount == 0)
                    return Color.green;
                else if (colliderCount > 0)
                    return Color.Lerp(Color.green, Color.red, Mathf.InverseLerp(0, 3, weightData));

                return Color.black;
            }

            public OverlapData(Vector2Int positionKey)
            {
                this.positionKey = positionKey;
            }
        }
        #endregion

        public Grid2D<OverlapData> dataGrid;
        public LayerMask layerMask;
        public List<OverlapData> ActiveOverlapData
        {
            get
            {
                if (dataGrid == null) return null;
                List<OverlapData> overlapDataList = dataGrid.GetDataValues();
                if (overlapDataList == null || overlapDataList.Count == 0) return null;
                return overlapDataList.Where(x => x.active).ToList();
            }
        }
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
            if (dataGrid == null) return null;
            List<OverlapData> overlapDataList = ActiveOverlapData;
            if (overlapDataList == null || overlapDataList.Count == 0) return null;
            OverlapData lowestWeightData = overlapDataList.OrderBy(x => x.weightData).FirstOrDefault();
            return lowestWeightData;
        }

        void UpdateOverlapData()
        {
            if (dataGrid == null) return;
            foreach (Vector2Int vector2Int in dataGrid.GetPositionKeys())
            {
                OverlapData activeData = dataGrid.GetData(vector2Int);
                if (activeData == null)
                {
                    // Create a new overlap data object
                    OverlapData overlapData = new OverlapData(vector2Int);
                    dataGrid.SetCoordinateValue(vector2Int, overlapData);
                    continue;
                }
                else if (activeData != null && activeData.active)
                {
                    // Update the active data object

                    // Get the world position of the coordinate
                    Vector3 worldPosition = dataGrid.GetWorldSpacePosition(vector2Int);
                    activeData.worldPosition = worldPosition;

                    // Create an overlap box at the world position
                    activeData.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * dataGrid.coordinateSize, 0, layerMask);
                    continue;
                }
            }
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
                    if (overlapData == null) continue;

                    Vector3 worldPosition = grid2D.GetWorldSpacePosition(vector2Int);
                    Color color = overlapData.GetColor();
                    float weightValue = overlapData.weightData;
                    float size = grid2D.coordinateSize;
                    Vector3 direction = Vector3.forward;

                    CustomGizmos.DrawLabel(overlapData.label, worldPosition, CustomGUIStyles.BoldStyle);
                    CustomGizmos.DrawWireSquare(worldPosition, size, direction, color);
                    CustomGizmos.DrawButtonHandle(worldPosition, size * 0.75f, direction, color, () =>
                    {
                        overlapData.CycleWeightValue();
                    }, Handles.RectangleHandleCap);
                }
            }
        }

    }

#endif
}
