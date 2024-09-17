using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.ObjectLibrary;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueInteractionHandler : Grid2D_OverlapWeightSpawner
{
    UXML_RenderTextureObject _speechBubbleObject;
    [SerializeField, ShowOnly] string _speakerTag = "";
    [SerializeField] UXML_UIDocumentPreset _speechBubblePreset;
    [SerializeField] DialogueBubbleLibrary _dialogueBubbleLibrary;

    public string SpeakerTag { get => _speakerTag; set => _speakerTag = value; }
    public override void OnInitialize(Grid2D grid)
    {
        if (_dialogueBubbleLibrary == null)
        {
            _dialogueBubbleLibrary = MTR_AssetManager.CreateOrLoadScriptableObject<DialogueBubbleLibrary>();
        }

        // Register all the anchor points in the library
        List<Cell2D> cells = BaseGrid.GetCells();
        foreach (Cell2D cell in cells)
        {
            Spatial2D.AnchorPoint anchor = GetAnchorPointFromCell(cell);
            _dialogueBubbleLibrary.RegisterKey(anchor);
        }

        base.OnInitialize(grid);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        UpdateSpeechBubble();
    }

    public void CreateNewSpeechBubble(string text)
    {
        if (_speechBubbleObject != null)
        {
            DestroySpeechBubble();
        }

        // Create a new Bubble
        _speechBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject(_speechBubblePreset, MTR_UIManager.Instance.UXML_RenderTextureMaterial, MTR_UIManager.Instance.UXML_RenderTexture);
        _speechBubbleObject.transform.SetParent(transform);

        SpeechBubble speechBubble = _speechBubbleObject.ElementQuery<SpeechBubble>();
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

        UpdateSpeechBubble();
    }

    // Helper method to update the UI of the speech bubble
    SpeechBubble UpdateSpeechBubble()
    {
        if (_speechBubbleObject == null)
        {
            return null;
        }

        Cell2D bestCell = this.GetBestCell();

        // << ADJUST SPEECH BUBBLE TRANSFORM >>
        SpawnerComponent.AssignTransformToCell(_speechBubbleObject.transform, bestCell);

        // Determine which bubble sprite to use based on direction
        Spatial2D.AnchorPoint anchor = this.GetAnchorPointFromCell(bestCell);
        Spatial2D.AnchorPoint origin = this.GetOriginPointFromCell(bestCell);

        // Get the bubble sprite from the library
        Sprite bubbleSprite = _dialogueBubbleLibrary.GetObject(anchor);

        SpeechBubble speechBubble = _speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.UpdateFontSizeToMatchScreen();
        //speechBubble.style.color = color;
        speechBubble.SetBackgroundSprite(bubbleSprite);

        speechBubble.SetAnchorPoint(anchor);
        speechBubble.SetOriginPoint(origin);

        return speechBubble;
    }

    IEnumerator SpeechBubbleRollingTextRoutine(string fullText, float interval)
    {
        SpeechBubble speechBubble = _speechBubbleObject.ElementQuery<SpeechBubble>();
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
        if (_speechBubbleObject != null)
        {
            if (Application.isPlaying)
                Destroy(_speechBubbleObject.gameObject);
            else
                DestroyImmediate(_speechBubbleObject.gameObject);
        }
        _speechBubbleObject = null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DialogueInteractionHandler))]
    public class DialogueInteractionHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        DialogueInteractionHandler _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (DialogueInteractionHandler)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (_script._speechBubbleObject == null && GUILayout.Button("Create New Speech Bubble"))
            {
                _script.CreateNewSpeechBubble("Hello World");
            }
            else if (_script._speechBubbleObject != null && GUILayout.Button("Destroy Speech Bubble"))
            {
                _script.DestroySpeechBubble();
            }


            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
