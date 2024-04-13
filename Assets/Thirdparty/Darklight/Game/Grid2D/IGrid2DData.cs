using Darklight.Game.Grid2D;
using UnityEngine;

public interface IGrid2DData
{
    Grid2D<IGrid2DData> gridParent { get; }
    bool active { get; }
    Color activeColor { get; }
    Vector2Int positionKey { get; }
    Vector3 worldPosition { get; }
    float coordinateSize { get; }

    void Initialize(Grid2D<IGrid2DData> gridParent, Vector2Int positionKey);
    void SetActive(bool active);
    void UpdateData();
}

/// <summary>
/// OverlapData class to store the collider data and manage the overlap box data.
/// </summary>
[System.Serializable]
public class OverlapGrid2DData : IGrid2DData
{
    #region [[ IGrid2DData Properties ]] =============================== >>
    public Grid2D<IGrid2DData> gridParent { get; set; }
    public Vector2Int positionKey { get; set; }
    public bool active { get; set; }
    public Color activeColor => active ? Color.green : Color.black;
    public Vector3 worldPosition
    {
        get
        {
            Vector3 worldPosition = gridParent.GetWorldSpacePosition(positionKey);
            return worldPosition;
        }
    }
    public float coordinateSize => gridParent.settings.coordinateSize;
    public string label
    {
        get
        {
            string outString = "";
            outString += $"active >> {active}";
            outString += $"\npos >> {positionKey}";
            outString += $"\ncolliders >> {colliderCount}";
            return outString;
        }
    }
    #endregion


    private bool forceDisable = false;
    public LayerMask layerMask;

    public Collider2D[] colliders = new Collider2D[0];
    public int colliderCount => colliders.Length;

    public OverlapGrid2DData(Grid2D<IGrid2DData> gridParent, Vector2Int positionKey, LayerMask layerMask)
    {
        this.gridParent = gridParent;
        this.positionKey = positionKey;
        this.layerMask = layerMask;
        Initialize(gridParent, positionKey);
    }
    public void Initialize(Grid2D<IGrid2DData> gridParent, Vector2Int positionKey)
    {
        this.gridParent = gridParent;
        this.positionKey = positionKey;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

    public void UpdateData()
    {
        // Update the collider data
        this.colliders = Physics2D.OverlapBoxAll(
            worldPosition, Vector2.one * coordinateSize, 0, layerMask);
    }
}
