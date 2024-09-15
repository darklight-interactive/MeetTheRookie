using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Core2D
{
    [RequireComponent(typeof(Grid2D))]
    [RequireComponent(typeof(Grid2D_WeightComponent), typeof(Grid2D_OverlapComponent))]
    public class Grid2D_OverlapWeightSpawner : Grid2D_SpawnerComponent
    {
        // ======== [[ FIELDS ]] ================================== >>>>
        Grid2D _grid;
        Grid2D_SpawnerComponent _grid_spawnerComponent;
        Grid2D_WeightComponent _grid_weightComponent;
        Grid2D_OverlapComponent _grid_overlapComponent;

        // ======== [[ PROPERTIES ]] ================================== >>>>    
        public Grid2D Grid { get => _grid; }
        public Grid2D_SpawnerComponent SpawnerComponent { get => _grid_spawnerComponent; }
        public Grid2D_WeightComponent WeightComponent { get => _grid_weightComponent; }
        public Grid2D_OverlapComponent OverlapComponent { get => _grid_overlapComponent; }

        // ======== [[ METHODS ]] ================================== >>>>
        // -- (( INTERFACE METHODS )) -------- ))
        public override void OnInitialize(Grid2D grid)
        {
            if (_grid == null)
            {
                _grid = this.GetComponent<Grid2D>();
                if (_grid == null)
                    _grid = this.gameObject.AddComponent<Grid2D>();
            }

            _grid_spawnerComponent = this.GetComponent<Grid2D_SpawnerComponent>();
            if (_grid_spawnerComponent == null)
                _grid_spawnerComponent = this.gameObject.AddComponent<Grid2D_SpawnerComponent>();

            _grid_weightComponent = this.GetComponent<Grid2D_WeightComponent>();
            if (_grid_weightComponent == null)
                _grid_weightComponent = this.gameObject.AddComponent<Grid2D_WeightComponent>();

            _grid_overlapComponent = this.GetComponent<Grid2D_OverlapComponent>();
            if (_grid_overlapComponent == null)
                _grid_overlapComponent = this.gameObject.AddComponent<Grid2D_OverlapComponent>();

            base.OnInitialize(grid);
        }

        public Cell2D GetBestCell()
        {
            // From all available cells, get the cells with the lowest collider count
            List<Cell2D> availableCells = _grid.GetCells();

            // Get the cells with the lowest collider count
            List<Cell2D> lowestColliderCells = _grid_overlapComponent.GetCellsWithColliderCount(0);
            if (lowestColliderCells.Count > 0)
            {
                // If there are cells with no colliders, return one of them
                //Debug.Log($"Found {lowestColliderCells.Count} cells with no colliders");
                Cell2D bestCell = _grid_weightComponent.GetCellWithHighestWeight(lowestColliderCells);
                if (bestCell == null)
                {
                    Debug.LogError("No best cell found");
                    return null;
                }

                Debug.Log($"{this.name} OverlapWeightSpawner: Best cell found {bestCell.Name}");
                return bestCell;
            }

            // If all cells have colliders, return the cell with the lowest weight from all available cells
            return _grid_weightComponent.GetCellWithHighestWeight(availableCells.ToList());
        }

        public Spatial2D.AnchorPoint GetAnchorPointFromCell(Cell2D cell)
        {
            return _grid_spawnerComponent.GetTargetAnchor(cell);
        }

        public Spatial2D.AnchorPoint GetOriginPointFromCell(Cell2D cell)
        {
            return _grid_spawnerComponent.GetOriginAnchor(cell);
        }
    }
}
