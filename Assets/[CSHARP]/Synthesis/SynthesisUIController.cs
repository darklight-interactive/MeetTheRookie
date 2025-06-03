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
    const string MYSTERY_0_TAG = "mystery-0";
    const string MYSTERY_1_TAG = "mystery-1";
    const string MYSTERY_2_TAG = "mystery-2";

    private VisualElement _rootContainer => ElementQuery<VisualElement>(CONTAINER_TAG);
    private MTRClueContainer _mystery0ClueContainer =>
        ElementQuery<MTRClueContainer>(MYSTERY_0_TAG);
    private MTRClueContainer _mystery1ClueContainer =>
        ElementQuery<MTRClueContainer>(MYSTERY_1_TAG);
    private MTRClueContainer _mystery2ClueContainer =>
        ElementQuery<MTRClueContainer>(MYSTERY_2_TAG);

    private List<MTRClueContainer> _mysteryContainers;

    void Awake()
    {
        MTRInputManager.OnTertiaryInteract += OnSynthesisButtonPressed;
        MTRStoryManager.OnGlobalKnowledgeUpdate += OnGlobalKnowledgeUpdate;
        InitializeMysteryContainers();
    }

    private void InitializeMysteryContainers()
    {
        _mysteryContainers = new List<MTRClueContainer>
        {
            _mystery0ClueContainer,
            _mystery1ClueContainer,
            _mystery2ClueContainer
        };
    }

    void Update() { }

    void OnSynthesisButtonPressed()
    {
        _rootContainer.visible = !_rootContainer.visible;
    }

    void OnGlobalKnowledgeUpdate(List<string> knowledge)
    {
        Debug.Log("OnGlobalKnowledgeUpdate: " + string.Join(", ", knowledge));

        foreach (var container in _mysteryContainers)
        {
            if (container == null)
                continue;

            foreach (string clueName in knowledge)
            {
                container.Contains(clueName, out MTRClueElement clueElement);
                if (clueElement != null)
                {
                    clueElement.IsDiscovered = true;
                }
            }
        }
    }
}
