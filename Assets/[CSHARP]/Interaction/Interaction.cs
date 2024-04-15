using Darklight.Game.Grid;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteraction
{
    public string ink_knot { get; }
    public int counter { get; set; }
    public abstract void Target();
    public virtual void Interact()
    {
        counter++;
    }
    public virtual void Reset()
    {
        counter = 0;
    }
}

[RequireComponent(typeof(BoxCollider2D))]
public class Interaction : Grid2D_OverlapGrid, IInteraction
{
    public string ink_knot { get; private set; } = "default";
    public int counter { get; set; }
    public virtual void Target()
    {
        Vector3? worldPostion = null;
        if (worldPostion == null) worldPostion = transform.position;
        //UXML_InteractionUI.Instance.DisplayInteractPrompt((Vector3)worldPostion);
    }

    public virtual void Interact()
    {
        counter++;
        Debug.Log($"Interact >> {counter}");
    }
    public virtual void StartInteractionKnot(InkyKnot.KnotComplete onComplete)
    {
        //InkyKnotThreader.Instance.GoToKnotAt(ink_knot);
        //InkyKnotThreader.Instance.ContinueStory();
    }

    public virtual void ResetInteraction()
    {
        //UXML_InteractionUI.Instance.HideInteractPrompt();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interaction))]
public class InteractionEditor : Grid2D_OverlapGridEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Interaction interaction = (Interaction)target;

        if (GUILayout.Button("SpawnBubble"))
        {
            Grid2D_OverlapData data = interaction.GetBestData();
            if (data == null)
            {
                Debug.LogWarning("No data found to spawn bubble at");
                return;
            }

            UXML_WorldSpaceElement element = UXML_WorldSpaceUI.Instance.CreateComicBubbleAt(data.worldPosition);
            element.SetLocalScale(data.coordinateSize);
        }
    }

    private void OnSceneGUI()
    {
        Interaction interaction = (Interaction)target;
        Grid2D_OverlapData data = interaction.GetBestData();

        DrawGrid();
    }
}
#endif