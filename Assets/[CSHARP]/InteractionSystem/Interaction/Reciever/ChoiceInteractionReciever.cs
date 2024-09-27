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
    Dictionary<Vector2Int, UXML_RenderTextureObject> _attachedBubbles = new Dictionary<Vector2Int, UXML_RenderTextureObject>();
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
    }

    public void CreateBubbleAtNextAvailableCell(string fullText)
    {
        Cell2D cell = Grid.GetNextAvailableCell();
        if (cell != null)
        {
            CreateBubbleAt(cell, fullText);
        }
    }

    void CreateBubbleAt(Cell2D cell, string fullText)
    {
        Cell2D.SpawnerComponent spawnerComponent = cell.GetComponent<Cell2D.SpawnerComponent>();
        if (spawnerComponent != null)
        {
            UXML_RenderTextureObject choiceBubbleObject = UXML_Utility.CreateUXMLRenderTextureObject(_choiceBubblePreset, _material, _renderTexture);
            _attachedBubbles.Add(cell.Key, choiceBubbleObject);

            spawnerComponent.AttachTransformToCell(choiceBubbleObject.transform);
            choiceBubbleObject.transform.SetParent(this.transform);

            TextBubble textBubble = choiceBubbleObject.ElementQuery<TextBubble>();
            textBubble.SetFullText(fullText);
            textBubble.InstantCompleteText();
        }
    }

    void DestroyAllBubbles()
    {
        List<UXML_RenderTextureObject> bubbles = new List<UXML_RenderTextureObject>(_attachedBubbles.Values);
        for (int i = 0; i < bubbles.Count; i++)
        {
            ObjectUtility.DestroyAlways(bubbles[i].gameObject);
        }
    }


    public void LoadChoices(List<Choice> choices)
    {
        foreach (Choice choice in choices)
        {
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


            if (_script._attachedBubbles.Count == 0 && GUILayout.Button("Create Default Sprite At All"))
            {
                //_script.CreateBubbleAtAllCells(_script._choiceBubbleLibrary.DefaultValue);
            }
            else if (_script._attachedBubbles.Count > 0 && GUILayout.Button("Destroy All Sprites"))
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


