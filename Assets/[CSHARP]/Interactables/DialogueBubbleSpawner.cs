using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueBubbleSpawner : Grid2D_OverlapWeightSpawner
{
    UXML_RenderTextureObject speechBubbleObject;
    [SerializeField] UXML_UIDocumentPreset _speechBubblePreset;
    [Dropdown("_speakerOptions")] public string speakerTag;

    // ======== [[ PROPERTIES ]] ================================== >>>>
    List<string> _speakerOptions
    {
        // This is just a getter a list of all the speakers in the story
        get
        {
            List<string> speakers = new List<string>();
            if (InkyStoryManager.Instance != null)
            {
                speakers = InkyStoryManager.SpeakerList;
            }
            return speakers;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        UpdateSpeechBubble();
    }

    public void CreateNewSpeechBubble(string text)
    {
        if (speechBubbleObject != null)
        {
            DestroySpeechBubble();
        }

        // Return if the application is not playing
        if (!Application.isPlaying) { return; }

        // Create a new Bubble
        speechBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject(_speechBubblePreset, MTR_UIManager.Instance.UXML_RenderTextureMaterial, MTR_UIManager.Instance.UXML_RenderTexture);
        SpeechBubble speechBubble = speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            float fullTextHeight = evt.newRect.height;
            float fullTextWidth = evt.newRect.width;

            speechBubble.style.height = fullTextHeight;
            speechBubble.style.width = fullTextWidth;

            speechBubble.SetFullText(text);
            StartCoroutine(SpeechBubbleRollingTextRoutine(text, 0.025f));
        });

        speechBubble.SetFullText(text);
        speechBubble.InstantCompleteText(); // Temporarily display full text

    }

    // Helper method to update the UI of the speech bubble
    SpeechBubble UpdateSpeechBubble()
    {
        if (speechBubbleObject == null)
        {
            return null;
        }

        Cell2D bestCell = this.GetBestCell();

        // << ADJUST SPEECH BUBBLE TRANSFORM >>
        SpawnerComponent.AssignGameObjectToCell(speechBubbleObject.transform, bestCell);

        // Determine which bubble sprite to use based on direction
        Spatial2D.AnchorPoint anchor = this.GetAnchorPointFromCell(bestCell);
        Spatial2D.AnchorPoint origin = this.GetOriginPointFromCell(bestCell);

        UnityEngine.Object objectToSpawn = bestCell.ComponentReg.GetComponent<Cell2D.SpawnerComponent>().Data.ObjectToSpawn;
        Sprite bubbleSprite = null;
        if (objectToSpawn is Sprite)
        {
            bubbleSprite = objectToSpawn as Sprite;
        }

        SpeechBubble speechBubble = speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.UpdateFontSizeToMatchScreen();
        //speechBubble.style.color = color;
        speechBubble.SetBackgroundSprite(bubbleSprite);

        speechBubble.SetAnchorPoint(anchor);
        speechBubble.SetOriginPoint(origin);

        return speechBubble;
    }

    IEnumerator SpeechBubbleRollingTextRoutine(string fullText, float interval)
    {
        SpeechBubble speechBubble = speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.SetFullText(fullText);

        while (true)
        {
            for (int i = 0; i < speechBubble.fullText.Length; i++)
            {
                speechBubble.RollingTextStep();
                yield return new WaitForSeconds(interval);
            }
            yield return null;
        }
    }
    public void DestroySpeechBubble()
    {
        if (speechBubbleObject != null)
        {
            if (Application.isPlaying)
                Destroy(speechBubbleObject.gameObject);
            else
                DestroyImmediate(speechBubbleObject.gameObject);
        }
        speechBubbleObject = null;
    }
}
