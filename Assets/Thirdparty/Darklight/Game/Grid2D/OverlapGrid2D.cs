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


        /// <summary>
        /// A map of the state of the overlap data at each position key. The Value Tuple contains the active state and the weight value of the data.
        /// </summary>
        [SerializeField] private Dictionary<Vector2Int, (bool, int)> _spawnWeightMap = new Dictionary<Vector2Int, (bool, int)>();

        public void Awake()
        {
            dataGrid = new Grid2D<IGrid2DData>(transform, grid2DSettings);
        }

        public void Update()
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

        /*
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
        */
    }


}
