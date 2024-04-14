using System;
using Darklight.Game.Grid2D;
using UnityEngine;

public interface IGrid2DData
{
    /// <summary>
    /// This is to be used to cycle the data between multiple states. This is useful for debugging and testing.
    /// </summary>
    void CycleDataState();

    /// <summary>
    /// The main update method for the data. This method should be called by a MonoBehaviour script to update the data.
    /// </summary>
    void UpdateData(Grid2D grid2D);

    void ClearData();
}


/// <summary>
/// Definition of the Grid2DData class. This class is used by the Grid2D class to store the data for each grid cell.
/// </summary>
public class Grid2DData : IGrid2DData
{
    /// <summary>
    /// The position key set by the related Grid2D class.
    /// </summary>
    public Vector2Int positionKey { get; private set; }

    /// <summary>
    /// Whether or not the data should be used.
    /// </summary>
    public bool disabled { get; private set; }

    /// <summary>
    /// The assigned weight value. This value is used to determine spawn rates and other values.
    /// </summary>
    public int weight { get; private set; }
    const int MIN_WEIGHT = 0;
    const int MAX_WEIGHT = 3;

    /// <summary>
    /// Where the world position of each Grid2DCoordinate is located in the scene.
    /// </summary>
    public Vector3 worldPosition { get; private set; }

    /// <summary>
    /// The size multiplier of the coordinate in the scene.
    /// </summary>
    public float coordinateSize { get; private set; }

    /// <summary>
    /// Event to notify listeners of a data state change.
    /// </summary>
    public event Action<Grid2DData> OnDataStateChanged;

    public Grid2DData(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
    {
        this.positionKey = positionKey;
        this.disabled = disabled;
        this.weight = weight;
        this.worldPosition = worldPosition;
        this.coordinateSize = coordinateSize;
    }

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

    public virtual void UpdateData(Grid2D grid2D)
    {
        this.worldPosition = grid2D.GetWorldSpacePosition(positionKey);
        this.coordinateSize = grid2D.preset.coordinateSize;
    }

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
public class Serialized_Grid2DData : Grid2DData
{
    [SerializeField] private Vector2Int _positionKey;
    [SerializeField] private bool _disabled;
    [SerializeField] private int _weight;

    public Serialized_Grid2DData(Grid2DData data) : base(data.positionKey, data.disabled, data.weight, data.worldPosition, data.coordinateSize)
    {
        this._positionKey = data.positionKey;
        this._disabled = data.disabled;
        this._weight = data.weight;
    }

    public Serialized_Grid2DData(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
        : base(positionKey, disabled, weight, worldPosition, coordinateSize)
    {
        this._positionKey = positionKey;
        this._disabled = disabled;
        this._weight = weight;
    }

    public override void UpdateData(Grid2D grid2D)
    {
        base.UpdateData(grid2D);
        this._positionKey = positionKey;
        this._disabled = disabled;
        this._weight = weight;
    }

    public Grid2DData ToGrid2DData()
    {
        return new Grid2DData(_positionKey, _disabled, _weight, worldPosition, coordinateSize);
    }
}

/// <summary>
/// Create and stores the data from a Physics2D.OverlapBoxAll call at the world position of the Grid2DData. 
/// </summary>
public class Overlap_Grid2DData : Grid2DData
{
    public Overlap_Grid2DData(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize)
        : base(positionKey, disabled, weight, worldPosition, coordinateSize) { }

    public LayerMask layerMask; // The layer mask to use for the OverlapBoxAll called
    public Collider2D[] colliders = new Collider2D[0]; /// The colliders found by the OverlapBoxAll call

    public override void CycleDataState()
    {
        base.CycleDataState();
    }

    public override void UpdateData(Grid2D grid2D)
    {
        base.UpdateData(grid2D);

        // Update the collider data
        this.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * coordinateSize, 0, layerMask);
    }
}
