using System.Collections.Generic;

using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;

using UnityEngine;

namespace Darklight.UnityExt.Core2D
{

    [System.Serializable]
    public partial class Cell2D : IVisitable<Cell2D>, ISpatial2D
    {
        // ======== [[ FIELDS ]] ======================================================= >>>>
        InternalComponentRegistry _componentRegistry;

        bool _enabled;
        bool _initialized;

        [SerializeField, ShowOnly] string _name = "Cell2D";
        [SerializeField] SettingsConfig _config;
        [SerializeField] SerializedData _data;


        // ======== [[ PROPERTIES ]] ======================================================= >>>>
        public string Name { get => _name; }
        public Vector2Int Key { get => _data.Key; }
        public SettingsConfig Config { get => _config; }
        public SerializedData Data { get => _data; }
        public InternalComponentRegistry ComponentReg { get => _componentRegistry; }
        public Vector3 Position { get => Data.Position; }
        public Vector2 Dimensions { get => Data.Dimensions; }
        public Vector3 Normal { get => Data.Normal; }

        // ======== [[ CONSTRUCTORS ]] ======================================================= >>>>
        public Cell2D(Vector2Int key) => Initialize(key, null);
        public Cell2D(Vector2Int key, SettingsConfig config) => Initialize(key, config);

        // ======== [[ METHODS ]] ============================================================ >>>>
        #region -- (( RUNTIME )) -------- )))
        public void Initialize(Vector2Int key, SettingsConfig config)
        {
            // Initialize the configuration
            if (config == null)
                config = new SettingsConfig();
            _config = config;

            // Create the data
            _data = new SerializedData(key);

            // Create the composite
            _componentRegistry = new InternalComponentRegistry(this);

            // Set the name
            _name = $"Cell2D ({key.x},{key.y})";

            // << SET INITIALIZED >>
            if (_config == null || _data == null || _componentRegistry == null)
            {
                _initialized = false;
                return;
            }
            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized) return;
        }
        #endregion

        // -- (( HANDLERS )) -------- )))
        public void RecalculateDataFromGrid(Grid2D grid)
        {
            if (!_initialized) return;
            if (grid == null) return;
            if (grid.GetConfig() == null) return;
            if (Data == null) return;

            // Calculate the cell's transform
            Grid2D.SpatialUtility.CalculateCellTransform(
                out Vector3 position, out Vector2Int coordinate,
                out Vector3 normal, out Vector2 dimensions,
                this, grid.GetConfig());

            // Assign the calculated values to the cell
            Data.SetPosition(position);
            Data.SetCoordinate(coordinate);
            Data.SetNormal(normal);
            Data.SetDimensions(dimensions);
        }

        public Cell2D Clone()
        {
            Cell2D clone = new Cell2D(Data.Key);
            SerializedData newData = new SerializedData(Data);
            SettingsConfig newConfig = new SettingsConfig(Config);
            InternalComponentRegistry newComposite = new InternalComponentRegistry(ComponentReg);
            clone.SetData(newData);
            clone.SetConfig(newConfig);
            clone.SetComposite(newComposite);
            return clone;
        }

        // (( INTERFACE )) : IVisitable -------- ))
        public void Accept(IVisitor<Cell2D> visitor)
        {
            visitor.Visit(this);
        }

        // (( GETTERS )) -------- ))
        public bool IsEnabled() => _enabled;
        public float GetMinDimension() => Mathf.Min(Data.Dimensions.x, Data.Dimensions.y);
        public void GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal)
        {
            position = Data.Position;
            dimensions = Data.Dimensions;
            normal = Data.Normal;
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
        {
            return _componentRegistry.GetComponent<TComponent>();
        }
        public Component GetComponentByTypeKey(ComponentTypeKey typeKey) => _componentRegistry.GetComponent(typeKey);

        // (( SETTERS )) -------- ))
        protected void SetData(SerializedData data) => _data = data;
        protected void SetConfig(SettingsConfig config) => _config = config;
        protected void SetComposite(InternalComponentRegistry composite) => _componentRegistry = composite;
        protected void SetEnabled(bool enabled) => _enabled = enabled;
    }
}
