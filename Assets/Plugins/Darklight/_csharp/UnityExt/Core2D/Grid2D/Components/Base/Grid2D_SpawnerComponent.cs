using Darklight.UnityExt.Utility;
using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_SpawnerComponent : Grid2D_BaseComponent
    {
        public Spatial2D.AnchorPoint anchorPointTag = Spatial2D.AnchorPoint.CENTER;
        public bool inheritWidth = true;
        public bool inheritHeight = true;
        public bool inheritNormal = true;

        protected Cell2D.ComponentVisitor SetOriginVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, new Cell2D.EventRegistry.VisitCellComponentEvent((Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
                cellSpawner.SetCellOrigin(anchorPointTag);
                return true;
            }));

        public override void OnUpdate()
        {
            base.OnUpdate();

            BaseGrid.SendVisitorToAllCells(SetOriginVisitor);
        }


        public void InstantiateObjectAtCell(GameObject obj, Cell2D cell)
        {
            if (cell == null)
            {
                Debug.LogError("No best cell found");
                return;
            }

            // Instantiate the object at the best cell
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            cellSpawner.InstantiateObject(obj);
        }

        public void AdjustTransformToCell(Transform transform, Cell2D cell)
        {
            if (cell == null)
            {
                Debug.LogError("No best cell found");
                return;
            }

            // Adjust the transform to the best cell
            Cell2D.SpawnerComponent cellSpawner = cell.GetComponent<Cell2D.SpawnerComponent>();
            cellSpawner.AdjustTransformToCellValues(transform, anchorPointTag, inheritWidth, inheritHeight, inheritNormal);
        }

    }
}