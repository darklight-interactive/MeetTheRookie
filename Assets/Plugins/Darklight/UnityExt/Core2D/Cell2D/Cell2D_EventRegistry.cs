namespace Darklight.UnityExt.Core2D
{
    partial class Cell2D
    {
        public static class EventRegistry
        {
            public delegate bool VisitCellEvent(Cell2D cell);
            public delegate bool VisitCellComponentEvent(
                Cell2D cell,
                ComponentTypeKey componentTypeKey
            );

            // ======== [[ BASE VISITOR FUNCTIONS ]] ================================== >>>>
            /// <summary>
            /// Register the component to the cell.
            /// </summary>
            public static VisitCellComponentEvent BaseRegisterFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.RegisterComponent(type);
                    return component != null;
                };

            /// <summary>
            /// The Default Initialization function to call when visiting the cell.
            /// </summary>
            public static VisitCellComponentEvent BaseInitFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.GetComponent(type);
                    if (component == null)
                        return false;

                    // < INITIALIZATION >
                    if (!component.IsInitialized)
                    {
                        component.OnInitialize(cell);
                        return true;
                    }

                    return false;
                };

            /// <summary>
            /// The Default Update function to call when visiting the cell.
            /// </summary>
            public static VisitCellComponentEvent BaseUpdateFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.GetComponent(type);
                    if (component == null)
                        return false;

                    // < INITIALIZATION >
                    if (!component.IsInitialized)
                    {
                        component.OnInitialize(cell);
                        return true;
                    }

                    // < UPDATE >
                    component.OnUpdate();
                    return true;
                };

            public static VisitCellComponentEvent BaseGizmosFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.GetComponent(type);
                    if (component == null)
                        return false;

                    component.DrawGizmos();
                    return true;
                };

            public static VisitCellComponentEvent BaseSelectedGizmosFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.GetComponent(type);
                    if (component == null)
                        return false;

                    component.DrawSelectedGizmos();
                    return true;
                };

            public static VisitCellComponentEvent BaseEditorGizmosFunc =>
                (Cell2D cell, ComponentTypeKey type) =>
                {
                    Component component = cell.ComponentReg.GetComponent(type);
                    if (component == null)
                        return false;

                    component.DrawEditorGizmos();
                    return true;
                };
        }
    }
}
