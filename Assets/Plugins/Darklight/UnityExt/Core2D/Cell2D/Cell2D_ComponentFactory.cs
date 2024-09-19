using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    partial class Cell2D
    {
        public static class ComponentFactory
        {
            private static Dictionary<ComponentTypeKey, Func<Cell2D, Component>> _componentFactory = new Dictionary<ComponentTypeKey, Func<Cell2D, Component>>();

            // Static constructor to initialize the factory with component registrations
            static ComponentFactory()
            {
                RegisterComponent(ComponentTypeKey.BASE, (Cell2D cell) => new BaseComponent(cell));
                RegisterComponent(ComponentTypeKey.OVERLAP, (Cell2D cell) => new OverlapComponent(cell));
                RegisterComponent(ComponentTypeKey.WEIGHT, (Cell2D cell) => new WeightComponent(cell));
                RegisterComponent(ComponentTypeKey.SPAWNER, (Cell2D cell) => new SpawnerComponent(cell));
            }

            // Method to register a component creation function
            static void RegisterComponent(ComponentTypeKey type, Func<Cell2D, Component> factoryMethod)
            {
                if (!_componentFactory.ContainsKey(type))
                {
                    _componentFactory[type] = factoryMethod;
                }
                else
                {
                    Debug.LogWarning($"Component type {type} is already registered in the factory.");
                }
            }

            // Method to create a component based on the TypeKey
            public static Component CreateComponent(ComponentTypeKey type, Cell2D cell)
            {
                if (_componentFactory.TryGetValue(type, out Func<Cell2D, Component> factoryMethod))
                {
                    return factoryMethod(cell);
                }

                Debug.LogError($"Component type {type} is not registered in the factory.");
                return null;
            }

            public static ComponentTypeKey GetTypeKey<TComponent>() where TComponent : Component
            {
                foreach (var pair in _componentFactory)
                {
                    if (pair.Value.Method.ReturnType == typeof(TComponent))
                    {
                        return pair.Key;
                    }
                }
                throw new InvalidEnumArgumentException(
                    $"Component type {typeof(TComponent)} is not registered in the factory.");
            }
        }
    }
}
