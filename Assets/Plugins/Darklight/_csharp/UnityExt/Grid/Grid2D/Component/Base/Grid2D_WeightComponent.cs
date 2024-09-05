using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_WeightComponent : Grid2D_Component
    {
        const int DEFAULT_WEIGHT = 100;
        const int MIN_WEIGHT = 0;

        // ======== [[ FIELDS ]] ================================== >>>>
        [SerializeField] bool _showGizmos;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( BASE VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(Cell2D.ComponentTypeKey.WEIGHT,
            (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_WeightComponent weightComponent = cell.ComponentReg.GetComponent<Cell2D_WeightComponent>();
                weightComponent.SetWeight(DEFAULT_WEIGHT);
                weightComponent.OnInitialize(cell);
                return true;
            });
        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateBaseUpdateVisitor(Cell2D.ComponentTypeKey.WEIGHT);
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

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE )) : IComponent -------- ))
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

                if (GUILayout.Button("Randomize Weights"))
                {
                    _script.RandomizeWeights();
                }
                if (GUILayout.Button("Reset Weights"))
                {
                    _script.ResetWeights();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif
    }
}

