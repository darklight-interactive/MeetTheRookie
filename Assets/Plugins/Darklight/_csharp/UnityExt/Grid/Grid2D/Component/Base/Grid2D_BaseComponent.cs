using System.Collections.Generic;

namespace Darklight.UnityExt.Game.Grid
{
    public class Grid2D_BaseComponent : Grid2D_Component
    {
        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( VISITORS )) -------- ))
        protected override Cell2D.ComponentVisitor InitVisitor =>
            Cell2D.VisitorFactory.CreateInitVisitor(Cell2D.ComponentTypeKey.BASE);
        protected override Cell2D.ComponentVisitor UpdateVisitor =>
            Cell2D.VisitorFactory.CreateBaseUpdateVisitor(Cell2D.ComponentTypeKey.BASE);
        protected override Cell2D.ComponentVisitor GizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseGizmosVisitor(Cell2D.ComponentTypeKey.BASE);
        protected override Cell2D.ComponentVisitor EditorGizmosVisitor =>
            Cell2D.VisitorFactory.CreateBaseEditorGizmosVisitor(Cell2D.ComponentTypeKey.BASE);
    }
}