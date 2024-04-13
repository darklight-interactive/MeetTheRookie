using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
using Ink.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ComicBubbleHandler : OverlapGrid2D
{
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;

    public List<SO_ComicBubble> comicBubbles = new List<SO_ComicBubble>();

    /// <summary>
    /// Create a dialogue bubble gameobject in the world space at the given position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="destroy_after"></param>
    /// <returns></returns>
    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition, float destroy_after = -1f)
    {
        if (activeDialogueBubble == null)
        {
            // Create a new dialogue bubble
            this.activeDialogueBubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
            activeDialogueBubble.transform.SetParent(this.transform);
            activeDialogueBubble.transform.position = worldPosition;
            activeDialogueBubble.Initialize(visualTreeAsset, panelSettings);
        }

        if (destroy_after >= 0)
        {
            Destroy(activeDialogueBubble.gameObject, destroy_after);
        }

        return activeDialogueBubble;
    }


    /*
        public void CreateBubbleAtBestPosition()
        {
            // Create a dialogue bubble at the best position
            IGrid2DData data = overlapGrid.dataGrid.GetData(Vector2Int.zero);
            Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(data.positionKey);
            CreateDialogueBubbleAt(worldPosition);
        }

        public void CreateChoiceBubbleAtBestPosition(Choice choice)
        {
            // Create a dialogue bubble at the best position
            IGrid2DData data = overlapGrid.dataGrid.GetData(Vector2Int.zero);
            Vector3 worldPosition = overlapGrid.dataGrid.GetWorldSpacePosition(data.positionKey);
            UXML_InteractionUI.Instance.CreateChoiceBubble(worldPosition, choice);
        }

        public void CreateChoices()
        {
            if (InkyKnotThreader.Instance.currentStory.currentChoices.Count > 0)
            {
                foreach (Choice choice in InkyKnotThreader.Instance.currentStory.currentChoices)
                {
                    IGrid2DData data = overlapGrid.dataGrid.GetData(Vector2Int.zero);
                    UXML_WorldSpaceElement choiceElement = CreateDialogueBubbleAt(data.worldPosition, 5f);
                }
            }
        }
        */


#if UNITY_EDITOR
    [CustomEditor(typeof(ComicBubbleHandler))]
    public class ComicBubbleHandlerEditor : Editor
    {
        private void OnEnable()
        {
            ComicBubbleHandler comicBubbleHandler = (ComicBubbleHandler)target;
            if (comicBubbleHandler == null) return;
            comicBubbleHandler.Awake();
        }

        private void OnSceneGUI()
        {
            ComicBubbleHandler comicBubbleHandler = (ComicBubbleHandler)target;
            if (comicBubbleHandler == null) return;
            if (comicBubbleHandler.dataGrid == null) return;

            // Draw the overlap grid
            SO_Grid2DSettings.DisplayGrid2D(comicBubbleHandler.dataGrid);
        }
    }
#endif
}