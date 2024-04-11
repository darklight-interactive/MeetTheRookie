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
            public string label => $"weight:\n\t value {weightValue} \n\t data {weightData}";
            [Range(0, 5)] public float weightValue;
            public Color debugColor => GetColor();

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

            Color GetColor()
            {
                if (!active)
                    return Color.black;
                else if (weightData == 0)
                    return Color.green;
                else if (weightData == 1)
                    return Color.yellow;
                else if (weightData >= 2)
                    return Color.red;
                return Color.white;
            }

            public OverlapData(Vector2Int positionKey, int weight = 1)
            {
                this.positionKey = positionKey;
                this.weightValue = weight;
            }
        }
        #endregion

        public Grid2D<OverlapData> dataGrid;
        public List<Vector2Int> activePositions = new List<Vector2Int>();
        public LayerMask layerMask;

        public void Awake()
        {
            dataGrid.SetParent(this.transform);
            dataGrid.Initialize();
        }

        public void Initialize()
        {

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
            List<Vector2Int> positionKeys = dataGrid.GetPositionKeys();
            List<OverlapData> dataValues = dataGrid.GetDataValues();

            // Loop through all the active overlap data objects
            foreach (Vector2Int vector2Int in positionKeys)
            {
                Vector3 worldPosition = dataGrid.GetWorldSpacePosition(vector2Int);

                if (dataGrid.GetData(vector2Int) == null)
                {
                    dataGrid.SetCoordinateValue(vector2Int, new OverlapData(vector2Int));
                }

                OverlapData newData = dataGrid.GetData(vector2Int);
                newData.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * dataGrid.coordinateSize, 0, layerMask);
                newData.worldPosition = worldPosition;
                newData.active = activePositions.Contains(vector2Int);
                dataGrid.SetCoordinateValue(vector2Int, newData);
            }

            // remove any overlap data that is not in the position keys
            foreach (OverlapData overlapData in dataValues)
            {
                if (!positionKeys.Contains(overlapData.positionKey))
                {
                    dataGrid.RemoveCoordinate(overlapData.positionKey);
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

        public OverlapData GetDataWithLowestWeightData()
        {
            float lowestValue = float.MaxValue;
            List<OverlapData> activeOverlapData = GetActiveOverlapData();
            if (activeOverlapData == null || activeOverlapData.Count == 0)
            {
                Debug.Log("No active overlap data found.");
                return null;
            }

            OverlapData outData = null;
            foreach (OverlapData overlapData in activeOverlapData)
            {
                if (overlapData.weightData < lowestValue)
                {
                    lowestValue = overlapData.weightData;
                    outData = overlapData;
                }
            }
            return outData;
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
            overlapGridScript = (OverlapGrid2D)target;
            if (GUILayout.Button("Initialize"))
            {
                overlapGridScript.dataGrid.Initialize();
            }
            if (EditorGUI.EndChangeCheck())
            {
                overlapGridScript.Update();
                serializedObject.ApplyModifiedProperties();
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
                    Vector3 worldPosition = grid2D.GetWorldSpacePosition(vector2Int);
                    Color color = overlapData.debugColor;
                    float weightValue = overlapData.weightValue;
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
