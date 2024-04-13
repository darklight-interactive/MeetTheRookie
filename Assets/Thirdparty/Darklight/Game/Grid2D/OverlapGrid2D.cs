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
        private List<IGrid2DData> _dataValues = new List<IGrid2DData>();
        private Dictionary<Vector2Int, bool> _activeMap = new Dictionary<Vector2Int, bool>();

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
            _dataValues = dataGrid.GetAllData();

            // Update the overlap data for each coordinate
            foreach (Vector2Int positionKey in _positionKeys)
            {
                IGrid2DData data = dataGrid.GetData(positionKey);
                if (data == null)
                {
                    // Create a new overlap data object if it doesn't exist
                    data = new OverlapGrid2DData(dataGrid, positionKey, layerMask);
                    dataGrid.SetData(positionKey, data);

                    if (_activeMap.ContainsKey(positionKey))
                    {
                        data.SetActive(_activeMap[positionKey]);
                    }
                    else
                    {
                        data.SetActive(false);
                    }
                }

                data.UpdateData();
                UpdateActiveMap(positionKey, data.active);
            }
        }

        public void UpdateActiveMap(Vector2Int positionKey, bool active)
        {
            if (_activeMap.ContainsKey(positionKey))
            {
                _activeMap[positionKey] = active;
                dataGrid.GetData(positionKey).SetActive(active);
            }
            else
            {
                _activeMap.Add(positionKey, active);
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
