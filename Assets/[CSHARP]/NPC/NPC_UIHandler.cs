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

public class NPC_UIHandler : Inky_Interaction
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

    [SerializeField] private List<DialogueSpriteOverride> dialogueBubbleOverrides;
    public NPC_DialogueBubble activeDialogueBubble { get; private set; } = null;

    public GameObject dialogueBubblePrefab;
    public NPC_DialogueBubble.Settings defaultBubbleSettings;

    public OverlapGrid2D overlapGrid => GetComponent<OverlapGrid2D>();

    public override void StartInteractionKnot(Inky_StoryManager.KnotComplete onComplete)
    {
        base.StartInteractionKnot(() =>
        {
            onComplete?.Invoke();
        });

        CreateNewDialogueBubbleAt(new Vector2(0, 1), Inky_StoryManager.Instance.currentText);
    }
    public override void ResetInteraction()
    {
        if (activeDialogueBubble != null) DeleteObject(activeDialogueBubble.gameObject);
    }

    public void CreateInteractionUIBubble(string text)
    {
        // Get the coordinate with zero colliders
        Dictionary<int, List<Coordinate>> coordinateDict = overlapGrid.GetCoordinatesByColliderCount();
        if (coordinateDict == null || coordinateDict.Count == 0 || !coordinateDict.ContainsKey(0))
        {
            Debug.LogError("No coordinates with zero colliders found.");
            return;
        }

        List<Coordinate> zeroColliderCoordinates = coordinateDict[0];

        UXML_InteractionUI interactionUI = ISceneSingleton<UXML_InteractionUI>.Instance;

        if (interactionUI != null)
            interactionUI.DisplayDialogueBubble(zeroColliderCoordinates[0].worldPosition, text);
        else
            Debug.LogError("No interaction UI found.");
    }


    NPC_DialogueBubble CreateNewDialogueBubbleAt(Vector2 positionKey, string text)
    {
        // Delete the active dialogue bubble
        if (activeDialogueBubble != null) DeleteObject(activeDialogueBubble.gameObject);

        // Get the coordinate with zero colliders
        Dictionary<int, List<Coordinate>> coordinateDict = overlapGrid.GetCoordinatesByColliderCount();
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
                //bubbleObject.transform.localScale = Vector3.one * coordinate.parent.coordinateSize;

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

        activeDialogueBubble.ManualUpdate(text);
        return activeDialogueBubble;
    }

    public void Update()
    {
        if (activeDialogueBubble == null) return;
        NPC_DialogueBubble.Settings settings = activeDialogueBubble.settings;
        string currStoryText = Inky_StoryManager.Instance.currentText;

        if (settings.inkyLabel == currStoryText) return;
        activeDialogueBubble.ManualUpdate(currStoryText);
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



