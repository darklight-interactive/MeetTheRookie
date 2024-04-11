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

            public void SetToDefaultValues()
            {
                weightValue = 1;
                active = true;
            }

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

        [SerializeField] public Grid2D<OverlapData> dataGrid;
        public LayerMask layerMask;

        public void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            dataGrid = new Grid2D<OverlapData>(this.transform);
            dataGrid.SetParent(this.transform);
        }

        public void Update()
        {
            UpdateOverlapData();
        }

        public void SetToDefaultValues()
        {
            List<Vector2Int> positionKeys = dataGrid.GetPositionKeys();
            foreach (Vector2Int vector2Int in positionKeys)
            {
                OverlapData overlapData = dataGrid.GetData(vector2Int);
                if (overlapData == null) continue;
                overlapData.SetToDefaultValues();
            }
        }

        void UpdateOverlapData()
        {
            if (dataGrid == null)
            {
                return;
            }

            // Loop through all the active overlap data objects
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

        public List<OverlapData> GetActiveOverlapData()
        {
            List<OverlapData> output = new List<OverlapData>();
            List<Vector2Int> positionKeys = dataGrid.GetPositionKeys();
            foreach (Vector2Int vector2Int in positionKeys)
            {
                OverlapData overlapData = dataGrid.GetData(vector2Int);
                if (overlapData == null) continue;
                if (overlapData.active)
                {
                    output.Add(overlapData);
                }
            }
            return output;
        }

        public OverlapData GetOverlapDataWithLowestWeightValue()
        {
            OverlapData lowestWeightData = null;
            float lowestWeightValue = float.MaxValue;
            List<OverlapData> activeOverlapData = GetActiveOverlapData();
            if (activeOverlapData == null || activeOverlapData.Count == 0)
            {
                Debug.Log("No active overlap data found.");
                return null;
            }

            foreach (OverlapData overlapData in activeOverlapData)
            {
                if (overlapData.weightData < lowestWeightValue)
                {
                    lowestWeightData = overlapData;
                    lowestWeightValue = overlapData.weightData;
                }
            }
            return lowestWeightData;
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
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();



            if (EditorGUI.EndChangeCheck())
            {
            }
        }

        private void OnSceneGUI()
        {
            overlapGridScript = (OverlapGrid2D)target;
            overlapGridScript.Update();
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
