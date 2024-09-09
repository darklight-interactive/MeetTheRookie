using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_WeightComponent : Grid2D_Component
    {
        const int DEFAULT_WEIGHT = 5;
        const int MIN_WEIGHT = 0;

        // ======== [[ FIELDS ]] ================================== >>>>
        Dictionary<Vector2Int, int> _weightData = new Dictionary<Vector2Int, int>();

        [SerializeField] bool _showGizmos;
        [SerializeField] bool _showEditorGizmos;
        [SerializeField] List<Weighted_SerializedCellData> _serializedWeightData;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( BASE VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(Cell2D.ComponentTypeKey.WEIGHT,
            (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();

                // Set the weight data to the default weight if it does not contain the key
                if (!_weightData.ContainsKey(cell.Key))
                    weightComponent.SetWeight(DEFAULT_WEIGHT);
                else
                    weightComponent.SetWeight(_weightData[cell.Key]);

                weightComponent.OnInitialize(cell);
                return true;
            });

        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(Cell2D.ComponentTypeKey.WEIGHT,
            (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
                weightComponent.SetWeight(_weightData[cell.Key]);
                weightComponent.OnUpdate();
                return true;
            });

        protected override Cell2D.ComponentVisitor GizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseGizmosVisitor(Cell2D.ComponentTypeKey.WEIGHT);
        protected override Cell2D.ComponentVisitor EditorGizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseEditorGizmosVisitor(Cell2D.ComponentTypeKey.WEIGHT);

        // -- (( CUSTOM VISITORS )) -------- ))
        private Cell2D.ComponentVisitor _randomizeVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (Cell2D.ComponentTypeKey.WEIGHT, (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
                weightComponent.SetRandomWeight();
                return true;
            });

        private Cell2D.ComponentVisitor _resetVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (Cell2D.ComponentTypeKey.WEIGHT, (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
                weightComponent.SetWeight(DEFAULT_WEIGHT);
                return true;
            });

        private Cell2D.ComponentVisitor _loadDataVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (Cell2D.ComponentTypeKey.WEIGHT, (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
                weightComponent.SetWeight(_weightData[cell.Key]);
                return true;
            });

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE )) : IComponent -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            base.OnInitialize(baseObj);
            LoadWeights();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateSerializedCellData();
        }

        public override void DrawGizmos()
        {
            if (!_showGizmos) return;
            base.DrawGizmos();
        }

        public override void DrawEditorGizmos()
        {
            if (!_showEditorGizmos) return;
            base.DrawEditorGizmos();
        }

        // -- (( HANDLER METHODS )) -------- ))
        public void LoadWeights()
        {
            // Set the weight data to match the serialized weight data
            foreach (Weighted_SerializedCellData data in _serializedWeightData)
            {
                _weightData[data.key] = data.weight;
            }
            BaseGrid.SendVisitorToAllCells(_loadDataVisitor);
        }

        public void RandomizeWeights()
        {
            BaseGrid.SendVisitorToAllCells(_randomizeVisitor);
        }

        public void ResetWeights()
        {
            BaseGrid.SendVisitorToAllCells(_resetVisitor);
        }

        public void SetCellToWeight(Cell2D cell, int weight)
        {
            Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
            weightComponent.SetWeight(weight);
        }

        // -- (( GETTERS )) -------- ))
        public Cell2D GetRandomCellByWeight()
        {
            List<Cell2D_WeightComponent> weightComponents = BaseGrid.GetComponentsByType<Cell2D_WeightComponent>();
            Cell2D chosenCell = WeightedDataSelector.SelectRandomWeightedItem(weightComponents, (Cell2D_WeightComponent weightComponent) =>
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

        public Cell2D GetLowestWeightedCell()
        {
            List<Cell2D_WeightComponent> weightComponents = BaseGrid.GetComponentsByType<Cell2D_WeightComponent>();
            Cell2D chosenCell = WeightedDataSelector.SelectLowestWeightedItem(weightComponents).BaseCell;

            Debug.Log($"Lowest Weight Chosen Cell: {chosenCell.Key}");
            return chosenCell;
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
                // Otherwise, update the weight data to match the serialized weight data
                else
                {
                    Vector2Int key = _serializedWeightData[i].key;
                    _weightData[key] = _serializedWeightData[i].weight;
                }
            }
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

                if (GUILayout.Button("Randomize Weights")) _script.RandomizeWeights();
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

