using UnityEngine;
using Darklight.UnityExt.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.Game.Grid
{
    /// <summary>
    /// A 2D Grid that stores Overlap_Grid2DData objects. 
    /// </summary>
    [ExecuteAlways]
    public class OverlapGrid2D : Grid2D<OverlapGrid2D_Data>
    {
        [Header("Overlap Grid2D Properties")]
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] public bool showGizmos = true;

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

            DataMap.Clear();
            for (int x = 0; x < GridArea.x; x++)
            {
                for (int y = 0; y < GridArea.y; y++)
                {
                    Vector2Int positionKey = new Vector2Int(x, y);
                    Vector3 worldPosition = GetWorldSpacePosition(positionKey);

                    OverlapGrid2D_Data newData = new OverlapGrid2D_Data();
                    Grid2D_SerializedData existingData = preset.LoadData(positionKey);
                    if (existingData != null)
                    {
                        newData.Initialize(existingData, worldPosition, preset.coordinateSize);
                        newData.layerMask = layerMask;

                    }
                    else
                    {
                        newData.Initialize(positionKey, worldPosition, preset.coordinateSize, layerMask);
                    }

                    // Set the data in the map
                    if (DataMap.ContainsKey(positionKey))
                        DataMap[positionKey] = newData;
                    else
                        DataMap.Add(positionKey, newData);

                    // Notify listeners of the data change
                    newData.OnDataStateChanged += (data) =>
                    {
                        preset.SaveData(data);
                    };
                }
            }
        }

        public void Update()
        {
            foreach (OverlapGrid2D_Data data in DataMap.Values)
            {
                Vector3 worldPosition = GetWorldSpacePosition(data.positionKey);
                data.worldPosition = worldPosition;

                data.UpdateData();
            }
        }

        public OverlapGrid2D_Data GetBestData()
        {
            OverlapGrid2D_Data bestData = null;

            foreach (OverlapGrid2D_Data data in DataMap.Values)
            {
                if (bestData == null) { bestData = data; }

                if (data.disabled) continue; // Skip disabled data
                if (data.colliders.Length > 0) continue; // Skip data with colliders

                // If the data has a higher or equal weight and less colliders, set it as the best data
                if (data.weight >= bestData.weight)
                {
                    bestData = data;
                }
            }
            return bestData;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OverlapGrid2D), true)]
    public class OverlapGrid2DEditor : Editor
    {
        private OverlapGrid2D grid2D;
        private void OnEnable()
        {
            grid2D = (OverlapGrid2D)target;
            grid2D.Awake();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void OnSceneGUI()
        {
            if (grid2D == null) return;
            if (grid2D.showGizmos == false) return;
            DrawGrid();
        }

        public void DrawGrid()
        {
            if (grid2D == null) return;

            foreach (Vector2Int positionKey in grid2D.GetPositionKeys())
            {
                Grid2D_Data data = grid2D.GetData(positionKey);
                if (data.initialized == false) continue; // Skip uninitialized data

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
