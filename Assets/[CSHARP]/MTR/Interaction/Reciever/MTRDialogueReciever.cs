using System.Collections;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid2D_OverlapWeightSpawner))]
public class MTRDialogueReciever : InteractionReciever
{
    Grid2D_OverlapWeightSpawner _grid;
    bool _isInDialogue = false;

    [SerializeField, ShowOnly]
    string _speakerTag = "";

    [SerializeField, ShowOnly]
    TextBubbleObject _dialogueBubbleObject;

    [SerializeField, Expandable]
    UXML_UIDocumentPreset _speechBubblePreset;

    [SerializeField, Expandable]
    TextBubbleLibrary _dialogueBubbleLibrary;

    Material _material => MTRGameManager.PrefabLibrary.uxmlRenderTextureMaterial;
    RenderTexture _renderTexture => MTRGameManager.PrefabLibrary.uxmlRenderTexture;

    public bool IsInDialogue => _isInDialogue;
    public override InteractionType InteractionType => InteractionType.DIALOGUE;
    public string SpeakerTag
    {
        get => _speakerTag;
        set => _speakerTag = value;
    }
    public Grid2D_OverlapWeightSpawner Grid
    {
        get
        {
            if (_grid == null)
            {
                _grid = GetComponent<Grid2D_OverlapWeightSpawner>();
            }
            return _grid;
        }
    }

    public void Awake()
    {
        if (_dialogueBubbleLibrary == null)
        {
            _dialogueBubbleLibrary =
                MTRAssetManager.CreateOrLoadScriptableObject<TextBubbleLibrary>();
        }
    }

    public void Update()
    {
        UpdateTextBubble();
    }

    public void CreateNewSpeechBubble(string text)
    {
        if (_dialogueBubbleObject != null)
        {
            DestroySpeechBubble();
        }

        // Create a new Bubble
        _dialogueBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject<TextBubbleObject>(
            _speechBubblePreset,
            _material,
            _renderTexture
        );
        _dialogueBubbleObject.transform.SetParent(transform);

        Cell2D bestCell = Grid.GetBestCell();
        Grid.SpawnerComponent.AssignTransformToCell(_dialogueBubbleObject.transform, bestCell);

        TextBubble textBubble = _dialogueBubbleObject.ElementQuery<TextBubble>();

        textBubble.Library = _dialogueBubbleLibrary; // << Set the bubble library

        // Determine which bubble sprite to use based on direction
        Spatial2D.AnchorPoint anchor = Grid.GetAnchorPointFromCell(bestCell);
        Spatial2D.AnchorPoint origin = Grid.GetOriginPointFromCell(bestCell);

        textBubble.OriginPoint = origin;
        textBubble.DirectionPoint = anchor;

        textBubble.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            float fullTextHeight = evt.newRect.height;
            float fullTextWidth = evt.newRect.width;

            textBubble.style.height = fullTextHeight;
            textBubble.style.width = fullTextWidth;

            textBubble.SetFullText(text);
            StartCoroutine(SpeechBubbleRollingTextRoutine(text, 0.025f));
        });

        textBubble.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            //Debug.Log($"DialogueTextBubble.OnInitialized() - ChangeEvent<string>", this);
        });

        textBubble.SetFullText(text);
        textBubble.InstantCompleteText(); // Temporarily display full text

        UpdateTextBubble();
    }

    // Helper method to update the UI of the speech bubble
    TextBubble UpdateTextBubble()
    {
        if (_dialogueBubbleObject == null)
        {
            return null;
        }

        Cell2D bestCell = Grid.GetBestCell();

        // << ADJUST SPEECH BUBBLE TRANSFORM >>
        Grid.SpawnerComponent.AssignTransformToCell(_dialogueBubbleObject.transform, bestCell);

        /*
                // Determine which bubble sprite to use based on direction
                Spatial2D.AnchorPoint anchor = Grid.GetAnchorPointFromCell(bestCell);
                Spatial2D.AnchorPoint origin = Grid.GetOriginPointFromCell(bestCell);
        */
        TextBubble textBubble = _dialogueBubbleObject.ElementQuery<TextBubble>();

        //textBubble.OriginPoint = origin;
        //textBubble.DirectionPoint = anchor;

        return textBubble;
    }

    IEnumerator SpeechBubbleRollingTextRoutine(string fullText, float interval)
    {
        TextBubble speechBubble = _dialogueBubbleObject.ElementQuery<TextBubble>();
        speechBubble.SetFullText(fullText);
        _isInDialogue = true;

        while (_isInDialogue)
        {
            for (int i = 0; i < speechBubble.FullText.Length; i++)
            {
                speechBubble.RollingTextStep();
                yield return new WaitForSeconds(interval);
            }
            _isInDialogue = false;
            yield return null;
        }
    }

    public void ForceComplete()
    {
        Debug.Log("Force Complete");
        StopCoroutine(SpeechBubbleRollingTextRoutine("", 0.025f));
        _isInDialogue = false;

        if (_dialogueBubbleObject == null)
        {
            return;
        }
        TextBubble textBubble = _dialogueBubbleObject.ElementQuery<TextBubble>();
        textBubble.InstantCompleteText();
    }

    public void DestroySpeechBubble()
    {
        Debug.Log("Destroying Speech Bubble");
        if (_dialogueBubbleObject != null)
        {
            if (Application.isPlaying)
                Destroy(_dialogueBubbleObject.gameObject);
            else
                DestroyImmediate(_dialogueBubbleObject.gameObject);
        }
        _dialogueBubbleObject = null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRDialogueReciever))]
    public class DialogueInteractionHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRDialogueReciever _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRDialogueReciever)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (GUILayout.Button("Create New Speech Bubble"))
            {
                _script.CreateNewSpeechBubble("Hello, World!");
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
