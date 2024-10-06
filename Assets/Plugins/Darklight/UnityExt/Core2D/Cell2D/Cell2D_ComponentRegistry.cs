using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    partial class Cell2D
    {
        public class InternalComponentRegistry
        {
            static Dictionary<ComponentTypeKey, Type> _componentTypeMap = new()
            {
                { ComponentTypeKey.BASE, typeof(BaseComponent) },
                { ComponentTypeKey.OVERLAP, typeof(OverlapComponent) },
                { ComponentTypeKey.WEIGHT, typeof(WeightComponent) },
                { ComponentTypeKey.SPAWNER, typeof(SpawnerComponent) },
            };

            // ======== [[ FIELDS ]] ======================================================= >>>>
            Cell2D _cell;
            Dictionary<ComponentTypeKey, Component> _componentMap = new();
            [SerializeField] List<Component> _components = new();


            // ======== [[ CONSTRUCTORS ]] ======================================================= >>>>
            public InternalComponentRegistry(Cell2D cell)
            {
                _cell = cell;
                _componentMap = new Dictionary<ComponentTypeKey, Component>();
                _components = new List<Component>();
            }

            public InternalComponentRegistry(InternalComponentRegistry originComposite)
            {
                _cell = originComposite._cell;
                LoadComponents(originComposite._components);
            }

            // ======== [[ METHODS ]] ============================================================ >>>>
            public Component RegisterComponent(ComponentTypeKey type, Visitor visitor = null)
            {
                if (!HasComponent(type))
                {
                    Component component = ComponentFactory.CreateComponent(type, _cell);
                    _componentMap.Add(type, component);

                    if (visitor != null)
                        visitor.Visit(_cell);

                    Refresh();
                    return component;
                }
                return null;
            }

            public Component GetComponent(ComponentTypeKey type)
            {
                if (_componentMap.ContainsKey(type))
                {
                    return _componentMap[type];
                }
                return null;
            }

            public TComponent GetComponent<TComponent>() where TComponent : Component
            {
                foreach (Component component in _components)
                {
                    if (component is TComponent)
                    {
                        return (TComponent)component;
                    }
                }
                return default;
            }

            public ComponentTypeKey[] GetComponentTypeKeys()
            {
                return _componentMap.Keys.ToArray();
            }

            public void RemoveComponent(ComponentTypeKey type)
            {
                // If the component is in the dictionary, remove it.
                if (_componentMap.ContainsKey(type))
                {
                    _componentMap.Remove(type);
                }
                Refresh();
            }

            public void LoadComponents(List<Component> originComponents)
            {
                _componentMap = new Dictionary<ComponentTypeKey, Component>();
                foreach (Component component in originComponents)
                {
                    ComponentTypeKey type = component.GetTypeKey();
                    Component newComponent = ComponentFactory.CreateComponent(type, _cell);
                    _componentMap.Add(type, newComponent);
                }
                Refresh();
            }

            public bool HasComponent(ComponentTypeKey type)
            {
                if (_componentMap.ContainsKey(type))
                {
                    if (_componentMap[type] != null)
                    {
                        return true;
                    }
                }
                return false;
            }

            void Refresh()
            {
                _components = _componentMap.Values.ToList();
            }

            // ---- (( STATIC METHODS )) -------- ))
            public static ComponentTypeKey GetTypeKey<TComponent>() where TComponent : Component
            {
                foreach (var pair in _componentTypeMap)
                {
                    if (pair.Value == typeof(TComponent))
                    {
                        return pair.Key;
                    }
                }
                throw new InvalidEnumArgumentException(
                    $"Component type {typeof(TComponent)} is not registered in the factory.");
            }

            public static ComponentTypeKey GetTypeKey(Cell2D.Component component)
            {
                foreach (var pair in _componentTypeMap)
                {
                    if (pair.Value == component.GetType())
                    {
                        return pair.Key;
                    }
                }
                throw new InvalidEnumArgumentException(
                    $"Component type {component.GetType()} is not registered in the factory.");
            }
        }
    }
}