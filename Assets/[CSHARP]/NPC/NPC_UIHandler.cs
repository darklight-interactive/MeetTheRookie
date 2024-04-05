using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;


[ExecuteAlways]
public class NPC_UIHandler : OverlapGrid2D
{
    private Dictionary<Vector2Int, GameObject> dialogueBubbles = new Dictionary<Vector2Int, GameObject>();
    public GameObject dialogueBubblePrefab;
    public List<Vector2Int> availablePositionKeys = new List<Vector2Int>();


    public override void Awake()
    {
        base.Awake();

    }

    public override void Update()
    {
        base.Update();

        if (base.grid2D == null) return;

        availablePositionKeys.Clear();
        List<Coordinate> coordinatesWithZeroColliders = GetCoordinatesByColliderCount()[0];
        foreach (Coordinate coordinate in coordinatesWithZeroColliders)
        {
            if (!availablePositionKeys.Contains(coordinate.positionKey))
            {
                availablePositionKeys.Add(coordinate.positionKey);
            }
            else
            {
                if (coordinate.typeValue.Length > 0)
                {
                    availablePositionKeys.Remove(coordinate.positionKey);
                }
            }
        }
    }
}
