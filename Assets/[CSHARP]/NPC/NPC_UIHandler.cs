using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class NPC_UIHandler : OverlapGrid2D
{
    private Dictionary<Vector2Int, GameObject> dialogueBubbles = new Dictionary<Vector2Int, GameObject>();
    public GameObject dialogueBubblePrefab;
    public List<Vector2Int> availablePositionKeys = new List<Vector2Int>();


    public override void Awake()
    {
        base.Awake(); // Activate the OverlapGrid2D

    }

    public override void Update()
    {
        base.Update();

        if (base.grid2D == null) return;

        availablePositionKeys.Clear();

        // Loop through the overlapped colliders and store the coordinates with zero colliders
        Dictionary<int, List<Coordinate>> coordinateDict = GetCoordinatesByColliderCount();
        if (coordinateDict == null || coordinateDict.Count == 0) return;

        List<Coordinate> coordinatesWithZeroColliders = coordinateDict[0];
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

    public override void Reset()
    {
        base.Reset();

        // Remove dialogue bubbles that are no longer in the available position keys
        foreach (Vector2Int key in dialogueBubbles.Keys)
        {
            if (Application.isPlaying)
            {
                Destroy(dialogueBubbles[key]);
            }
            else
            {
                DestroyImmediate(dialogueBubbles[key]);
            }
        }
        dialogueBubbles.Clear();
    }

    public void SpawnDialogueBubble(Vector2Int positionKey)
    {
        if (dialogueBubbles.ContainsKey(positionKey)) return;

        Vector3 worldPosition = grid2D.GetCoordinatePositionInWorldSpace(positionKey);
        GameObject dialogueBubble = Instantiate(dialogueBubblePrefab, worldPosition, Quaternion.identity);
        dialogueBubble.transform.SetParent(transform);
        dialogueBubble.transform.localScale = Vector3.one * grid2D.coordinateSize;
        dialogueBubbles.Add(positionKey, dialogueBubble);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPC_UIHandler))]
public class NPC_UIHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NPC_UIHandler npcUIHandler = (NPC_UIHandler)target;

        if (GUILayout.Button("Spawn Dialogue Bubble"))
        {
            if (npcUIHandler.availablePositionKeys.Count > 0)
            {
                int randomIndex = Random.Range(0, npcUIHandler.availablePositionKeys.Count);
                npcUIHandler.SpawnDialogueBubble(npcUIHandler.availablePositionKeys[randomIndex]);
            }
        }

        if (GUILayout.Button("Reset"))
        {
            npcUIHandler.Reset();
        }
    }

    private void OnSceneGUI()
    {
        DisplayOverlapGrid2D((NPC_UIHandler)target);
    }
}

#endif


