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
    public class Grid2D_OverlapWeightSpawner : MonoBehaviour
    {
        // ======== [[ FIELDS ]] ================================== >>>>
        Grid2D _grid;

        // -- (( COMPONENTS )) -------- ))
        Grid2D_SpawnerComponent _spawnerComponent;
        Grid2D_WeightComponent _weightComponent;
        Grid2D_OverlapComponent _overlapComponent;

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE METHODS )) -------- ))
        public void Awake()
        {
            _grid = this.GetComponent<Grid2D>();
            if (_grid == null)
                _grid = this.gameObject.AddComponent<Grid2D>();

            _spawnerComponent = this.GetComponent<Grid2D_SpawnerComponent>();
            if (_spawnerComponent == null)
                _spawnerComponent = this.gameObject.AddComponent<Grid2D_SpawnerComponent>();

            _weightComponent = this.GetComponent<Grid2D_WeightComponent>();
            if (_weightComponent == null)
                _weightComponent = this.gameObject.AddComponent<Grid2D_WeightComponent>();

            _overlapComponent = this.GetComponent<Grid2D_OverlapComponent>();
            if (_overlapComponent == null)
                _overlapComponent = this.gameObject.AddComponent<Grid2D_OverlapComponent>();
        }

        public Cell2D GetBestCell()
        {
            // From all available cells, get the cells with the lowest collider count
            List<Cell2D> availableCells = _grid.GetCells();

            // Get the cells with the lowest collider count
            List<Cell2D> lowestColliderCells = _overlapComponent.GetCellsWithColliderCount(0);
            if (lowestColliderCells.Count > 0)
            {
                // If there are cells with no colliders, return one of them
                Debug.Log($"Found {lowestColliderCells.Count} cells with no colliders");
                Cell2D bestCell = _weightComponent.GetCellWithHighestWeight(lowestColliderCells);
                if (bestCell == null)
                {
                    Debug.LogError("No best cell found");
                    return null;
                }

                Debug.Log($"{this.name} OverlapWeightSpawner: Best cell found {bestCell.Name}");
                return bestCell;
            }

            // If all cells have colliders, return the cell with the lowest weight from all available cells
            return _weightComponent.GetCellWithHighestWeight(availableCells.ToList());
        }
    }
}
