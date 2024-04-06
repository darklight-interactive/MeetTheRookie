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

    #region [[ DialogueBubble Class ]] =============================== >>
    [System.Serializable]
    public class DialogueBubble
    {
        public bool Active
        {
            // is the dialogue bubble active?
            get => active;
            set
            {
                active = value;
            }
        }

        NPC_UIHandler parent; // reference to parent
        public DialogueBubbleHandler dialogueBubbleHandler { get; private set; } // reference to the dialogue bubble handler
        public Coordinate gridCoordinate { get; private set; } // reference to the grid coordinate;
        public GameObject dialogueBubbleObject { get; private set; }    // reference to the dialogue bubble game object
        [SerializeField] string dialogueText = "Hello, World!";
        public Sprite bubbleSprite;
        [SerializeField] bool active = false;
        public DialogueBubble(NPC_UIHandler parent, Coordinate gridCoordinate, string dialogueText)
        {
            this.parent = parent;
            this.gridCoordinate = gridCoordinate;
            this.dialogueText = dialogueText;
        }

        public void Update()
        {
            if (active && dialogueBubbleObject == null)
            {
                parent.DeactivateOtherBubbles(this);
                SpawnDialogueBubble();
            }
            else if (!active && dialogueBubbleObject != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(dialogueBubbleObject);
                }
                else
                {
                    DestroyImmediate(dialogueBubbleObject);
                }
            }
        }

        public void SpawnDialogueBubble()
        {
            if (dialogueBubbleObject != null) return;

            // Create the dialogue bubble game object
            Vector3 worldPosition = this.gridCoordinate.worldPosition;
            dialogueBubbleObject = Instantiate(parent.dialogueBubblePrefab, worldPosition, Quaternion.identity);
            dialogueBubbleObject.transform.SetParent(parent.transform);
            dialogueBubbleObject.transform.localScale = Vector3.one * gridCoordinate.parent.coordinateSize;

            // Initialize the dialogue bubble handler
            this.dialogueBubbleHandler = dialogueBubbleObject.GetComponent<DialogueBubbleHandler>();
            dialogueBubbleHandler.dialogueText = dialogueText;
            dialogueBubbleHandler.bubbleSprite = bubbleSprite;

            dialogueBubbleHandler.Awake();
            dialogueBubbleHandler.Update();
        }
    }
    #endregion

    [SerializeField] private Dictionary<Vector2Int, DialogueBubble> _dict = new Dictionary<Vector2Int, DialogueBubble>();
    public GameObject dialogueBubblePrefab;
    public List<DialogueBubble> availableBubbles = new List<DialogueBubble>();

    public override void Update()
    {
        base.Update(); // Get the overlapped colliders & available coordinates

        // Loop through the coordinates with zero colliders and spawn dialogue bubbles
        Dictionary<int, List<Coordinate>> coordinateDict = GetCoordinatesByColliderCount();
        if (coordinateDict == null || coordinateDict.Count == 0 || !coordinateDict.ContainsKey(0)) return;

        List<Coordinate> coordinatesWithZeroColliders = coordinateDict[0]; // << the key 0 represents zero colliders
        foreach (Coordinate coordinate in coordinatesWithZeroColliders)
        {
            // If the dialogue bubble does not exist, create it
            if (!_dict.ContainsKey(coordinate.positionKey))
            {
                DialogueBubble dialogueBubble = new DialogueBubble(this, coordinate, "Dialogue Text Here");
                _dict.Add(coordinate.positionKey, dialogueBubble);
            }
            else
            {
                _dict[coordinate.positionKey].Update();
            }
        }

        // Set the available dialogue bubbles
        availableBubbles.Clear();
        availableBubbles = _dict.Values.ToList();
    }

    public void DeactivateOtherBubbles(DialogueBubble activeBubble)
    {
        foreach (DialogueBubble dialogueBubble in _dict.Values)
        {
            if (dialogueBubble != activeBubble)
                dialogueBubble.Active = false;
        }
    }

    public override void Reset()
    {
        base.Reset();
        availableBubbles.Clear();

        foreach (DialogueBubble dialogueBubble in _dict.Values)
        {
            dialogueBubble.Active = false; // deactivate the dialogue bubble
        }
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

        if (GUILayout.Button("Update"))
        {
            npcUIHandler.Update();
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


