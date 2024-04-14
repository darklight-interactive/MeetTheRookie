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
    void UpdateData();
}


/// <summary>
/// Definition of the Grid2DData class. This class is used by the Grid2D class to store the data for each grid cell.
/// </summary>
public abstract class Grid2DData : IGrid2DData
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

    /// <summary>
    /// Where the world position of each Grid2DCoordinate is located in the scene.
    /// </summary>
    public Vector3 worldPosition { get; private set; }

    /// <summary>
    /// The size multiplier of the coordinate in the scene.
    /// </summary>
    public float coordinateSize { get; private set; }

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
        if (weight < 3)
        {
            weight++;
            disabled = true;
        }
        else
        {
            weight = 0;
            disabled = false;
        }
    }

    public abstract void UpdateData();
}

/// <summary>
/// OverlapData class to store the collider data and manage the overlap box data.
/// </summary>
public class OverlapGrid2DData : Grid2DData
{
    public LayerMask layerMask;
    public Collider2D[] colliders = new Collider2D[0];
    public int colliderCount => colliders.Length;

    public OverlapGrid2DData(Vector2Int positionKey, bool disabled, int weight, Vector3 worldPosition, float coordinateSize, LayerMask layerMask) : base(positionKey, disabled, weight, worldPosition, coordinateSize)
    {
        this.layerMask = layerMask;
    }

    public override void CycleDataState()
    {
        base.CycleDataState();
    }

    public override void UpdateData()
    {
        // Update the collider data
        this.colliders = Physics2D.OverlapBoxAll(worldPosition, Vector2.one * coordinateSize, 0, layerMask);
    }
}
