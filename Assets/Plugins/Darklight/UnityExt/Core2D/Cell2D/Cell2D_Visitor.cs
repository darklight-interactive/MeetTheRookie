
using System;
using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using UnityEngine;
using static Darklight.UnityExt.Core2D.Cell2D.EventRegistry;
namespace Darklight.UnityExt.Core2D
{
    partial class Cell2D
    {
        public class Visitor : IVisitor<Cell2D>
        {
            VisitCellEvent _visitFunction;
            public Visitor(VisitCellEvent visitFunction)
            {
                _visitFunction = visitFunction;
            }

            public virtual void Visit(Cell2D cell)
            {
                _visitFunction(cell);
            }
        }

        public class ComponentVisitor : IVisitor<Cell2D>
        {
            // ======== [[ FIELDS ]] ======================================================= >>>>
            ComponentTypeKey _type; // The type of component to look for
            VisitCellComponentEvent _visitFunction; // The function to call when visiting the cell

            // ======== [[ PROPERTIES ]] ======================================================= >>>>
            public VisitCellComponentEvent VisitFunc
            {
                get
                {
                    if (_visitFunction == null)
                        _visitFunction = BaseUpdateFunc;
                    return _visitFunction;
                }
                set => _visitFunction = value;
            }

            // ======== [[ CONSTRUCTOR ]] ======================================================= >>>>
            public ComponentVisitor(ComponentTypeKey type, VisitCellComponentEvent visitFunction)
            {
                _type = type;
                VisitFunc = visitFunction;
            }

            // ======== [[ METHODS ]] ======================================================= >>>>
            public void Visit(Cell2D cell)
            {
                InternalComponentRegistry componentRegistry = cell.ComponentReg;

                // Check if the stored component type exists in the cell
                if (componentRegistry.HasComponent(_type))
                {
                    VisitFunc(cell, _type);
                }
                // Register the component if it doesn't exist
                else
                {
                    BaseRegisterFunc(cell, _type);
                }
            }

        }

        public static class VisitorFactory
        {
            public static ComponentVisitor CreateComponentVisitor
                (ComponentTypeKey type, VisitCellComponentEvent visitFunction)
            {
                return new ComponentVisitor(type, visitFunction);
            }

            public static ComponentVisitor CreateComponentVisitor
                (Grid2D.Component gridComponent, VisitCellComponentEvent visitFunction)
            {
                return new ComponentVisitor(gridComponent.TypeKey, visitFunction);
            }
        }

    }
}
