using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using UnityEditor;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    public partial class Cell2D
    {
        [System.Serializable]
        public abstract class Component :
            IComponent<Cell2D, ComponentTypeKey>,
            IVisitable<Component>
        {
            protected Cell2D _baseCell;
            [SerializeField, ShowOnly] protected int _guid = System.Guid.NewGuid().GetHashCode();
            [SerializeField, ShowOnly] protected ComponentTypeKey _typeKey;
            protected bool _initialized = false;
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
            public abstract void OnInitialize(Cell2D cell);
            public abstract void OnUpdate();
            public abstract void DrawGizmos();
            public abstract void DrawSelectedGizmos();
            public abstract void DrawEditorGizmos();

            // ---- (( VISITOR METHODS )) ---- >>
            public virtual void Accept(IVisitor<Component> visitor)
            {
                visitor.Visit(this);
            }

            // ---- (( GETTER METHODS )) ---- >>
            public virtual ComponentTypeKey GetTypeKey() => InternalComponentRegistry.GetTypeKey(this);
        }

        public class BaseComponent : Component
        {
            public BaseComponent(Cell2D cell) : base(cell) { }
            public override void OnInitialize(Cell2D cell)
            {
                _baseCell = cell;
                _initialized = true;
            }

            public override void OnUpdate()
            {
                if (BaseCell == null) return;
            }

            public override void DrawGizmos() { }
            public override void DrawSelectedGizmos()
            {
                BaseCell.GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

#if UNITY_EDITOR
                Color faintWhite = new Color(1, 1, 1, 0.5f);
                CustomGizmos.DrawWireRect(position, dimensions, normal, faintWhite);

                Vector3 labelPos = Spatial2D.GetAnchorPointPosition(position, dimensions, Spatial2D.AnchorPoint.CENTER);
                CustomGizmos.DrawLabel($"{_baseCell.Key}", labelPos, new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                });
#endif
            }
            public override void DrawEditorGizmos() { }
            public override ComponentTypeKey GetTypeKey() => ComponentTypeKey.BASE;
        }
    }
}