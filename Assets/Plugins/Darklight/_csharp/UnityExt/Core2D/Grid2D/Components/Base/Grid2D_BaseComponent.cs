using System.Collections.Generic;

namespace Darklight.UnityExt.Core2D
{
    public class Grid2D_BaseComponent : Grid2D.Component
    {
        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor CellComponent_InitVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, Cell2D.EventRegistry.BaseInitFunc);
        protected override Cell2D.ComponentVisitor CellComponent_UpdateVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, Cell2D.EventRegistry.BaseUpdateFunc);
        protected override Cell2D.ComponentVisitor CellComponent_BaseGizmosVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, Cell2D.EventRegistry.BaseGizmosFunc);
        protected override Cell2D.ComponentVisitor CellComponent_SelectedGizmosVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, Cell2D.EventRegistry.BaseSelectedGizmosFunc);
        protected override Cell2D.ComponentVisitor CellComponent_EditorGizmosVisitor =>
            Cell2D.VisitorFactory.CreateComponentVisitor(this, Cell2D.EventRegistry.BaseEditorGizmosFunc);
    }
}