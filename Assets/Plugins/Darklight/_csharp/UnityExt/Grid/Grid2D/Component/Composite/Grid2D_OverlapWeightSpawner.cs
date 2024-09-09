using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Game.Grid
{
    [RequireComponent(typeof(Grid2D), typeof(Grid2D_SpawnerComponent))]
    [RequireComponent(typeof(Grid2D_WeightComponent), typeof(Grid2D_OverlapComponent))]
    public class Grid2D_OverlapWeightSpawner : Grid2D_BaseComponent
    {
        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( COMPONENTS )) -------- ))
        Grid2D_SpawnerComponent _spawnerComponent;
        Grid2D_WeightComponent _weightComponent;
        Grid2D_OverlapComponent _overlapComponent;

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE METHODS )) -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            base.OnInitialize(baseObj);

            _spawnerComponent = baseObj.GetComponent<Grid2D_SpawnerComponent>();
            if (_spawnerComponent == null)
                _spawnerComponent = baseObj.gameObject.AddComponent<Grid2D_SpawnerComponent>();

            _weightComponent = baseObj.GetComponent<Grid2D_WeightComponent>();
            if (_weightComponent == null)
                _weightComponent = baseObj.gameObject.AddComponent<Grid2D_WeightComponent>();

            _overlapComponent = baseObj.GetComponent<Grid2D_OverlapComponent>();
            if (_overlapComponent == null)
                _overlapComponent = baseObj.gameObject.AddComponent<Grid2D_OverlapComponent>();
        }

        public Cell2D GetBestCell()
        {
            // From all available cells, get the cells with the lowest collider count
            List<Cell2D> availableCells = BaseGrid.GetCells();

            // Get the cells with the lowest collider count
            List<Cell2D> lowestColliderCells = _overlapComponent.GetCellsWithColliderCount(0);
            if (lowestColliderCells.Count > 0)
            {
                // If there are cells with no colliders, return one of them
                Debug.Log($"Found {lowestColliderCells.Count} cells with no colliders");
                Cell2D bestCell = _weightComponent.GetCellWithHighestWeight(lowestColliderCells.ToList());
                return bestCell;
            }

            // If all cells have colliders, return the cell with the lowest weight from all available cells
            return _weightComponent.GetCellWithHighestWeight(availableCells.ToList()); ;
        }
    }
}
