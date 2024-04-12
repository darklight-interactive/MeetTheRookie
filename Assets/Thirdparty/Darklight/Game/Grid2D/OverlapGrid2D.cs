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
            public bool active
            {
                get
                {
                    if (colliderCount > 0) return false;
                    if (weight == 0) return false;
                    return true;
                }
            }
            public Vector2Int positionKey;
            public Vector3 worldPosition;
            public Collider2D[] colliders = new Collider2D[0];
            public int colliderCount => colliders.Length;
            public string label
            {
                get
                {
                    string outString = "";
                    outString += $"active >> {active}";
                    outString += $"\npos >> {positionKey}";
                    outString += $"\nweight >> {weight}";
                    outString += $"\ncolliders >> {colliderCount}";
                    return outString;
                }
            }
            [Range(0, 5)] public float weight = 1;
            public Color debugColor => GetColor();

            public void CycleWeight()
            {
                weight++;
                if (weight > 2)
                {
                    weight = 0;
                }
            }

            Color GetColor()
            {
                if (!active)
                    return Color.black;
                else if (weight == 0)
                    return Color.green;
                else if (weight == 1)
                    return Color.yellow;
                else if (weight >= 2)
                    return Color.red;
                return Color.white;
            }

            public OverlapData(Vector2Int positionKey)
            {
                this.positionKey = positionKey;
            }
        }
        #endregion

        public Grid2D<OverlapData> dataGrid;
        private List<Vector2Int> _activeMap = new List<Vector2Int>();
        private List<Vector2Int> _positionKeys = new List<Vector2Int>();
        private List<OverlapData> _dataValues = new List<OverlapData>();
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

        void UpdateOverlapData()
        {
            _positionKeys = dataGrid.GetPositionKeys();
            _dataValues = dataGrid.GetDataValues();
            _activeMap.Clear();

            foreach (Vector2Int positionKey in _positionKeys)
            {
                OverlapData data = dataGrid.GetData(positionKey);
                if (data == null)
                {
                    data = new OverlapData(positionKey);
                    dataGrid.SetCoordinateValue(positionKey, data);
                }
                data.worldPosition = dataGrid.GetWorldSpacePosition(positionKey);
                if (data.active)
                {
                    _activeMap.Add(data.positionKey);
                }
                else
                {
                    _activeMap.Remove(data.positionKey);
                }

                Vector3 worldPosition = dataGrid.GetWorldSpacePosition(positionKey);
                data.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * dataGrid.coordinateSize, 0, layerMask);
            }
        }

        public OverlapData GetDataWithLowestWeightData()
        {
            float lowestValue = float.MaxValue;
            // Find the overlap data with the lowest weight value
            OverlapData outData = null;
            foreach (OverlapData overlapData in _dataValues)
            {
                if (overlapData.weight < lowestValue)
                {
                    lowestValue = overlapData.weight;
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
                    if (overlapData == null) continue;
                    float weightValue = overlapData.weight;
                    float size = grid2D.coordinateSize;
                    Vector3 direction = Vector3.forward;


                    CustomGizmos.DrawLabel(overlapData.label, overlapData.worldPosition, CustomGUIStyles.BoldStyle);
                    CustomGizmos.DrawWireSquare(overlapData.worldPosition, size, direction, overlapData.debugColor);
                    CustomGizmos.DrawButtonHandle(overlapData.worldPosition, size * 0.75f, direction, overlapData.debugColor, () =>
                    {
                        overlapData.CycleWeight();
                    }, Handles.RectangleHandleCap);
                }
            }
        }

    }

#endif
}
