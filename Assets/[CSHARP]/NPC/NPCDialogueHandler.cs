using Darklight.Game.Grid;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPCDialogueHandler : OverlapGrid2D
{

    public void CreateDialogueBubble()
    {
        OverlapGrid2D_Data data = this.GetBestData();
        Vector3 position = data.worldPosition;
        UXML_WorldSpaceElement element = UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(position);
        element.transform.SetParent(this.transform);
        element.transform.localScale = data.coordinateSize * Vector3.one;
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

