using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using Darklight.UnityExt.Utility;
using UnityEngine;
using NaughtyAttributes;
using Darklight.UnityExt.UXML;
using Ink.Runtime;
using System;





#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid2D_OverlapWeightSpawner))]
public class ChoiceInteractionReciever : InteractionReciever
{
    Grid2D_OverlapWeightSpawner _grid;
    public Library<Vector2Int, UXML_RenderTextureObject> _attachedBubbles;
    [SerializeField] UXML_UIDocumentPreset _choiceBubblePreset;
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

    Material _material => MTR_UIManager.Instance.UXML_RenderTextureMaterial;
    RenderTexture _renderTexture => MTR_UIManager.Instance.UXML_RenderTexture;

    public void Awake()
    {
        if (_choiceBubbleLibrary == null)
        {
            _choiceBubbleLibrary = MTR_AssetManager.CreateOrLoadScriptableObject<ChoiceBubbleLibrary>();
        }

        if (_attachedBubbles == null)
        {
            _attachedBubbles = new Library<Vector2Int, UXML_RenderTextureObject>
            {
                ReadOnlyKey = true,
                ReadOnlyValue = true
            };

            _attachedBubbles.SetRequiredKeys(Grid.GetCellKeys().ToArray());
        }

    }

    public void CreateBubbleAtNextAvailableCell(string fullText)
    {
        foreach (Cell2D cell in Grid.BaseGrid.GetCells())
        {
            if (!_attachedBubbles.ContainsKey(cell.Key))
                _attachedBubbles.Add(cell.Key, null);

            if (_attachedBubbles[cell.Key] == null)
            {
                CreateBubbleAt(cell, fullText);
                return;
            }
        }
    }

    void CreateBubbleAt(Cell2D cell, string fullText)
    {
        Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
        if (spawnerComponent != null)
        {
            TextBubbleObject choiceBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject<TextBubbleObject>(_choiceBubblePreset, _material, _renderTexture);

            _attachedBubbles[cell.Key] = choiceBubbleObject;

            spawnerComponent.AttachTransformToCell(choiceBubbleObject.transform, false, true);
            choiceBubbleObject.transform.SetParent(this.transform);

            choiceBubbleObject.SetText(fullText);
        }
    }

    void DestroyAllBubbles()
    {
        List<UXML_RenderTextureObject> bubbles = new List<UXML_RenderTextureObject>(_attachedBubbles.Values);
        for (int i = 0; i < bubbles.Count; i++)
        {
            ObjectUtility.DestroyAlways(bubbles[i].gameObject);
        }
        _attachedBubbles.Reset();
    }


    public void LoadChoices(List<Choice> choices)
    {
        foreach (Choice choice in choices)
        {
            Debug.Log($"Choice: {choice.text}");
            CreateBubbleAtNextAvailableCell(choice.text);
        }
    }

    public void ConfirmChoice(Choice choice)
    {
        //InkyStoryManager.Iterator.ChooseChoice(choice);
        MTR_AudioManager.Instance.PlayMenuSelectEvent();
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


            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}


