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
        #region ============== [[ DATA CLASS ]] =============================== >>
        /// <summary>
        /// OverlapData class to store the collider data and manage the overlap box data.
        /// </summary>
        [System.Serializable]
        public class OverlapData : IGrid2DData
        {
            public bool active
            {
                get
                {
                    bool isEmpty = colliderCount == 0;
                    return isEmpty;
                }
            }
            public Color activeColor => active ? Color.green : Color.black;
            public Vector2Int positionKey { get; set; }
            public Vector3 worldPosition { get; set; }
            public Collider2D[] colliders = new Collider2D[0];
            public int colliderCount => colliders.Length;
            public string label
            {
                get
                {
                    string outString = "";
                    outString += $"active >> {active}";
                    outString += $"\npos >> {positionKey}";
                    outString += $"\ncolliders >> {colliderCount}";
                    return outString;
                }
            }
            public OverlapData(Vector2Int positionKey)
            {
                this.positionKey = positionKey;
            }
        }
        #endregion

        public SO_Grid2DSettings grid2DSettings;
        public Grid2D<IGrid2DData> dataGrid { get; private set; }
        private List<Vector2Int> _positionKeys = new List<Vector2Int>();
        private List<IGrid2DData> _dataValues = new List<IGrid2DData>();
        private Dictionary<Vector2Int, bool> _activeMap = new Dictionary<Vector2Int, bool>();

        [Tooltip("The layer mask to use for the overlap box. This will determine which coordinates are active based on how many colliders of this layer mask are found at each overlap coordinate.")]
        public LayerMask layerMask;
        public void Awake()
        {
            dataGrid = new Grid2D<IGrid2DData>(transform, grid2DSettings);
            Debug.Log("OverlapGrid2D Awake", this);
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
                    data = new OverlapData(positionKey);
                    dataGrid.SetData(positionKey, data);
                }

                // set the world position of the overlap data
                data.worldPosition = dataGrid.GetWorldSpacePosition(positionKey);
                if (!_activeMap.Keys.Contains(data.positionKey))
                {
                    _activeMap[data.positionKey] = data.active;
                }

                Vector3 worldPosition = dataGrid.GetWorldSpacePosition(positionKey);
                (data as OverlapData).colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * dataGrid.settings.coordinateSize, 0, layerMask);
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
