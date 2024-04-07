using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using static Darklight.Game.Grid2D.Grid2D<UnityEngine.Collider2D[]>;
using System.Linq;
using Darklight.UnityExt;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class NPC_UIHandler : OverlapGrid2D
{
    private NPC_DialogueBubble activeDialogueBubble;
    public GameObject dialogueBubblePrefab;
    public NPC_DialogueBubble.Settings defaultBubbleSettings;

    public override void Awake()
    {
        base.Awake(); // Initialize the grid2D
        if (activeDialogueBubble != null)
        {
            DeleteObject(activeDialogueBubble.gameObject);
        }
    }

    public override void Update()
    {
        base.Update(); // Get the overlapped colliders & available coordinates

        if (activeDialogueBubble != null)
        {
            activeDialogueBubble.settings = defaultBubbleSettings; // << TODO: get the current ink value
            activeDialogueBubble.Update(); // Update the dialogue bubble
        }
    }

    public override void Reset()
    {
        base.Reset(); // Reset the grid2D
        if (activeDialogueBubble != null)
        {
            DeleteObject(activeDialogueBubble.gameObject);
        }
    }

    public NPC_DialogueBubble CreateNewDialogueBubbleAt(Vector2 positionKey)
    {
        // Delete the active dialogue bubble
        if (activeDialogueBubble != null) DeleteObject(activeDialogueBubble.gameObject);

        // Get the coordinate with zero colliders
        Dictionary<int, List<Coordinate>> coordinateDict = GetCoordinatesByColliderCount();
        if (coordinateDict == null || coordinateDict.Count == 0 || !coordinateDict.ContainsKey(0))
        {
            Debug.LogError("No coordinates with zero colliders found.");
            return null;
        }

        // Get the coordinate with zero colliders with the given position key
        foreach (Coordinate coordinate in coordinateDict[0])
        {
            if (coordinate.positionKey == positionKey)
            {
                // Create the dialogue bubble game object
                Vector3 worldPosition = coordinate.worldPosition;
                GameObject bubbleObject = Instantiate(dialogueBubblePrefab, worldPosition, Quaternion.identity);
                bubbleObject.transform.SetParent(transform);
                bubbleObject.transform.localScale = Vector3.one * coordinate.parent.coordinateSize;

                // Get the dialogue bubble component
                activeDialogueBubble = bubbleObject.GetComponent<NPC_DialogueBubble>(); ;
                activeDialogueBubble.settings = defaultBubbleSettings; // << get the default settings from the inspector
            }
        }

        return activeDialogueBubble;
    }

    public void DeleteObject(GameObject obj)
    {
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(NPC_UIHandler))]
public class NPC_UIHandlerEditor : OverlapGrid2DEditor
{
    bool showAdvancedSettings = false;
    public override void OnInspectorGUI()
    {

        NPC_UIHandler npcUIHandler = (NPC_UIHandler)target;

        EditorGUI.BeginChangeCheck();

        CustomInspectorGUI.CreateFoldout(ref showAdvancedSettings, "Advanced Settings", () =>
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update"))
            {
                npcUIHandler.Update();
            }

            if (GUILayout.Button("Reset"))
            {
                npcUIHandler.Reset();
            }
        });

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(npcUIHandler);
            npcUIHandler.Update();
        }
    }

    private void OnSceneGUI()
    {
        NPC_UIHandler npcUIHandler = (NPC_UIHandler)target;
        List<Vector2Int> positionKeys = npcUIHandler.grid2D.GetPositionKeys();
        if (positionKeys != null && positionKeys.Count > 0)
        {
            foreach (Vector2Int vector2Int in positionKeys)
            {
                Coordinate coordinate = npcUIHandler.grid2D.GetCoordinate(vector2Int);

                CustomGizmos.DrawLabel($"{vector2Int}", coordinate.worldPosition, CustomGUIStyles.BoldStyle);
                CustomGizmos.DrawButtonHandle(coordinate.worldPosition, npcUIHandler.grid2D.coordinateSize * 0.75f, Vector3.forward, coordinate.color, () =>
                {
                    NPC_DialogueBubble dialogueBubble = npcUIHandler.CreateNewDialogueBubbleAt(vector2Int);
                }, Handles.RectangleHandleCap);
            }
        }
    }
}

#endif


