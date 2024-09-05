using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_OverlapComponent : Grid2D_Component
    {
        // ======== [[ FIELDS ]] =========================== >>>>
        [SerializeField] LayerMask _layerMask;
        [SerializeField] bool _showGizmos;
        Dictionary<Cell2D, int> _colliderWeightMap = new Dictionary<Cell2D, int>();

        #region ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( INITIALIZATION EVENT )) -------- ))
        protected Cell2D.EventRegistry.VisitCellComponentEvent InitEvent =>
            (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_OverlapComponent overlapComponent =
                    cell.ComponentReg.GetComponent(type) as Cell2D_OverlapComponent;
                if (overlapComponent == null) return false;

                // << INITIALIZATION >> 
                overlapComponent.LayerMask = _layerMask;
                overlapComponent.OnColliderEnter += OnColliderEnter;
                overlapComponent.OnColliderExit += OnColliderExit;
                overlapComponent.OnInitialize(cell);
                return true;
            };

        // -- (( UPDATE EVENT )) -------- ))
        protected Cell2D.EventRegistry.VisitCellComponentEvent UpdateEvent =>
            (Cell2D cell, Cell2D.ComponentTypeKey type) =>
            {
                Cell2D_OverlapComponent overlapComponent =
                    cell.ComponentReg.GetComponent<Cell2D_OverlapComponent>();
                if (overlapComponent == null) return false;

                // << UPDATE >>
                overlapComponent.LayerMask = _layerMask;
                overlapComponent.OnUpdate();
                return true;
            };

        // -- (( VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(Cell2D.ComponentTypeKey.OVERLAP, InitEvent);
        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(Cell2D.ComponentTypeKey.OVERLAP, UpdateEvent);
        protected override Cell2D.ComponentVisitor GizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseGizmosVisitor(Cell2D.ComponentTypeKey.OVERLAP);
        protected override Cell2D.ComponentVisitor EditorGizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseEditorGizmosVisitor(Cell2D.ComponentTypeKey.OVERLAP);
        #endregion

        // ======== [[ EVENTS ]] ================================== >>>>
        public UltEvent<Cell2D> HandleCollisionEnter;
        public UltEvent<Cell2D> HandleCollisionExit;

        // ======== [[ METHODS ]] ================================== >>>>
        #region -- (( INTERFACE )) : IComponent -------- ))
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



        // -- (( EVENT HANDLERS )) -------- ))
        void OnColliderEnter(Cell2D cell, Collider2D collider)
        {
            HandleCollisionEnter?.Invoke(cell);
        }

        void OnColliderExit(Cell2D cell, Collider2D collider)
        {
            HandleCollisionExit?.Invoke(cell);
        }
    }
}

