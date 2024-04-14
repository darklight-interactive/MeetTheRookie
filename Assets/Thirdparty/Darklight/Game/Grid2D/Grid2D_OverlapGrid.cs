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
    public class Grid2D_OverlapData : Grid2DData
    {
        public LayerMask layerMask; // The layer mask to use for the OverlapBoxAll called
        public Collider2D[] colliders = new Collider2D[0]; /// The colliders found by the OverlapBoxAll call
        public Grid2D_OverlapData() { }
        // Initialization method to set properties
        public void Initialize(Vector2Int positionKey, Vector3 worldPosition, float coordinateSize, LayerMask mask)
        {
            this.positionKey = positionKey;
            this.worldPosition = worldPosition;
            this.coordinateSize = coordinateSize;
            this.layerMask = mask;
            UpdateData(); // Ensure the colliders are updated immediately upon initialization
        }

        public override void UpdateData()
        {
            // Update the collider data
            this.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * coordinateSize, 0, layerMask);
        }
    }


    /// <summary>
    /// A 2D Grid that stores Overlap_Grid2DData objects. 
    /// </summary>
    public class Grid2D_OverlapGrid : Grid2D_Abstract<Grid2D_OverlapData>
    {
        [SerializeField]
        private LayerMask layerMask;

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
                    Vector2Int positionKey = new Vector2Int(x, y) + OriginKey;
                    Grid2D_OverlapData dataObject = new Grid2D_OverlapData();
                    Vector3 worldPosition = GetWorldSpacePosition(positionKey);

                    dataObject.Initialize(positionKey, worldPosition, preset.coordinateSize, layerMask);
                    DataMap[positionKey] = dataObject;
                }
            }
        }

        public void Update()
        {
            foreach (Grid2D_OverlapData data in DataMap.Values)
            {
                data.UpdateData();
            }
        }

        protected override void OnDataChanged(Vector2Int position, Grid2D_OverlapData data)
        {
            throw new System.NotImplementedException();
        }
    }

}
