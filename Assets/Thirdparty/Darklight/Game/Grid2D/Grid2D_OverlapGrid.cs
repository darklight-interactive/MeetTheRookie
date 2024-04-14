using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Darklight.Game.Grid
{
    /// <summary>
    /// Create and stores the data from a Physics2D.OverlapBoxAll call at the world position of the Grid2DData. 
    /// </summary>
    public class Grid2D_OverlapData : Grid2D_Data
    {
        public LayerMask layerMask; // The layer mask to use for the OverlapBoxAll called
        public Collider2D[] colliders = new Collider2D[0]; /// The colliders found by the OverlapBoxAll call
        public Grid2D_OverlapData() { }
        // Initialization method to set properties

        public void Initialize(Vector2Int positionKey, Vector3 worldPosition, float coordinateSize, LayerMask layerMask)
        {
            base.Initialize(positionKey, disabled, weight, worldPosition, coordinateSize);
            this.layerMask = layerMask;
        }

        public override void UpdateData()
        {
            // Update the collider data
            this.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * coordinateSize, 0, layerMask);
            this.SetDisabled(colliders.Length > 0);
        }
    }


    /// <summary>
    /// A 2D Grid that stores Overlap_Grid2DData objects. 
    /// </summary>
    [ExecuteAlways]
    public class Grid2D_OverlapGrid : Grid2D_AbstractGrid<Grid2D_OverlapData>
    {
        [SerializeField] protected LayerMask layerMask;

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

                    Grid2D_OverlapData newData = new Grid2D_OverlapData();
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
                }
            }
        }

        public void Update()
        {
            foreach (Grid2D_OverlapData data in DataMap.Values)
            {
                Vector3 worldPosition = GetWorldSpacePosition(data.positionKey);
                data.worldPosition = worldPosition;

                data.UpdateData();
            }
        }

        protected override void OnDataChanged(Vector2Int position, Grid2D_OverlapData data)
        {
            throw new System.NotImplementedException();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Grid2D_OverlapGrid), true)]
    public class Grid2D_OverlapGridEditor : Editor
    {
        private Grid2D_OverlapGrid grid2D;
        private void OnEnable()
        {
            grid2D = (Grid2D_OverlapGrid)target;
            grid2D.Awake();
        }

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();

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
                Grid2D_Data data = grid2D.GetData(positionKey);
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
