using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.UXML;
using Ink.Runtime;
using System;
using Darklight.UnityExt.Inky;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid2D_OverlapWeightSpawner))]
public class ChoiceInteractionReciever : InteractionReciever
{
    Grid2D_OverlapWeightSpawner _grid;

    TextBubbleObject _selectedBubble;
    Queue<TextBubbleObject> _choiceSelectionQueue = new Queue<TextBubbleObject>();
    Dictionary<TextBubbleObject, Choice> _bubbleChoiceMap = new Dictionary<TextBubbleObject, Choice>();

    public Library<Vector2Int, TextBubbleObject> _attachedBubbles;
    [SerializeField, Expandable] UXML_UIDocumentPreset _choiceBubblePreset;
    [SerializeField, Expandable] ChoiceBubbleLibrary _choiceBubbleLibrary;

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

    Material _material => MTR_UIManager.Instance.UXML_RenderTextureMaterial;
    RenderTexture _renderTexture => MTR_UIManager.Instance.UXML_RenderTexture;

    public void Awake()
    {
        if (_choiceBubbleLibrary == null)
        {
            _choiceBubbleLibrary = MTRAssetManager.CreateOrLoadScriptableObject<ChoiceBubbleLibrary>();
        }

        if (_attachedBubbles == null)
        {
            _attachedBubbles = new Library<Vector2Int, TextBubbleObject>
            {
                ReadOnlyKey = true,
                ReadOnlyValue = true
            };

            _attachedBubbles.SetRequiredKeys(Grid.GetCellKeys().ToArray());
        }

    }

    public void Update()
    {
        if (_choiceSelectionQueue.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                RotateSelection();
            }
        }
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
            TextBubbleObject choiceBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject<TextBubbleObject>(_choiceBubblePreset, _material, _renderTexture, true);

            _attachedBubbles[cell.Key] = choiceBubbleObject;

            spawnerComponent.AttachTransformToCell(choiceBubbleObject.transform, true, false);
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
        List<UXML_RenderTextureObject> bubbles = new List<UXML_RenderTextureObject>(_attachedBubbles.Values);
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i] != null && bubbles[i].gameObject != null)
                ObjectUtility.DestroyAlways(bubbles[i].gameObject);
        }
        _attachedBubbles.Reset();
    }


    public void LoadChoices(List<Choice> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            TextBubbleObject textBubble = CreateBubbleAtNextAvailableCell(choices[i].text);
            _choiceSelectionQueue.Enqueue(textBubble);

            // Map the text bubble to the choice
            _bubbleChoiceMap[textBubble] = choices[i];
        }

        if (_choiceSelectionQueue.Count > 0)
        {
            TextBubbleObject firstChoice = _choiceSelectionQueue.Dequeue();
            firstChoice.Select();
            _selectedBubble = firstChoice;
        }
    }

    public void RotateSelection()
    {
        if (_selectedBubble != null)
        {
            // Deselect the old bubble and enqueue it
            _selectedBubble.Deselect();
            _choiceSelectionQueue.Enqueue(_selectedBubble);
        }


        // Select the next bubble in the queue
        _selectedBubble = _choiceSelectionQueue.Dequeue();
        _selectedBubble.Select();
    }

    public void ConfirmChoice()
    {
        Choice choice = _bubbleChoiceMap[_selectedBubble];
        InkyStoryManager.Iterator.ChooseChoice(choice);
        MTR_AudioManager.Instance.PlayMenuSelectEvent();

        _bubbleChoiceMap.Clear();
        _selectedBubble = null;
        _choiceSelectionQueue.Clear();
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
    [CustomEditor(typeof(ChoiceInteractionReciever))]
    public class ChoiceInteractionHandlerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        ChoiceInteractionReciever _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (ChoiceInteractionReciever)target;
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


