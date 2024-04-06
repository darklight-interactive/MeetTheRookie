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
        public GameObject GameObjectInstance { get; private set; }    // reference to the dialogue bubble game object
        public bool Active => active; // is the dialogue bubble active?

        NPC_UIHandler parent; // reference to parent
        Coordinate gridCoordinate;
        [SerializeField] string dialogueText = "Hello, World!";
        [SerializeField] GameObject dialogueBubblePrefab = null;
        [SerializeField] bool active = false;
        public DialogueBubble(NPC_UIHandler parent, Coordinate gridCoordinate, GameObject dialogueBubblePrefab, string dialogueText)
        {
            this.parent = parent;
            this.gridCoordinate = gridCoordinate;
            this.dialogueBubblePrefab = dialogueBubblePrefab;
            this.dialogueText = dialogueText;
        }

        public void Update()
        {
            if (active && GameObjectInstance == null)
            {
                SpawnDialogueBubble();
            }
            else if (!active && GameObjectInstance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(GameObjectInstance);
                }
                else
                {
                    DestroyImmediate(GameObjectInstance);
                }
            }
        }

        public void SetActive(bool active)
        {
            this.active = active;
            Update();
        }

        public void SpawnDialogueBubble()
        {
            Vector3 worldPosition = this.gridCoordinate.worldPosition;
            GameObjectInstance = Instantiate(dialogueBubblePrefab, worldPosition, Quaternion.identity);
            GameObjectInstance.transform.SetParent(parent.transform);
            GameObjectInstance.transform.localScale = Vector3.one * gridCoordinate.parent.coordinateSize;
        }
    }
    #endregion

    private Dictionary<Vector2Int, DialogueBubble> _dict = new Dictionary<Vector2Int, DialogueBubble>();
    public GameObject dialogueBubblePrefab;
    public List<DialogueBubble> availableBubbles = new List<DialogueBubble>();

    public override void Awake()
    {
        this.Reset();
        this.Update();
    }

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
                DialogueBubble dialogueBubble = new DialogueBubble(this, coordinate, dialogueBubblePrefab, "Dialogue Text Here");
                _dict.Add(coordinate.positionKey, dialogueBubble);
            }
        }

        // Set the available dialogue bubbles
        availableBubbles.Clear();
        availableBubbles = _dict.Values.ToList();

        // Loop through the available dialogue bubbles and update them
        foreach (DialogueBubble dialogueBubble in availableBubbles)
        {
            dialogueBubble.Update();
        }
    }

    public override void Reset()
    {
        base.Reset();

        _dict.Clear();
        availableBubbles.Clear();
        foreach (DialogueBubble dialogueBubble in availableBubbles)
        {
            if (Application.isPlaying)
            {
                Destroy(dialogueBubble.GameObjectInstance);
            }
            else
            {
                DestroyImmediate(dialogueBubble.GameObjectInstance);
            }
        }

        this.Update();
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


