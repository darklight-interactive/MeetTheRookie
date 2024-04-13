using Darklight.Game.Grid2D;
using UnityEngine;

public interface IGrid2DData
{
    Grid2D<IGrid2DData> gridParent { get; }
    bool active { get; set; }
    int weight { get; set; }
    Color debugColor { get; }
    Vector2Int positionKey { get; }
    Vector3 worldPosition { get; }
    float coordinateSize { get; }
    void CycleDataState();
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
    public int weight { get; set; }
    public Color debugColor
    {
        get
        {
            Color color = Color.white;
            if (active)
            {
                switch (weight)
                {
                    case 0:
                        color = Color.red;
                        break;
                    case 1:
                        color = Color.yellow;
                        break;
                    case 2:
                        color = Color.green;
                        break;
                }
            }
            else
            {
                color = Color.gray;
            }
            return color;
        }
    }
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

    public OverlapGrid2DData(Grid2D<IGrid2DData> gridParent, Vector2Int positionKey, bool active, int weight, LayerMask layerMask)
    {
        this.gridParent = gridParent;
        this.positionKey = positionKey;
        this.layerMask = layerMask;
        this.active = active;
        this.weight = weight;
    }

    public void CycleDataState()
    {
        if (active)
        {
            if (weight < 2)
            {
                weight++;
            }
            else
            {
                active = false;
                weight = 0;
            }
        }
        else
        {
            active = true;
        }

        // Update the spawn weight map
        gridParent.settings.SetSpawnWeight(positionKey, active, weight);
    }

    public void UpdateData()
    {
        // Update the collider data
        this.colliders = Physics2D.OverlapBoxAll(
            worldPosition, Vector2.one * coordinateSize, 0, layerMask);
    }
}
