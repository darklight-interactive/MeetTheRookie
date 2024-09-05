namespace Darklight.UnityExt.Game.Grid
{
    partial class Cell2D
    {
        public static class VisitorFactory
        {
            public static ComponentVisitor CreateComponentVisitor
                (ComponentTypeKey type, EventRegistry.VisitCellComponentEvent visitFunction)
            {
                return new ComponentVisitor(type, visitFunction);
            }

            public static ComponentVisitor CreateBaseRegisterVisitor(ComponentTypeKey type)
            {
                return new ComponentVisitor(type, EventRegistry.BaseRegisterFunc);
            }

            public static ComponentVisitor CreateInitVisitor(ComponentTypeKey type)
            {
                return new ComponentVisitor(type, EventRegistry.BaseInitFunc);
            }

            public static ComponentVisitor CreateBaseUpdateVisitor(ComponentTypeKey type)
            {
                return new ComponentVisitor(type, EventRegistry.BaseUpdateFunc);
            }

            public static ComponentVisitor CreateBaseGizmosVisitor(ComponentTypeKey type)
            {
                return new ComponentVisitor(type, EventRegistry.BaseGizmosFunc);
            }

            public static ComponentVisitor CreateBaseEditorGizmosVisitor(ComponentTypeKey type)
            {
                return new ComponentVisitor(type, EventRegistry.BaseEditorGizmosFunc);
            }
        }
    }
}