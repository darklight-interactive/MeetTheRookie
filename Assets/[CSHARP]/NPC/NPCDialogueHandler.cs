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
    private Queue<UXML_WorldSpaceUI> dialogueBubbles = new Queue<UXML_WorldSpaceUI>();
    [SerializeField] private List<UXML_WorldSpaceUI> dialogueBubblesList = new List<UXML_WorldSpaceUI>();

    public void Start()
    {
        // >> ON INTERACTION -------------------------------------
        inkyInteractable.OnInteraction += () =>
        {
            CreateDialogueBubble(inkyInteractable.knotIterator.currentText);
        };

        // >> ON COMPLETED -------------------------------------
        inkyInteractable.OnCompleted += () =>
        {
            UIManager.WorldSpaceUI.Hide();
        };
    }

    public UXML_WorldSpaceUI CreateDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.WorldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one;
        worldSpaceUIDoc.SetText(text);
        return worldSpaceUIDoc;
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

