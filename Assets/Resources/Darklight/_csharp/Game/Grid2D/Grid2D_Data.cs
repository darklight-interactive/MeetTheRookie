using System;
using Darklight.Game.Grid;
using UnityEngine;

#region IGrid2D_Data : Interface

/// <summary>
/// Interface for the Grid2DData class. This interface is used to define the methods and properties that the Grid2DData class should have.
/// </summary>
public interface IGrid2D_Data
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
    public Vector3 worldPosition { get; set; }

    /// <summary>
    /// The size of the coordinate in the grid.
    /// </summary>
    public float coordinateSize { get; }

    /// <summary>
    /// Event to notify listeners of a data state change.
    /// </summary>
    public event Action<Grid2D_Data> OnDataStateChanged;

    public void Initialize(Grid2D_Data data);

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

#endregion

// -------------------------------------------------------------------------------- >>

#region Grid2D_Data : Class
/// <summary>
/// Definition of the Grid2DData class. This class is used by the Grid2D class to store the data for each grid cell.
/// </summary>
public class Grid2D_Data : IGrid2D_Data
{
    public bool initialized { get; private set; }
    public Vector2Int positionKey { get; private set; }
    public bool disabled { get; protected set; }
    public int weight { get; protected set; }
    public Vector3 worldPosition { get; set; }
    public float coordinateSize { get; private set; }

    /// <summary>
    /// Event to notify listeners of a data state change.
    /// </summary>
    public event Action<Grid2D_Data> OnDataStateChanged;

    public Grid2D_Data()
    {
        this.positionKey = Vector2Int.zero;
        this.disabled = false;
        this.weight = 0;
        this.worldPosition = Vector3.zero;
        this.coordinateSize = 1;
    }

    public Grid2D_Data(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
    {
        Initialize(positionKey, disabled, weight, worldPosition, coordinateSize);
    }

    // ------------------------------------------------------------------------------- >>

    public virtual void Initialize(Grid2D_Data data)
    {
        Initialize(data.positionKey, data.disabled, data.weight, data.worldPosition, data.coordinateSize);
    }

    public virtual void Initialize(Grid2D_SerializedData serializedData, Vector3 worldPosition, float coordinateSize)
    {
        Initialize(serializedData.ToData()); // Load the data from the serialized data object
        this.worldPosition = worldPosition; // Set the world position
        this.coordinateSize = coordinateSize; // Set the coordinate size
    }

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

        Debug.Log("Data state changed: " + weight);

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
#endregion

/// <summary>
/// Create and stores the data from a Physics2D.OverlapBoxAll call at the world position of the Grid2DData. 
/// </summary>
public class OverlapGrid2D_Data : Grid2D_Data
{
    private bool disabledInitially = false;
    public LayerMask layerMask; // The layer mask to use for the OverlapBoxAll called
    public Collider2D[] colliders = new Collider2D[0]; /// The colliders found by the OverlapBoxAll call

    public OverlapGrid2D_Data() { }
    // Initialization method to set properties

    public void Initialize(Vector2Int positionKey, Vector3 worldPosition, float coordinateSize, LayerMask layerMask)
    {
        base.Initialize(positionKey, disabled, weight, worldPosition, coordinateSize);
        this.layerMask = layerMask;
        this.disabledInitially = disabled; // << set equal to initial value
    }

    public override void CycleDataState()
    {
        base.CycleDataState();
        this.disabledInitially = disabled; // << set to match the new state
    }

    public override void UpdateData()
    {
        // Update the collider data
        this.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * coordinateSize, 0, layerMask);
        if (!disabledInitially)
        {
            this.disabled = colliders.Length > 0;
        }
    }
}


