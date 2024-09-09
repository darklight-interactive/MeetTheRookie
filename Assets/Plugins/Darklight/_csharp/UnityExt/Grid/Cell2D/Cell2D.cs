using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Behaviour.Interface;
using Darklight.UnityExt.Editor;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
namespace Darklight.UnityExt.Game.Grid
{

    [System.Serializable]
    public partial class Cell2D : IVisitable<Cell2D>
    {
        // ======== [[ FIELDS ]] ======================================================= >>>>
        ComponentRegistry _componentReg;
        List<string> _componentDebugLabels = new List<string>();

        bool _enabled;
        bool _initialized;

        [SerializeField, ShowOnly] string _name = "Cell2D";
        [SerializeField] SettingsConfig _config;
        [SerializeField] SerializedData _data;
        [ShowOnly, NonReorderable] public ComponentTypeKey[] componentTypeKeys;

        // ======== [[ PROPERTIES ]] ======================================================= >>>>
        public string Name { get => _name; }
        public Vector2Int Key { get => _data.Key; }
        public SettingsConfig Config { get => _config; }
        public SerializedData Data { get => _data; }
        public ComponentRegistry ComponentReg { get => _componentReg; }

        // ======== [[ CONSTRUCTORS ]] ======================================================= >>>>
        public Cell2D(Vector2Int key) => Initialize(key, null);
        public Cell2D(Vector2Int key, SettingsConfig config) => Initialize(key, config);

        // ======== [[ METHODS ]] ============================================================ >>>>
        // -- (( RUNTIME )) -------- )))
        public void Initialize(Vector2Int key, SettingsConfig config)
        {
            // Initialize the configuration
            if (config == null)
                config = new SettingsConfig();
            _config = config;

            // Create the data
            _data = new SerializedData(key);

            // Create the composite
            _componentReg = new ComponentRegistry(this);
            componentTypeKeys = _componentReg.GetComponentTypeKeys();

            // Set the name
            _name = $"Cell2D ({key.x},{key.y})";

            // << SET INITIALIZED >>
            if (_config == null || _data == null || _componentReg == null)
            {
                _initialized = false;
                return;
            }
            _initialized = true;
        }

        public void Update()
        {
            if (!_initialized) return;
            componentTypeKeys = _componentReg.GetComponentTypeKeys();
        }

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
            ComponentRegistry newComposite = new ComponentRegistry(ComponentReg);
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

        // (( SETTERS )) -------- ))
        protected void SetData(SerializedData data) => _data = data;
        protected void SetConfig(SettingsConfig config) => _config = config;
        protected void SetComposite(ComponentRegistry composite) => _componentReg = composite;
        protected void SetEnabled(bool enabled) => _enabled = enabled;

        // (( GIZMOS )) -------- ))
        public void DrawGizmos()
        {
            if (_data == null) return;

            GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal);

            Color faintWhite = new Color(1, 1, 1, 0.5f);
            CustomGizmos.DrawWireRect(position, dimensions, normal, faintWhite);
        }

        public void DrawEditorGizmos()
        {
            if (_data == null) return;
        }
    }
}
