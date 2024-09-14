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
        public void InstantiateObjectAtCell(GameObject obj, Cell2D cell)
        {
            // << CHECK IF CELL IS NULL >>
            if (cell == null)
            {
                Debug.LogError("Cell is null");
                return;
            }

            // << GET SPAWNER COMPONENT OF THE CELL >>
            Cell2D.SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>();
            if (spawnerComponent == null)
            {
                Debug.LogError("Cell does not have a spawner component");
                return;
            }

            // << INSTANTIATE OBJECT >>
            spawnerComponent.InstantiateObject(obj);
        }

        public void AdjustTransformToCell(Transform transform, Cell2D cell,
            bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
        {
            // << CHECK IF CELL IS NULL >>
            if (cell == null)
            {
                Debug.LogError("Cell is null");
                return;
            }

            // << GET SPAWNER COMPONENT OF THE CELL >>
            Cell2D.SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>();
            if (spawnerComponent == null)
            {
                Debug.LogError("Cell does not have a spawner component");
                return;
            }

            // << ADJUST TRANSFORM >>
            spawnerComponent.AdjustTransformToCellValues(transform, inheritWidth, inheritHeight, inheritNormal);
        }
    }
}