using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Core2D
{
    /// <summary>
    /// The type of Grid2D component.
    /// </summary>
    /// <remarks>
    /// <para>Each Grid2D component has a type tag that can be used to identify the component.</para>
    /// </remarks>
    public enum ComponentTypeKey
    {
        BASE,
        OVERLAP,
        WEIGHT,
        SPAWNER,
    }

    public partial class Grid2D
    {
        public class ComponentRegistry
        {
            // ======== [[ STATIC FIELDS ]] ================================== >>>>
            private static readonly Dictionary<ComponentTypeKey, Type> _typeMap = new Dictionary<ComponentTypeKey, Type>
            {
                {ComponentTypeKey.BASE, typeof(Component)},
                {ComponentTypeKey.OVERLAP, typeof(Grid2D_OverlapComponent)},
                {ComponentTypeKey.WEIGHT, typeof(Grid2D_WeightComponent)},
                {ComponentTypeKey.SPAWNER, typeof(Grid2D_SpawnerComponent)},
            };

            private readonly Grid2D _grid;
            private List<Component> _components = new List<Component>();

            // ======== [[ CONSTRUCTORS ]] ================================== >>>>
            public ComponentRegistry(Grid2D grid)
            {
                _grid = grid;
                grid.OnGridInitialized += InitializeComponents;
                grid.OnGridUpdated += UpdateComponents;
            }

            // ======== [[ METHODS ]] ================================== >>>>
            private void InitializeComponents()
            {
                _components.Clear();
                _grid.GetComponentsInChildren(_components);

                _components.ForEach(component => component.OnInitialize(_grid));
            }

            private void UpdateComponents()
            {
                _components.ForEach(component => component.Update());
            }

            // ---- (( STATIC METHODS )) -------- )))
            public static ComponentTypeKey GetTypeKey<TComponent>() where TComponent : Component
            {
                Type type = typeof(TComponent);
                foreach (Type t in _typeMap.Values)
                {
                    // Check if the type is the same or a subclass of the type.
                    if (t == type || type.IsSubclassOf(t))
                    {
                        // Return the key that corresponds to the type.
                        return _typeMap.First(pair => pair.Value == t).Key;
                    }
                }

                throw new InvalidEnumArgumentException(
                    $"Component type {typeof(TComponent)} is not registered in the factory.");
            }

            public static ComponentTypeKey GetTypeKey(Component component)
            {
                Type type = component.GetType();
                foreach (Type t in _typeMap.Values)
                {
                    // Check if the type is the same or a subclass of the type.
                    if (t == type || type.IsSubclassOf(t))
                    {
                        // Return the key that corresponds to the type.
                        return _typeMap.First(pair => pair.Value == t).Key;
                    }
                }

                throw new InvalidEnumArgumentException(
                    $"Component type {component.GetType()} is not registered in the factory.");
            }

        }
    }
}
