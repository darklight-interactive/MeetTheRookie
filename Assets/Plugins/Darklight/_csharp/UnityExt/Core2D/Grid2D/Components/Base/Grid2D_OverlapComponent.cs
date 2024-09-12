using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Darklight.UnityExt.Core2D
{
    [RequireComponent(typeof(Grid2D))]
    public class Grid2D_OverlapComponent : Grid2D_BaseComponent
    {
        // ======== [[ FIELDS ]] =========================== >>>>
        [SerializeField] LayerMask _layerMask;
        [SerializeField] bool _showGizmos;
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
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, InitEvent);
        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, UpdateEvent);
        #endregion

        // ======== [[ METHODS ]] ================================== >>>>
        #region -- (( INTERFACE )) : IComponent -------- ))
        public override void OnInitialize(Grid2D baseObj)
        {
            _colliderWeightMap.Clear();

            base.OnInitialize(baseObj);
        }

        public override void OnUpdate()
        {
            BaseGrid.SendVisitorToAllCells(UpdateVisitor);
        }

        public override void DrawGizmos()
        {
            if (!_showGizmos) return;
            BaseGrid.SendVisitorToAllCells(GizmosVisitor);
        }

        public override void DrawEditorGizmos()
        {
            if (!_showGizmos) return;
            BaseGrid.SendVisitorToAllCells(EditorGizmosVisitor);
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
    }
}

