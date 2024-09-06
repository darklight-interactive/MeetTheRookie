using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            List<Cell2D> cellsWithLeastOverlap = _overlapComponent.GetCellsWithLeastOverlap();
            Cell2D cell2D = _weightComponent.GetLowestWeightedCell();

            return cell2D;
        }
    }
}
