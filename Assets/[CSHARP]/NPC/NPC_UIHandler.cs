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
    [System.Serializable]
    public class DialogueSpriteOverride
    {
        public Vector2Int positionKey;
        public Sprite bubbleSprite;
        public DialogueSpriteOverride(Vector2Int positionKey, Sprite bubbleSprite)
        {
            this.positionKey = positionKey;
            this.bubbleSprite = bubbleSprite;
        }
    }

    [SerializeField] private List<DialogueSpriteOverride> dialogueBubbleOverrides = new List<DialogueSpriteOverride>();
    public NPC_DialogueBubble activeDialogueBubble { get; private set; } = null;
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
        base.Update(); // Update the grid2D
    }

    public override void Reset()
    {
        base.Reset(); // Reset the grid2D
        if (activeDialogueBubble != null)
        {
            DeleteObject(activeDialogueBubble.gameObject);
        }
    }

    public void NewDialogBubble(string text)
    {
        CreateNewDialogueBubbleAt(new Vector2(0, 0), text);
    }

    NPC_DialogueBubble CreateNewDialogueBubbleAt(Vector2 positionKey, string text)
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
                activeDialogueBubble = bubbleObject.GetComponent<NPC_DialogueBubble>();

                // Set Overrides
                if (dialogueBubbleOverrides != null && dialogueBubbleOverrides.Count > 0)
                {
                    DialogueSpriteOverride dialogueSpriteOverride = GetDialogueSpriteOverride(coordinate.positionKey);
                    if (dialogueSpriteOverride != null)
                    {
                        activeDialogueBubble.settings.inkyLabel = text;
                        activeDialogueBubble.settings.bubbleSprite = dialogueSpriteOverride.bubbleSprite;
                    }
                }
                else
                {
                    activeDialogueBubble.settings = defaultBubbleSettings;
                }
            }
        }

        activeDialogueBubble.ManualUpdate();
        return activeDialogueBubble;
    }

    public DialogueSpriteOverride GetDialogueSpriteOverride(Vector2Int positionKey)
    {
        return dialogueBubbleOverrides.FirstOrDefault(x => x.positionKey == positionKey);
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
[CanEditMultipleObjects]
public class NPC_UIHandlerEditor : OverlapGrid2DEditor
{
    bool showAdvancedSettings = false;
    public override void OnInspectorGUI()
    {

        NPC_UIHandler npcUIHandler = (NPC_UIHandler)target;

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        CustomInspectorGUI.CreateFoldout(ref showAdvancedSettings, "Advanced Settings", () =>
        {

            if (GUILayout.Button("Manual Update"))
            {
                if (npcUIHandler.activeDialogueBubble != null)
                {
                    Debug.LogError("No active dialogue bubble found.");
                    return;
                }
                npcUIHandler.activeDialogueBubble.ManualUpdate();
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
                if (coordinate == null) continue;

                CustomGizmos.DrawLabel($"{vector2Int}", coordinate.worldPosition, CustomGUIStyles.BoldStyle);
                CustomGizmos.DrawButtonHandle(coordinate.worldPosition, npcUIHandler.grid2D.coordinateSize * 0.75f, Vector3.forward, coordinate.color, () =>
                {
                    npcUIHandler.NewDialogBubble("Hello World!");
                }, Handles.RectangleHandleCap);
            }
        }
    }
}

#endif


