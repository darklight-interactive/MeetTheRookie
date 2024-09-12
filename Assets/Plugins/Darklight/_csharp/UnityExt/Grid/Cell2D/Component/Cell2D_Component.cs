using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{

    public partial class Cell2D
    {

        [System.Serializable]
        public abstract class Component :
            IComponent<Cell2D, ComponentTypeKey>,
            IVisitable<Component>
        {
            Cell2D _baseCell;
            [SerializeField, ShowOnly] int _guid = System.Guid.NewGuid().GetHashCode();
            [SerializeField, ShowOnly] ComponentTypeKey _typeKey;
            bool _initialized = false;
            // ======== [[ PROPERTIES ]] ================================== >>>>
            public Cell2D BaseCell => _baseCell;
            public int GUID => _guid;
            public ComponentTypeKey TypeKey => GetTypeKey();
            public bool Initialized => _initialized;


            // ======== [[ CONSTRUCTORS ]] ================================== >>>>
            public Component(Cell2D baseObj)
            {
                _guid = System.Guid.NewGuid().GetHashCode();
                _baseCell = baseObj;
                _typeKey = GetTypeKey();
            }

            // ======== [[ METHODS ]] ================================== >>>>
            public virtual void OnInitialize(Cell2D cell)
            {
                _baseCell = cell;
                _initialized = true;
            }

            public virtual void OnUpdate()
            {
                if (BaseCell == null) return;
            }

            public virtual ComponentTypeKey GetTypeKey() => ComponentRegistry.GetTypeKey(this);
            public abstract void DrawGizmos();
            public abstract void DrawSelectedGizmos();
            public abstract void DrawEditorGizmos();

            // ---- (( VISITOR METHODS )) ---- >>
            public virtual void Accept(IVisitor<Component> visitor)
            {
                visitor.Visit(this);
            }
        }
    }
}