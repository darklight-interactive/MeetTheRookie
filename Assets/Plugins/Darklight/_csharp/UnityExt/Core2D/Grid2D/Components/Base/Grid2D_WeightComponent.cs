using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_WeightComponent : Grid2D_BaseComponent
    {
        const int DEFAULT_WEIGHT = 5;
        const int MIN_WEIGHT = 0;

        // ======== [[ FIELDS ]] ================================== >>>>
        Dictionary<Vector2Int, int> _weightData = new Dictionary<Vector2Int, int>();

        [SerializeField] bool _showGizmos;

        [Header("Serialized Data")]
        [SerializeField] bool _autoUpdate = true;
        [SerializeField] List<Weighted_SerializedCellData> _serializedWeightData;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( BASE VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.WEIGHT,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // << SET WEIGHT FROM MAP >>
                if (!_weightData.ContainsKey(cell.Key))
                    weightComponent.SetWeight(DEFAULT_WEIGHT);
                else
                    weightComponent.SetWeight(_weightData[cell.Key]);

                weightComponent.OnInitialize(cell);
                return true;
            });

        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.WEIGHT,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // << UPDATE WEIGHT FROM MAP >>
                if (_weightData.ContainsKey(cell.Key))
                    weightComponent.SetWeight(_weightData[cell.Key]);
                SaveWeight(weightComponent);

                weightComponent.OnUpdate();
                return true;
            });

        // -- (( CUSTOM VISITORS )) -------- ))
        private Cell2D.ComponentVisitor _randomizeVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.WEIGHT, (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // << SET RANDOM WEIGHT >>
                weightComponent.SetRandomWeight();
                SaveWeight(weightComponent);

                return true;
            });
        private Cell2D.ComponentVisitor _resetVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.WEIGHT, (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // << SET WEIGHT TO DEFAULT >>
                weightComponent.SetWeight(DEFAULT_WEIGHT);
                SaveWeight(weightComponent);

                return true;
            });

        private Cell2D.ComponentVisitor _loadDataVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.WEIGHT, (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // Set the cell's weight component to the internal weight data
                if (_weightData.ContainsKey(cell.Key))
                    weightComponent.SetWeight(_weightData[cell.Key]);
                return true;
            });

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE )) : IComponent -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            base.OnInitialize(baseObj);
            LoadWeightDataToCells();
        }

        public override void OnUpdate()
        {
            _weightData.Clear();

            base.OnUpdate();
            UpdateSerializedCellData();
        }

        public override void DrawGizmos()
        {
            if (!_showGizmos) return;
            base.DrawGizmos();
        }

        // -- (( HANDLER METHODS )) -------- ))
        public void RandomizeWeights()
        {
            BaseGrid.SendVisitorToAllCells(_randomizeVisitor);
        }

        public void ResetWeights()
        {
            BaseGrid.SendVisitorToAllCells(_resetVisitor);
        }

        // -- (( GETTERS )) -------- ))
        public Cell2D GetRandomCellByWeight()
        {
            List<Cell2D.WeightComponent> weightComponents = BaseGrid.GetComponentsByType<Cell2D.WeightComponent>();
            Cell2D chosenCell = WeightedDataSelector.SelectRandomWeightedItem(weightComponents, (Cell2D.WeightComponent weightComponent) =>
            {
                return weightComponent.BaseCell;
            });

            // Begin recursive search for a cell with a weight
            if (chosenCell == null)
            {
                return GetRandomCellByWeight();
            }

            Debug.Log($"Random Weight Chosen Cell: {chosenCell.Key}");
            return chosenCell;
        }

        public Cell2D GetRandomCellByWeight(List<Cell2D> cells)
        {
            List<Cell2D.WeightComponent> weightComponents = new List<Cell2D.WeightComponent>();
            foreach (Cell2D cell in cells)
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();
                if (weightComponent != null)
                    weightComponents.Add(weightComponent);
            }

            Cell2D chosenCell = WeightedDataSelector.SelectRandomWeightedItem(weightComponents, (Cell2D.WeightComponent weightComponent) =>
            {
                return weightComponent.BaseCell;
            });

            // Begin recursive search for a cell with a weight
            if (chosenCell == null)
            {
                return GetRandomCellByWeight(cells);
            }

            Debug.Log($"Random Weight Chosen Cell: {chosenCell.Key}");
            return chosenCell;
        }

        public Dictionary<int, List<Cell2D>> GetAllCellsByWeight()
        {
            Dictionary<int, List<Cell2D>> weightMap = new Dictionary<int, List<Cell2D>>();
            foreach (KeyValuePair<Vector2Int, int> pair in _weightData)
            {
                if (!weightMap.ContainsKey(pair.Value))
                    weightMap[pair.Value] = new List<Cell2D>();
                weightMap[pair.Value].Add(BaseGrid.GetCell(pair.Key));
            }
            return weightMap;
        }

        public List<Cell2D> GetCellsWithWeight(int weight)
        {
            List<Cell2D> cells = new List<Cell2D>();
            foreach (KeyValuePair<Vector2Int, int> pair in _weightData)
            {
                if (pair.Value == weight)
                    cells.Add(BaseGrid.GetCell(pair.Key));
            }
            return cells;
        }

        public Cell2D GetCellWithHighestWeight()
        {
            Dictionary<int, List<Cell2D>> weightMap = GetAllCellsByWeight();
            int highestWeight = 0;
            foreach (int weight in weightMap.Keys)
            {
                if (weight > highestWeight)
                    highestWeight = weight;
            }

            if (highestWeight == 0)
                return null;

            List<Cell2D> cells = weightMap[highestWeight];
            return GetRandomCellByWeight(cells);
        }

        public Cell2D GetCellWithHighestWeight(List<Cell2D> cells)
        {
            if (cells.Count == 0)
            {
                Debug.LogError("Cannot get cell with highest weight from empty list.", this);
            }

            Cell2D highestWeightCell = cells[0];
            int highestWeight = 0;
            foreach (Cell2D cell in cells)
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();
                if (highestWeightCell == null || weightComponent.GetWeight() > highestWeight)
                {
                    highestWeight = weightComponent.GetWeight();
                    highestWeightCell = cell;
                }
            }
            return highestWeightCell;
        }

        public Cell2D GetCellWithLowestWeight(List<Cell2D> cells)
        {
            if (cells.Count == 0)
            {
                Debug.LogError("Cannot get cell with highest weight from empty list.", this);
            }

            Cell2D lowestWeightCell = cells[0];
            int lowestWeight = cells[0].ComponentReg.GetComponent<Cell2D.WeightComponent>().GetWeight();
            foreach (Cell2D cell in cells)
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();
                if (lowestWeightCell == null || weightComponent.GetWeight() < lowestWeight)
                {
                    lowestWeight = weightComponent.GetWeight();
                    lowestWeightCell = cell;
                }
            }
            return lowestWeightCell;
        }

        // -- (( SETTERS )) -------- ))
        public void SetCellToWeight(Cell2D cell, int weight)
        {
            Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();
            weightComponent.SetWeight(weight);
        }

        // ======== [[ PRIVATE METHODS ]] ================================== >>>>
        void UpdateSerializedCellData()
        {
            if (_weightData == null)
                _weightData = new Dictionary<Vector2Int, int>();
            if (_serializedWeightData == null)
                _serializedWeightData = new List<Weighted_SerializedCellData>();

            // Iterate through the weight data
            foreach (Vector2Int key in _weightData.Keys)
            {
                // If the serialized weight data does not contain the key, add the serialized data
                if (!_serializedWeightData.Exists(x => x.key == key))
                    _serializedWeightData.Add(new Weighted_SerializedCellData(key, _weightData[key]));
            }

            // Iterate through the serialized weight data
            for (int i = 0; i < _serializedWeightData.Count; i++)
            {
                // If the weight data does not contain the key, remove the serialized data
                if (!_weightData.ContainsKey(_serializedWeightData[i].key))
                    _serializedWeightData.RemoveAt(i);
            }

            // << AUTO SAVE >>
            if (_autoUpdate)
            {
                LoadWeightDataToCells();
            }
        }

        void SaveWeight(Cell2D.WeightComponent weightComponent)
        {
            Vector2Int cellKey = weightComponent.BaseCell.Key;
            _weightData[cellKey] = weightComponent.GetWeight();
        }

        void LoadWeightDataToCells()
        {
            if (_weightData == null)
                _weightData = new Dictionary<Vector2Int, int>();
            if (_serializedWeightData == null)
            {
                _serializedWeightData = new List<Weighted_SerializedCellData>();
                return;
            }

            // Set the weight data to match the serialized weight data
            foreach (Weighted_SerializedCellData data in _serializedWeightData)
            {
                _weightData[data.key] = data.weight;
            }
            BaseGrid.SendVisitorToAllCells(_loadDataVisitor);
        }

        // ======== [[ NESTED TYPES ]] ================================== >>>>

        [System.Serializable]
        public class Weighted_SerializedCellData
        {
            [ShowOnly] public string name;
            [ShowOnly] public Vector2Int key;
            [Range(MIN_WEIGHT, 10)] public int weight;
            public Weighted_SerializedCellData(Vector2Int key, int weight)
            {
                this.name = $"Cell ({key.x},{key.y})";
                this.key = key;
                this.weight = weight;
            }
        }


#if UNITY_EDITOR
        [CustomEditor(typeof(Grid2D_WeightComponent))]
        public class Grid2D_WeightComponentCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            Grid2D_WeightComponent _script;
            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (Grid2D_WeightComponent)target;
                _script.Awake();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Reset Weights")) _script.ResetWeights();

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }

                _script.Update();
            }

            private void OnSceneGUI()
            {
                _script.DrawEditorGizmos();
            }
        }
#endif
    }
}
