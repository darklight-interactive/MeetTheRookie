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
        // ======== [[ SERIALIZED FIELDS ]] ======================================================= >>>>
        [SerializeField, ShowOnly] string _name = "Cell2D";
        [SerializeField] SettingsConfig _config;
        [SerializeField] SerializedData _data;
        [SerializeField] ComponentRegistry _componentReg;

        // ======== [[ PROPERTIES ]] ======================================================= >>>>
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

            // Set the name
            _name = $"Cell2D ({key.x},{key.y})";
        }

        public void Update()
        {
            if (_data == null) return;
            if (_componentReg == null) return;
        }

        // -- (( HANDLERS )) -------- )))
        public void RecalculateDataFromGrid(Grid2D grid)
        {
            if (_data == null) return;
            if (_componentReg == null) return;

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
        public float GetMinDimension() => Mathf.Min(Data.Dimensions.x, Data.Dimensions.y);
        public void GetTransformData(out Vector3 position, out Vector2 dimensions, out Vector3 normal)
        {
            position = Data.Position;
            dimensions = Data.Dimensions;
            normal = Data.Normal;
        }

        public void GetTransformData(out Vector3 position, out float size, out Vector3 normal)
        {
            position = Data.Position;
            size = GetMinDimension();
            normal = Data.Normal;
        }

        // (( SETTERS )) -------- ))
        protected void SetData(SerializedData data) => _data = data;
        protected void SetConfig(SettingsConfig config) => _config = config;
        protected void SetComposite(ComponentRegistry composite) => _componentReg = composite;


        // (( GIZMOS )) -------- ))
        public void DrawDefaultGizmos()
        {
            if (_data == null) return;

            GetTransformData(out Vector3 position, out float radius, out Vector3 normal);
            CustomGizmos.DrawWireSquare(position, radius, normal, Color.gray);
        }

        public void DrawEditor()
        {
            if (_data == null) return;
        }
    }
}
