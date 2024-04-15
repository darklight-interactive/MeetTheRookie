using Darklight.Game.Grid;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(InkyInteractable))]
public class NPCDialogueHandler : OverlapGrid2D
{
    private InkyInteractable inkyInteractable => GetComponent<InkyInteractable>();

    public void Start()
    {
        inkyInteractable.OnInteraction += () =>
        {
            UXML_WorldSpaceElement bubble = CreateDialogueBubble();
            bubble.comicBubble.Text = "Hello, World!";
        };
    }

    public UXML_WorldSpaceElement CreateDialogueBubble()
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;
        UXML_WorldSpaceElement element = UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(position);
        element.transform.SetParent(this.transform);
        element.transform.localScale = data.coordinateSize * Vector3.one;
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

        NPCDialogueHandler myScript = (NPCDialogueHandler)target;
        if (GUILayout.Button("Create Dialogue Bubble"))
        {
            myScript.CreateDialogueBubble();
        }
    }
}
#endif

