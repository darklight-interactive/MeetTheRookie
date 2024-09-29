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
    public class Grid2D_OverlapWeightSpawner : Grid2D.CompositeComponent<
        Grid2D_OverlapComponent, Grid2D_WeightComponent, Grid2D_SpawnerComponent>
    {
        public Grid2D_OverlapComponent OverlapComponent { get => _componentA; }
        public Grid2D_WeightComponent WeightComponent { get => _componentB; }
        public Grid2D_SpawnerComponent SpawnerComponent { get => _componentC; }

        // ======== [[ METHODS ]] ================================== >>>>
        public List<Vector2Int> GetCellKeys()
        {
            return BaseGrid.CellKeys.ToList();
        }

        public Cell2D GetBestCell()
        {
            // From all available cells, get the cells with the lowest collider count
            List<Cell2D> availableCells = BaseGrid.GetCells();

            // Get the cells with the lowest collider count
            List<Cell2D> lowestColliderCells = OverlapComponent.GetCellsWithColliderCount(0);
            if (lowestColliderCells.Count > 0)
            {
                // If there are cells with no colliders, return one of them
                //Debug.Log($"Found {lowestColliderCells.Count} cells with no colliders");
                Cell2D bestCell = WeightComponent.GetCellWithHighestWeight(lowestColliderCells);
                if (bestCell == null)
                {
                    Debug.LogError("No best cell found");
                    return null;
                }

                //Debug.Log($"{this.name} OverlapWeightSpawner: Best cell found {bestCell.Name}");
                return bestCell;
            }

            // If all cells have colliders, return the cell with the lowest weight from all available cells
            return WeightComponent.GetCellWithHighestWeight(availableCells.ToList());
        }

        public Cell2D GetNextAvailableCell()
        {
            // << GET CELLS WITH NO OVERLAP >>
            List<Cell2D> availableCells = OverlapComponent.GetCellsWithColliderCount(0);

            // << CELLS WITH NO ATTTACHED TRANSFORMS >>
            List<Cell2D> cellsWithNoAttachedObjects = new List<Cell2D>();
            foreach (Cell2D cell in availableCells)
            {
                if (SpawnerComponent.GetAttachedTransformsAtCell(cell).Count == 0)
                {
                    cellsWithNoAttachedObjects.Add(cell);
                }
            }

            // << GET CELL WITH HIGHEST WEIGHT >>
            if (cellsWithNoAttachedObjects.Count > 0)
            {
                return WeightComponent.GetCellWithHighestWeight(cellsWithNoAttachedObjects);
            }

            return null;
        }

        public Spatial2D.AnchorPoint GetAnchorPointFromCell(Cell2D cell)
        {
            return SpawnerComponent.GetTargetAnchor(cell);
        }

        public Spatial2D.AnchorPoint GetOriginPointFromCell(Cell2D cell)
        {
            return SpawnerComponent.GetOriginAnchor(cell);
        }
    }
}
