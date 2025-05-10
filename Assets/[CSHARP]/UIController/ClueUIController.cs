using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryUIController : UXML_UIDocumentObject
{
    const string QUEST_CONTAINER = "quest-container";
    const string QUEST_GROUP_BOX = "quest-group-box";

    MTRStoryManager _storyManager;

    GroupBox _questGroupBox;
    List<Label> _questLabels = new List<Label>();

    public void Awake()
    {
        Initialize(preset);
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);

        _storyManager = MTRStoryManager.Instance;
        MTRStoryManager.OnActiveQuestListUpdate += UpdateQuests;

        _questGroupBox = ElementQuery<GroupBox>(QUEST_GROUP_BOX);
        //SetDefaultQuests();
    }

    void SetDefaultQuests()
    {
        ClearQuests();
        AddNewQuestLabelToGroupBox("Quest A");
        AddNewQuestLabelToGroupBox("Quest B");
        AddNewQuestLabelToGroupBox("Quest C");
    }

    void UpdateQuests(List<string> questNames)
    {
        ClearQuests();
        AddQuestLabelsToGroupBox(questNames);
    }

    void AddQuestLabelToGroupBox(Label label)
    {
        if (!_questLabels.Contains(label))
            _questLabels.Add(label);

        if (!_questGroupBox.Contains(label))
            _questGroupBox.Add(label);
    }

    void AddNewQuestLabelToGroupBox(string questName)
    {
        Label label = new Label(questName);
        label.AddToClassList("h3");
        AddQuestLabelToGroupBox(label);
    }

    void AddQuestLabelsToGroupBox(List<string> questNames)
    {
        foreach (string questName in questNames)
            AddNewQuestLabelToGroupBox(questName);
    }

    void RemoveQuestLabelFromGroupBox(Label label)
    {
        if (_questLabels.Contains(label))
            _questLabels.Remove(label);

        if (_questGroupBox.Contains(label))
            _questGroupBox.Remove(label);
    }

    void AddQuestLabelsToGroupBox(List<Label> questLabels)
    {
        foreach (Label label in questLabels)
            AddQuestLabelToGroupBox(label);
    }

    void RemoveQuestLabelsFromGroupBox(List<Label> questLabels)
    {
        foreach (Label label in questLabels)
            RemoveQuestLabelFromGroupBox(label);
    }

    void ClearQuests()
    {
        List<VisualElement> children = new List<VisualElement>(_questGroupBox.Children());
        foreach (VisualElement child in children)
            _questGroupBox.Remove(child);

        _questLabels.Clear();
    }
}
