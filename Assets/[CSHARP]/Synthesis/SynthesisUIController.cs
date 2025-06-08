using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
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
    const string MYSTERY_3_TAG = "mystery-3";

    private VisualElement _rootContainer => ElementQuery<VisualElement>(CONTAINER_TAG);
    private List<MTRClueContainer> _mysteryContainers =>
        new List<MTRClueContainer>
        {
            ElementQuery<MTRClueContainer>(MYSTERY_0_TAG),
            ElementQuery<MTRClueContainer>(MYSTERY_1_TAG),
            ElementQuery<MTRClueContainer>(MYSTERY_2_TAG),
            ElementQuery<MTRClueContainer>(MYSTERY_3_TAG)
        };

    [SerializeField, ShowOnly]
    private bool _isVisible = false;

    [SerializeField, ShowOnly]
    private int _activeMysteryIndex = 0;

    void Awake()
    {
        MTRInputManager.OnTertiaryInteract += OnSynthesisButtonPressed;
        MTRStoryManager.OnGlobalKnowledgeUpdate += OnGlobalKnowledgeUpdate;
        MTRStoryManager.OnMysteryKnowledgeUpdate += OnMysteryKnowledgeUpdate;

        SetVisible(false);
    }

    void Update() { }

    void OnDestroy()
    {
        MTRInputManager.OnTertiaryInteract -= OnSynthesisButtonPressed;
        MTRStoryManager.OnGlobalKnowledgeUpdate -= OnGlobalKnowledgeUpdate;
        MTRStoryManager.OnMysteryKnowledgeUpdate -= OnMysteryKnowledgeUpdate;
    }

    void OnSynthesisButtonPressed()
    {
        ToggleVisibility();

        if (_isVisible)
            MTRSceneController.StateMachine.GoToState(MTRSceneState.SYNTHESIS_MODE);
        else
            MTRSceneController.StateMachine.GoToState(MTRSceneState.PLAY_MODE);
    }

    void OnMysteryKnowledgeUpdate(int mysteryIndex)
    {
        Debug.Log("OnMysteryKnowledgeUpdate: " + mysteryIndex);
        SetActiveMystery(mysteryIndex);
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

    void SetActiveMystery(int mysteryIndex)
    {
        // If the mystery index is out of bounds, return
        if (mysteryIndex < 0 || mysteryIndex >= _mysteryContainers.Count)
            return;

        // Set the active mystery index
        _activeMysteryIndex = mysteryIndex;
        ResetMysteries();

        // Set the active mystery container to visible
        if (_mysteryContainers[mysteryIndex] != null)
            _mysteryContainers[mysteryIndex].style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Reset all the mystery containers to hidden
    /// </summary>
    void ResetMysteries()
    {
        foreach (var container in _mysteryContainers)
        {
            if (container == null)
                continue;

            container.style.display = DisplayStyle.None;
        }
    }

    void SetVisible(bool visible)
    {
        _isVisible = visible;
        _rootContainer.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
    }

    void DisplayMystery(int mysteryIndex)
    {
        SetActiveMystery(mysteryIndex);
    }

    [Button]
    public new void ToggleVisibility()
    {
        SetVisible(!_isVisible);
    }

    [Button]
    public void DisplayMystery0() => DisplayMystery(0);

    [Button]
    public void DisplayMystery1() => DisplayMystery(1);

    [Button]
    public void DisplayMystery2() => DisplayMystery(2);

    [Button]
    public void DisplayMystery3() => DisplayMystery(3);
}
