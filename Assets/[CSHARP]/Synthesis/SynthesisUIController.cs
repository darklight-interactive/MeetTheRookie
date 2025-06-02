using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handle the UI and <see cref="SynthesisClueElement"/>s.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SynthesisUIController : UXML_UIDocumentObject, IUnityEditorListener
{
    const string CONTAINER_TAG = "container";
    private VisualElement _rootContainer => ElementQuery<VisualElement>(CONTAINER_TAG);
    private MTRClueContainer _clueContainer => ElementQuery<MTRClueContainer>();

    void Awake()
    {
        MTRInputManager.OnTertiaryInteract += OnSynthesisButtonPressed;
        MTRStoryManager.OnGlobalKnowledgeUpdate += OnGlobalKnowledgeUpdate;
    }

    void Update() { }

    void OnSynthesisButtonPressed()
    {
        _rootContainer.visible = !_rootContainer.visible;
    }

    void OnGlobalKnowledgeUpdate(List<string> clues) { }
}
