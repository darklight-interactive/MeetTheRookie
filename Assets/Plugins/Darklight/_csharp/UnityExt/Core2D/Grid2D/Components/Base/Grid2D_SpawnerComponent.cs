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
        public void SpawnObjectAtCell(GameObject obj, Cell2D cell)
        {
            if (cell == null)
            {
                Debug.LogError("Cell is null");
                return;
            }

            Cell2D.SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>();
            if (spawnerComponent == null)
            {
                Debug.LogError("Cell does not have a spawner component");
                return;
            }

            spawnerComponent.SpawnObject(obj);
        }

        public void AdjustTransformToCell(Transform transform, Cell2D cell, SpatialUtils2D.OriginPoint originPoint = SpatialUtils2D.OriginPoint.CENTER, bool inheritWidth = true, bool inheritHeight = true, bool inheritNormal = true)
        {
            if (cell == null)
            {
                Debug.LogError("Cell is null");
                return;
            }

            Cell2D.SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>();
            if (spawnerComponent == null)
            {
                Debug.LogError("Cell does not have a spawner component");
                return;
            }

            spawnerComponent.AdjustTransform(transform, originPoint, inheritWidth, inheritHeight, inheritNormal);
        }

        public void AdjustTransformToSquareFromWidth(Transform transform, Cell2D cell, SpatialUtils2D.OriginPoint originPoint = SpatialUtils2D.OriginPoint.CENTER, bool inheritNormal = true)
        {
            Cell2D.SpawnerComponent spawnerComponent = cell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>();
            spawnerComponent.AdjustTransformToSquareFromWidth(transform, originPoint, inheritNormal);
        }

    }
}