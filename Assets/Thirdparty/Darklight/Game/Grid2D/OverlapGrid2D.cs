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
    [ExecuteAlways]
    public class OverlapGrid2D : MonoBehaviour
    {
        public SO_Grid2DSettings grid2DSettings;
        public LayerMask layerMask;
        public Grid2D<IGrid2DData> dataGrid { get; private set; }
        private List<Vector2Int> _positionKeys = new List<Vector2Int>();

        public void Awake()
        {
            dataGrid = new Grid2D<IGrid2DData>(transform, grid2DSettings);
        }

        public virtual void Update()
        {
            UpdateOverlapData();
        }

        void UpdateOverlapData()
        {
            if (dataGrid == null) return;
            _positionKeys = dataGrid.GetPositionKeys();

            if (grid2DSettings == null) return;

            // Update the overlap data for each coordinate
            foreach (Vector2Int positionKey in _positionKeys)
            {
                IGrid2DData data = dataGrid.GetData(positionKey);
                if (data == null)
                {
                    // Check if the spawn weight map contains the position key
                    if (grid2DSettings.spawnWeightMap.ContainsKey(positionKey))
                    {
                        // Set the data to the active state and weight value from the spawn weight map
                        (bool active, int weight) = grid2DSettings.spawnWeightMap[positionKey];
                        data = new OverlapGrid2DData(dataGrid, positionKey, active, weight, layerMask);
                    }
                    else
                    {
                        // Create a new overlap data with the default values
                        data = new OverlapGrid2DData(dataGrid, positionKey, false, 0, layerMask);
                        grid2DSettings.SetSpawnWeight(positionKey, false, 0);
                    }

                    dataGrid.SetData(positionKey, data);
                }

                data.UpdateData();
            }
        }

        public Vector2Int? GetBestPositionKey()
        {
            Dictionary<Vector2Int, (bool, int)> spawnWeightMap = grid2DSettings.spawnWeightMap;
            Vector2Int? bestPositionKey = null;
            int bestWeight = 0;
            if (spawnWeightMap == null || spawnWeightMap.Count == 0)
            {
                Debug.Log("No spawn weight map found.");
                return null;
            }

            if (_positionKeys == null || _positionKeys.Count == 0)
            {
                Debug.Log("No position keys found.");
                return null;
            }

            foreach (Vector2Int positionKey in _positionKeys)
            {
                if (dataGrid.GetData(positionKey) == null) continue;
                if (dataGrid.GetData(positionKey).active == false) continue;

                (bool active, int weight) = spawnWeightMap[positionKey];
                //Debug.Log($"Position: {positionKey} Active: {active} Weight: {weight}");

                if (!active) continue;

                if (weight > bestWeight || bestPositionKey == null)
                {
                    bestWeight = weight;
                    bestPositionKey = positionKey;
                }
            }

            //Debug.Log($"Best Position: {bestPositionKey} Weight: {bestWeight}");
            return bestPositionKey;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OverlapGrid2D), true)]
    public class OverlapGrid2DEditor : Editor
    {
        OverlapGrid2D overlapGrid2D;
        private void OnEnable()
        {
            OverlapGrid2D overlapGrid2D = (OverlapGrid2D)target;
            overlapGrid2D.Awake();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            OverlapGrid2D overlapGrid2D = (OverlapGrid2D)target;
            if (overlapGrid2D.dataGrid == null) return;

            // Draw the overlap grid
            SO_Grid2DSettings.DisplayGrid2D(overlapGrid2D.dataGrid);
        }
    }
#endif
}
