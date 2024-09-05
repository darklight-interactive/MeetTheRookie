using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_SerializedDataObject : ScriptableObject
    {
        [SerializeField] Cell2D[] savedCells;

        public List<Cell2D> LoadCells()
        {
            // Return a new list of cells from the saved cells array
            List<Cell2D> savedCellsList = savedCells.ToList();
            List<Cell2D> clonedCells = new();
            foreach (Cell2D cell in savedCellsList)
            {
                clonedCells.Add(cell.Clone());
            }
            return clonedCells;
        }

        public void SaveCells(List<Cell2D> cells)
        {
            // Create a clone of the cells list
            List<Cell2D> newCells = new List<Cell2D>(cells);
            List<Cell2D> clonedCells = new();
            foreach (Cell2D cell in newCells)
            {
                clonedCells.Add(cell.Clone());
            }

            // Save the cloned cells to the saved cells array
            savedCells = clonedCells.ToArray();
        }
        public void ClearData() => savedCells = new Cell2D[0];
    }
}
