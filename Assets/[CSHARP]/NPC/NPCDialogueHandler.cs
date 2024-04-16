using Darklight.Game.Grid;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(InkyInteractable))]
public class NPCDialogueHandler : OverlapGrid2D
{
    private InkyInteractable inkyInteractable => GetComponent<InkyInteractable>();
    private Queue<UXML_WorldSpaceElement> dialogueBubbles = new Queue<UXML_WorldSpaceElement>();
    [SerializeField] private List<UXML_WorldSpaceElement> dialogueBubblesList = new List<UXML_WorldSpaceElement>();

    public void Start()
    {
        // >> ON INTERACTION -------------------------------------
        inkyInteractable.OnInteraction += () =>
        {
            // Dequeue the top bubble and destroy it
            if (dialogueBubbles.Count > 0)
            {
                UXML_WorldSpaceElement topBubble = dialogueBubbles.Dequeue();
                dialogueBubblesList.RemoveAt(0);
                Destroy(topBubble.gameObject);
            }

            // Create a new bubble with the current text
            UXML_WorldSpaceElement dialogueBubble = CreateDialogueBubble(inkyInteractable.knotIterator.currentText);
            dialogueBubbles.Enqueue(dialogueBubble);
            dialogueBubblesList.Add(dialogueBubble);
        };

        // >> ON COMPLETED -------------------------------------
        inkyInteractable.OnCompleted += () =>
        {
            foreach (UXML_WorldSpaceElement bubble in dialogueBubblesList)
            {
                Destroy(bubble.gameObject);
            }
            dialogueBubbles.Clear();
            Debug.Log("NPCDialogueHandler: Interaction Completed");
        };
    }

    public UXML_WorldSpaceElement CreateDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;
        UXML_WorldSpaceElement element = UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(position);
        element.transform.SetParent(this.transform);
        element.transform.localScale = data.coordinateSize * Vector3.one;
        element.SetText(text);
        return element;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPCDialogueHandler), true)]
public class NPCDialogueHandlerEditor : OverlapGrid2DEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif

