using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using Darklight.UnityExt.Utility;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_WeightComponent : Grid2D.BaseComponent
    {
        const string DATA_OBJECT_PATH = "Assets/Resources/Darklight/Grid2D/WeightData";
        const int DEFAULT_WEIGHT = 5;
        const int MIN_WEIGHT = 0;

        // ======== [[ FIELDS ]] ================================== >>>>
        [SerializeField, Expandable] Grid2D_WeightDataObject _weightDataObject;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        #region -- (( BASE VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor CellComponent_InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.WEIGHT,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                if (_weightDataObject == null)
                    return false;

                // << SET WEIGHT FROM MAP >>
                if (!_weightDataObject.ContainsKey(cell.Key))
                {
                    weightComponent.SetWeight(DEFAULT_WEIGHT);
                    _weightDataObject.Add(cell.Key, DEFAULT_WEIGHT);

                }
                else
                    weightComponent.SetWeight(_weightDataObject[cell.Key]);

                weightComponent.OnInitialize(cell);
                return true;
            });

        protected override Cell2D.ComponentVisitor CellComponent_UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(ComponentTypeKey.WEIGHT,
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                if (_weightDataObject == null)
                    return false;

                // << UPDATE WEIGHT FROM MAP >>
                if (_weightDataObject.ContainsKey(cell.Key)
                    && _weightDataObject[cell.Key] != weightComponent.GetWeight())
                {
                    weightComponent.SetWeight(_weightDataObject[cell.Key]);
                    //Debug.Log($"Updating Weight: {cell.Key} to {_weightDataObject[cell.Key]}");
                }
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
                return true;
            });
        private Cell2D.ComponentVisitor _resetVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.WEIGHT, (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                // << SET WEIGHT TO DEFAULT >>
                weightComponent.SetWeight(DEFAULT_WEIGHT);
                return true;
            });

        private Cell2D.ComponentVisitor _loadDataVisitor => Cell2D.VisitorFactory.CreateComponentVisitor
            (ComponentTypeKey.WEIGHT, (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D.WeightComponent>();

                if (_weightDataObject == null)
                    return false;

                // Set the cell's weight component to the internal weight data
                if (_weightDataObject.ContainsKey(cell.Key))
                    weightComponent.SetWeight(_weightDataObject[cell.Key]);
                return true;
            });
        #endregion

        // ======== [[ METHODS ]] ================================== >>>>
        private void OnValidate()
        {

        }

        // -- (( INTERFACE )) : IComponent -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
#if UNITY_EDITOR
            if (_weightDataObject == null)
            {
                _weightDataObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<Grid2D_WeightDataObject>(DATA_OBJECT_PATH, "DefaultWeightDataObject");
            }
#endif

            base.OnInitialize(baseObj);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
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
            foreach (KeyValuePair<Vector2Int, int> pair in _weightDataObject)
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
            foreach (KeyValuePair<Vector2Int, int> pair in _weightDataObject)
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
        void LoadWeightDataToCells()
        {
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
    }
}

