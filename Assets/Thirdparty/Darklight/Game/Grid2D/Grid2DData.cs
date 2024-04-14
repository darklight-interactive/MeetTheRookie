using System;
using Darklight.Game.Grid;
using UnityEngine;



/// <summary>
/// Interface for the Grid2DData class. This interface is used to define the methods and properties that the Grid2DData class should have.
/// </summary>
public interface IGrid2DData
{
    /// <summary>
    /// Has the data been initialized?
    /// </summary>
    bool initialized { get; }

    /// <summary>
    /// The position key set by the related Grid2D class.
    /// </summary>
    public Vector2Int positionKey { get; }

    /// <summary>
    /// Whether or not the data should be used.
    /// </summary>
    public bool disabled { get; }

    /// <summary>
    /// The assigned weight value. This value is used to determine spawn rates and other values.
    /// </summary>
    public int weight { get; }

    /// <summary>
    /// The world position of the data.
    /// </summary>
    public Vector3 worldPosition { get; }

    /// <summary>
    /// The size of the coordinate in the grid.
    /// </summary>
    public float coordinateSize { get; }

    /// <summary>
    /// Event to notify listeners of a data state change.
    /// </summary>
    public event Action<Grid2DData> OnDataStateChanged;

    /// <summary>
    /// Initializes the data with the given values.
    /// </summary>
    /// <param name="positionKey"></param>
    /// <param name="disabled"></param>
    /// <param name="weight"></param>
    /// <param name="worldPosition"></param>
    /// <param name="coordinateSize"></param>
    public abstract void Initialize(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize);

    /// <summary>
    /// This is to be used to cycle the data between multiple states. This is useful for debugging and testing.
    /// </summary>
    public abstract void CycleDataState();

    /// <summary>
    /// The main update method for the data. This method should be called by a MonoBehaviour script to update the data.
    /// </summary>
    public abstract void UpdateData();

    /// <summary>
    /// Clears the data and any events that are attached to it.
    /// </summary>
    public abstract void ClearData();
}

// -------------------------------------------------------------------------------- >>

/// <summary>
/// Definition of the Grid2DData class. This class is used by the Grid2D class to store the data for each grid cell.
/// </summary>
public class Grid2DData : IGrid2DData
{
    public bool initialized { get; protected set; } = false;
    public Vector2Int positionKey { get; protected set; }
    public bool disabled { get; protected set; }

    public int weight { get; protected set; }
    public Vector3 worldPosition { get; protected set; }
    public float coordinateSize { get; protected set; }

    /// <summary>
    /// Event to notify listeners of a data state change.
    /// </summary>
    public event Action<Grid2DData> OnDataStateChanged;

    public Grid2DData()
    {
        this.positionKey = Vector2Int.zero;
        this.disabled = false;
        this.weight = 0;
        this.worldPosition = Vector3.zero;
        this.coordinateSize = 1;
    }

    public Grid2DData(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
    {
        Initialize(positionKey, disabled, weight, worldPosition, coordinateSize);
    }

    // ------------------------------------------------------------------------------- >>


    public virtual void Initialize(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
    {
        this.positionKey = positionKey;
        this.disabled = disabled;
        this.weight = weight;
        this.worldPosition = worldPosition;
        this.coordinateSize = coordinateSize;

        initialized = true;
    }


    const int MIN_WEIGHT = 0;
    const int MAX_WEIGHT = 3;
    public virtual void CycleDataState()
    {
        // << CYCLE THE WEIGHT VALUE >>
        if (weight < MAX_WEIGHT)
        {
            weight++;
            disabled = false;
        }
        else
        {
            weight = MIN_WEIGHT;
            disabled = true;
        }

        // Notify listeners that the data state has changed
        OnDataStateChanged?.Invoke(this);
    }

    public virtual void UpdateData() { }

    public virtual void ClearData()
    {
        // Clear the event
        OnDataStateChanged = null;
    }


    /// <summary>
    /// Returns a color based on the data properties.
    /// </summary>
    /// <returns></returns>
    public virtual Color GetColor()
    {
        Color color = Color.black;
        if (disabled) return color;

        switch (weight)
        {
            case 0:
                color = Color.white;
                break;
            case 1:
                color = Color.red;
                break;
            case 2:
                color = Color.yellow;
                break;
            case 3:
                color = Color.green;
                break;
        }

        return color;
    }
}

[System.Serializable]
/// <summary>
/// Serialized version of the Grid2DData class. This class is used to store the data in a serialized format.
/// </summary>
public class Serialized_Grid2DData : Grid2DData, IGrid2DData
{
    [SerializeField] private Vector2Int _positionKey;
    [SerializeField] private bool _disabled;
    [SerializeField] private int _weight;

    public override void Initialize(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
    {
        base.Initialize(positionKey, disabled, weight, worldPosition, coordinateSize);
        this._positionKey = positionKey;
        this._disabled = disabled;
        this._weight = weight;
    }

    /// <summary>
    /// Converts the Serialized_Grid2DData to a Grid2DData object from the stored data.
    /// </summary>
    /// <returns></returns>
    public Grid2DData ToGrid2DData()
    {
        Grid2DData grid2DData = new Grid2DData();
        grid2DData.Initialize(_positionKey, _disabled, _weight, worldPosition, coordinateSize);
        return grid2DData;
    }
}


