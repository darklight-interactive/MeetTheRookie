using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Darklight.UnityExt.Core2D
{
    [RequireComponent(typeof(Grid2D))]
    public class Grid2D_OverlapComponent : Grid2D.BaseComponent
    {
        // ======== [[ FIELDS ]] =========================== >>>>
        [SerializeField] LayerMask _layerMask;
        Dictionary<Cell2D, int> _colliderWeightMap = new Dictionary<Cell2D, int>();

        #region ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( INITIALIZATION EVENT )) -------- ))
        protected Cell2D.EventRegistry.VisitCellComponentEvent InitEvent =>
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.OverlapComponent overlapComponent =
                    cell.ComponentReg.GetComponent(type) as Cell2D.OverlapComponent;
                if (overlapComponent == null) return false;

                // << INITIALIZATION >> 
                overlapComponent.LayerMask = _layerMask;
                _colliderWeightMap[cell] = overlapComponent.GetColliderCount();

                overlapComponent.OnInitialize(cell);
                return true;
            };

        // -- (( UPDATE EVENT )) -------- ))
        protected Cell2D.EventRegistry.VisitCellComponentEvent UpdateEvent =>
            (Cell2D cell, ComponentTypeKey type) =>
            {
                Cell2D.OverlapComponent overlapComponent =
                    cell.ComponentReg.GetComponent<Cell2D.OverlapComponent>();
                if (overlapComponent == null) return false;

                // << UPDATE >>
                overlapComponent.LayerMask = _layerMask;
                _colliderWeightMap[cell] = overlapComponent.GetColliderCount();

                overlapComponent.OnUpdate();
                return true;
            };

        // -- (( VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor CellComponent_InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, InitEvent);
        protected override Cell2D.ComponentVisitor CellComponent_UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, UpdateEvent);
        #endregion

        // ======== [[ METHODS ]] ================================== >>>>
        #region -- (( INTERFACE )) : IComponent -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            _colliderWeightMap.Clear();

            base.OnInitialize(baseObj);
        }
        #endregion

        // -- (( GETTERS )) -------- ))
        public Dictionary<int, List<Cell2D>> GetCellsByOverlap()
        {
            Dictionary<int, List<Cell2D>> overlapMap = new Dictionary<int, List<Cell2D>>();
            foreach (KeyValuePair<Cell2D, int> pair in _colliderWeightMap)
            {
                if (!overlapMap.ContainsKey(pair.Value))
                    overlapMap[pair.Value] = new List<Cell2D>();
                overlapMap[pair.Value].Add(pair.Key);
            }
            return overlapMap;
        }

        public List<Cell2D> GetCellsWithColliderCount(int count)
        {
            List<Cell2D> cells = new List<Cell2D>();
            foreach (KeyValuePair<Cell2D, int> pair in _colliderWeightMap)
            {
                if (pair.Value == count)
                    cells.Add(pair.Key);
            }
            return cells;
        }

        public Cell2D GetClosestCellWithColliderCount(int count, Vector2 position)
        {
            Cell2D closestCell = null;
            float closestDistance = float.MaxValue;
            foreach (KeyValuePair<Cell2D, int> pair in _colliderWeightMap)
            {
                if (pair.Value == count)
                {
                    float distance = Vector2.Distance(pair.Key.Position, position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCell = pair.Key;
                    }
                }
            }
            return closestCell;
        }
    }
}

