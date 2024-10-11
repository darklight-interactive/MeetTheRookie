using UnityEngine;
using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using UnityEngine.Events;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Core2D
{

    public partial class Grid2D
    {

        /// <summary>
        /// The base MonoBehaviour class for all Grid2D components. <br/>
        /// <para>Grid2D components are used to extend the functionality of a Grid2D object.</para>
        /// </summary>
        [ExecuteAlways]
        [RequireComponent(typeof(Grid2D))]
        public abstract class Component : MonoBehaviour, IComponent<Grid2D>
        {
            // ======== [[ FIELDS ]] ================================== >>>>
            Grid2D _baseGrid;

            [SerializeField, ShowOnly] int _guid;
            [SerializeField, ShowOnly] ComponentTypeKey _typeKey;
            [SerializeField] bool _showGizmos;

            // ======== [[ PROPERTIES ]] ================================== >>>>
            public Grid2D BaseGrid
            {
                get
                {
                    if (_baseGrid == null) _baseGrid = GetComponent<Grid2D>();
                    return _baseGrid;
                }
                private set => _baseGrid = value;
            }

            public int GUID => _guid;
            public ComponentTypeKey TypeKey => GetTypeKey();
            public bool ShowGizmos
            {
                get
                {
                    if (BaseGrid.ShowGizmos == false)
                        return false;
                    else
                        return _showGizmos;
                }
            }

            // -- (( VISITORS )) -------- ))
            protected abstract Cell2D.ComponentVisitor CellComponent_InitVisitor { get; }
            protected abstract Cell2D.ComponentVisitor CellComponent_UpdateVisitor { get; }
            protected abstract Cell2D.ComponentVisitor CellComponent_BaseGizmosVisitor { get; }
            protected abstract Cell2D.ComponentVisitor CellComponent_SelectedGizmosVisitor { get; }
            protected abstract Cell2D.ComponentVisitor CellComponent_EditorGizmosVisitor { get; }

            // ======== [[ METHODS ]] ================================== >>>>

            // -- (( UNITY METHODS )) -------- ))
            public void Awake() => OnInitialize(BaseGrid);
            public void Update() => OnUpdate();
            public void OnDrawGizmos() => DrawGizmos();
            public void OnDrawGizmosSelected() => DrawSelectedGizmos();

            // -- (( INTERFACE METHODS )) -------- ))
            public virtual void OnInitialize(Grid2D baseObj)
            {
                _guid = System.Guid.NewGuid().GetHashCode();
                _typeKey = GetTypeKey();
                BaseGrid = baseObj;

                if (BaseGrid == null)
                {
                    Debug.LogError("Grid2D_Component: BaseGrid is null. Cannot initialize component.");
                    return;
                }
                BaseGrid.SendVisitorToAllCells(CellComponent_InitVisitor);
            }
            public virtual void OnUpdate()
            {
                BaseGrid.SendVisitorToAllCells(CellComponent_UpdateVisitor);
            }
            public virtual void DrawGizmos()
            {
                if (ShowGizmos == false) return;
                BaseGrid.SendVisitorToAllCells(CellComponent_BaseGizmosVisitor);
            }
            public virtual void DrawSelectedGizmos()
            {
                if (ShowGizmos == false) return;
                BaseGrid.SendVisitorToAllCells(CellComponent_SelectedGizmosVisitor);
            }
            public virtual void DrawEditorGizmos()
            {
                BaseGrid.SendVisitorToAllCells(CellComponent_EditorGizmosVisitor);
            }

            // -- (( GETTERS )) -------- ))
            public virtual ComponentTypeKey GetTypeKey() => ComponentRegistry.GetTypeKey(this);


#if UNITY_EDITOR
            [CustomEditor(typeof(Component), true)]
            public class Grid2D_ComponentCustomEditor : UnityEditor.Editor
            {
                SerializedObject _serializedObject;
                Component _script;

                private void OnEnable()
                {
                    _serializedObject = new SerializedObject(target);
                    _script = (Component)target;
                    _script.Awake();
                }

                void OnDisable()
                {
                    _script._showGizmos = false;
                    EditorUtility.SetDirty(_script);
                }

                public override void OnInspectorGUI()
                {
                    if (_serializedObject == null) return;
                    _serializedObject.Update();

                    EditorGUI.BeginChangeCheck();

                    // < DEFAULT INSPECTOR > ------------------------------------ >>
                    CustomInspectorGUI.DrawDefaultInspectorWithoutSelfReference(_serializedObject);

                    if (EditorGUI.EndChangeCheck())
                    {
                        _serializedObject.ApplyModifiedProperties();
                    }
                    _script.Update();

                }

                private void OnSceneGUI()
                {
                    _script.DrawEditorGizmos();
                }
            }
#endif
        }

        public abstract class BaseComponent : Component
        {
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

        public abstract class CompositeComponent<TComponentA, TComponentB> : BaseComponent
            where TComponentA : Component
            where TComponentB : Component
        {
            protected TComponentA _componentA;
            protected TComponentB _componentB;

            public override void OnInitialize(Grid2D baseObj)
            {
                if (_componentA == null) _componentA = GetComponent<TComponentA>();
                if (_componentA == null) _componentA = gameObject.AddComponent<TComponentA>();

                if (_componentB == null) _componentB = GetComponent<TComponentB>();
                if (_componentB == null) _componentB = gameObject.AddComponent<TComponentB>();

                base.OnInitialize(baseObj);
            }
        }

        public abstract class CompositeComponent<TComponentA, TComponentB, TComponentC> : CompositeComponent<TComponentA, TComponentB>
            where TComponentA : Component
            where TComponentB : Component
            where TComponentC : Component
        {
            protected TComponentC _componentC;

            public override void OnInitialize(Grid2D baseObj)
            {
                if (_componentC == null) _componentC = GetComponent<TComponentC>();
                if (_componentC == null) _componentC = gameObject.AddComponent<TComponentC>();

                base.OnInitialize(baseObj);
            }
        }
    }
}