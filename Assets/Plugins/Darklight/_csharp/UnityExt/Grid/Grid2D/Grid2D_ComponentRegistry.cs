using System;
using System.Collections.Generic;
using System.ComponentModel;
using Darklight.UnityExt.Editor;
using UnityEngine;

namespace Darklight.UnityExt.Game.Grid
{
    public partial class Grid2D
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
            CONFIG,
            OVERLAP,
            WEIGHT,
            SPAWNER,
            WEIGHTED_SPAWNER
        }

        public class ComponentRegistry
        {
            // ======== [[ STATIC FIELDS ]] ================================== >>>>
            private static readonly Dictionary<ComponentTypeKey, Type> _typeMap = new Dictionary<ComponentTypeKey, Type>()
            {
                { ComponentTypeKey.BASE, typeof(Grid2D_BaseComponent) },
                { ComponentTypeKey.CONFIG, typeof(Grid2D_ConfigComponent) },
                { ComponentTypeKey.OVERLAP, typeof(Grid2D_OverlapComponent) },
                { ComponentTypeKey.WEIGHT, typeof(Grid2D_WeightComponent) },
                { ComponentTypeKey.SPAWNER, typeof(Grid2D_SpawnerComponent) },
                { ComponentTypeKey.WEIGHTED_SPAWNER, typeof(Grid2D_WeightedSpawner) }
            };

            // Reverse map for faster lookups in GetTypeKey methods
            private static readonly Dictionary<Type, ComponentTypeKey> _reverseTypeMap = new Dictionary<Type, ComponentTypeKey>();

            private readonly Grid2D _grid;
            private List<Grid2D_Component> _components = new List<Grid2D_Component>();

            // ======== [[ CONSTRUCTORS ]] ================================== >>>>
            static ComponentRegistry()
            {
                // Initialize reverse map for constant time lookups
                foreach (var pair in _typeMap)
                {
                    _reverseTypeMap[pair.Value] = pair.Key;
                }
            }

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

            // ---- (( STATIC METHODS )) -------- ))
            public static ComponentTypeKey GetTypeKey<TComponent>() where TComponent : Grid2D_Component
            {
                if (_reverseTypeMap.TryGetValue(typeof(TComponent), out var key))
                {
                    return key;
                }

                throw new InvalidEnumArgumentException(
                    $"Component type {typeof(TComponent)} is not registered in the factory.");
            }

            public static ComponentTypeKey GetTypeKey(Grid2D_Component component)
            {
                if (_reverseTypeMap.TryGetValue(component.GetType(), out var key))
                {
                    return key;
                }

                throw new InvalidEnumArgumentException(
                    $"Component type {component.GetType()} is not registered in the factory.");
            }
        }
    }
}
