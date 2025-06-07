using System;
using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using Ink.Runtime;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid2D_OverlapWeightSpawner))]
public class MTRChoiceReciever : InteractionReciever
{
    Grid2D_OverlapWeightSpawner _grid;

    /// <summary>
    /// The currently selected bubble
    /// </summary>
    [SerializeField, ReadOnly]
    TextBubbleObject _selectedBubble;

    [SerializeField, ShowOnly]
    int _selectedIndex;

    /// <summary>
    /// The queue of bubbles that are available to be selected
    /// </summary>
    [SerializeField, ReadOnly]
    List<TextBubbleObject> _choiceObjects = new List<TextBubbleObject>();
    Dictionary<TextBubbleObject, Choice> _bubbleChoiceMap =
        new Dictionary<TextBubbleObject, Choice>();

    /// <summary>
    /// The library of bubbleObjects that are attached to the grid
    /// </summary>
    public Library<Vector2Int, TextBubbleObject> _attachedBubbles;

    [SerializeField, Expandable]
    UXML_UIDocumentPreset _choiceBubblePreset;

    [SerializeField, Expandable]
    ChoiceBubbleLibrary _choiceBubbleLibrary;

    public override InteractionType InteractionType => InteractionType.CHOICE;
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
    public bool ChoiceSelected => _selectedBubble != null;
    Material _material => MTRGameManager.PrefabLibrary.uxmlRenderTextureMaterial;
    RenderTexture _renderTexture => MTRGameManager.PrefabLibrary.uxmlRenderTexture;

    public void Awake()
    {
        // << SETUP BUBBLE LIBRARY >>
        if (_choiceBubbleLibrary == null)
        {
            _choiceBubbleLibrary =
                MTRAssetManager.CreateOrLoadScriptableObject<ChoiceBubbleLibrary>();
        }

        // << SETUP ATTACHED BUBBLES >>
        if (_attachedBubbles == null)
        {
            _attachedBubbles = new Library<Vector2Int, TextBubbleObject>
            {
                ReadOnlyKey = true,
                ReadOnlyValue = true
            };

            _attachedBubbles.SetRequiredKeys(Grid.GetCellKeys().ToArray());
        }

        // << SETUP INPUT LISTENERS >>
        MTRInputManager.OnMoveInputStarted += (direction) => RotateSelection(direction.y > 0);
    }

    public void OnDestroy()
    {
        MTRInputManager.OnMoveInputStarted -= (direction) => RotateSelection(direction.y > 0);
    }

    public TextBubbleObject CreateBubbleAtNextAvailableCell(string fullText)
    {
        foreach (Cell2D cell in Grid.BaseGrid.GetCells())
        {
            // Skip cells that have colliders
            Cell2D.OverlapComponent overlapComponent = cell.GetComponent<Cell2D.OverlapComponent>();
            if (overlapComponent != null)
            {
                if (overlapComponent.ColliderCount > 0)
                {
                    continue;
                }
            }

            // If the cell doesn't have a key in the dictionary, create one
            if (!_attachedBubbles.ContainsKey(cell.Key))
                _attachedBubbles.Add(cell.Key, null);

            // If the value is null, it is available for a bubble
            if (_attachedBubbles[cell.Key] == null)
            {
                return CreateBubbleAt(cell, fullText);
            }
        }
        return null;
    }

    TextBubbleObject CreateBubbleAt(Cell2D cell, string fullText)
    {
        Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
        if (spawnerComponent != null)
        {
            TextBubbleObject choiceBubbleObject =
                UXML_Utility.CreateUXMLRenderTextureObject<TextBubbleObject>(
                    _choiceBubblePreset,
                    _material,
                    _renderTexture,
                    true
                );

            _attachedBubbles[cell.Key] = choiceBubbleObject;

            spawnerComponent.AttachTransformToCell(choiceBubbleObject.transform);
            choiceBubbleObject.transform.SetParent(this.transform);

            TextBubble choiceBubble = choiceBubbleObject.ElementQuery<TextBubble>();
            choiceBubble.OriginPoint = Grid.GetOriginPointFromCell(cell);
            choiceBubble.DirectionPoint = Grid.GetAnchorPointFromCell(cell);

            choiceBubble.FontSizePercentage = 240;

            choiceBubbleObject.SetText(fullText);

            Debug.Log($"Created Bubble at {cell.Key} with text: {fullText}");

            return choiceBubbleObject;
        }
        return null;
    }

    void DestroyAllBubbles()
    {
        List<UXML_RenderTextureObject> bubbles = new List<UXML_RenderTextureObject>(
            _attachedBubbles.Values
        );
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i] != null && bubbles[i].gameObject != null)
                ObjectUtility.DestroyAlways(bubbles[i].gameObject);
        }
        _attachedBubbles.Reset();
    }

    public void LoadChoices(List<Choice> choices)
    {
        // << CREATE BUBBLES >>
        for (int i = 0; i < choices.Count; i++)
        {
            // Instantiate the bubble at the next available cell
            TextBubbleObject choiceBubble = CreateBubbleAtNextAvailableCell(choices[i].text);
            _choiceObjects.Add(choiceBubble);

            // Map the text bubble to the choice
            _bubbleChoiceMap[choiceBubble] = choices[i];
        }

        // << SELECT THE FIRST BUBBLE >>
        if (_choiceObjects.Count > 0)
        {
            TextBubbleObject firstChoice = _choiceObjects[0];
            firstChoice.Select();
            _selectedBubble = firstChoice;
        }
    }

    /// <summary>
    /// Rotates the selection of choice bubbles either up or down in the queue
    /// </summary>
    /// <param name="rotateUp">If true, rotates selection upward (default). If false, rotates downward.</param>
    public void RotateSelection(bool rotateUp = true)
    {
        if (_selectedBubble == null || _choiceObjects.Count == 0)
            return;

        // Deselect the current bubble
        _selectedBubble.Deselect();
        if (rotateUp)
        {
            // Standard upward rotation
            _selectedIndex++;
            // If the selected index is greater than the number of choices, wrap around to the first index
            if (_selectedIndex >= _choiceObjects.Count)
                _selectedIndex = 0;
            _selectedBubble = _choiceObjects[_selectedIndex];
        }
        else
        {
            // Downward rotation
            _selectedIndex--;
            // If the selected index is less than 0, wrap around to the last index
            if (_selectedIndex < 0)
                _selectedIndex = _choiceObjects.Count - 1;
            _selectedBubble = _choiceObjects[_selectedIndex];
        }

        // Select the new bubble
        _selectedBubble.Select();
    }

    public void ConfirmChoice()
    {
        Choice choice = _bubbleChoiceMap[_selectedBubble];
        MTRStoryManager.ChooseChoice(choice);
        MTR_AudioManager.Instance.PlayMenuSelectEvent();

        _bubbleChoiceMap.Clear();
        _selectedBubble = null;
        _choiceObjects.Clear();
        DestroyAllBubbles();
    }

    void Select(SelectableButton newSelection)
    {
        /*
        if (newSelection == null || lockSelection) return;
        if (newSelection == selectedButton) return;

        // Transfer the selection
        previousButton = selectedButton;
        selectedButton = newSelection;

        // Set the selection classes
        previousButton?.Deselect();
        newSelection.SetSelected();

        lockSelection = true;
        */

        MTR_AudioManager.Instance.PlayMenuHoverEvent();
        Invoke(nameof(UnlockSelection), 0.1f);
    }

    void UnlockSelection()
    {
        //lockSelection = false;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRChoiceReciever))]
    public class ChoiceInteractionHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRChoiceReciever _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRChoiceReciever)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (GUILayout.Button("Create Bubble At Next Available Cell"))
            {
                List<string> testChoices = new List<string>
                {
                    "This is a long test choice that should wrap around the bubble.",
                    "This is a short test choice.",
                    "This is a medium length test choice that should wrap around the bubble.",
                    "This is a suuuuuper long test choice. Like unnecessarily long - like your momma long.",
                };

                string randChoice = testChoices[UnityEngine.Random.Range(0, testChoices.Count)];
                _script.CreateBubbleAtNextAvailableCell(randChoice);
            }

            if (GUILayout.Button("Destroy All Sprites"))
            {
                _script.DestroyAllBubbles();
            }

            if (GUILayout.Button("Rotate Selection"))
            {
                _script.RotateSelection();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
