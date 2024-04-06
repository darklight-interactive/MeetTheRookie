using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/// <summary>
/// Singleton for managing all Ink UI.
/// </summary>
[System.Serializable]
public class StoryManager
{
    private static StoryManager instance;
    
    public static StoryManager Instance {
        get {
            if (instance == null) {
                instance = new StoryManager();
            }
            return instance;
        }
    }

    protected GameObject dialogPrefab;

    VisualElement inkUI;
    Label speaker;
    Label text;
    GroupBox choices;
    private StoryManager() {
        story = new Story(((TextAsset)Resources.Load("[INKY]/test")).text);
        dialogPrefab = (GameObject)Resources.Load("[INKY]/DialogBubble");

        inkUI = ISceneSingleton<UIManager>.Instance.GetUIComponent("inkDialog");
        speaker = inkUI.Query<Label>("speaker");
        text = inkUI.Query<Label>("inkText");
        choices = inkUI.Query<GroupBox>("choices");

        story.onError += (message, type) => {
            Debug.LogError("[Ink] " + type + " " + message);
        };
    }

    /// <summary>
    /// Look for [SpeakerName:] at the beginning of story text when finding a speaker.
    /// </summary>
    Regex dialogReader = new Regex(@"^(\[(?<speaker>.+):\]){0,1}(?<dialog>.*)");

    bool handlingChoice = false;
    int activeChoice = 0;
    /// <summary>
    /// Because Ink doesn't give us choice indices 1:1, we have this mapping instead.
    /// </summary>
    List<int> choiceMapping = new List<int>();
    public void Continue() {
        if (handlingChoice) {
            story.ChooseChoiceIndex(choiceMapping[activeChoice]);

            choices.Clear();
            choiceMapping.Clear();
            text.style.display = DisplayStyle.Flex;
            handlingChoice = false;
        }

        if (story.canContinue) {
            var storyText = story.Continue();
            var dialog = dialogReader.Match(storyText);
            if (dialog.Success) {
                if (dialog.Groups["speaker"].Success) {
                    speaker.text = dialog.Groups["speaker"].Value;
                    speaker.style.display = DisplayStyle.Flex;
                } else {
                    speaker.style.display = DisplayStyle.None;
                }
                text.text = dialog.Groups["dialog"].Value;
            } else {
                Debug.LogError("Regex match for dialog not found.");
                text.text = storyText;
            }
        } else {
            if (story.currentChoices.Count > 0) {
                handlingChoice = true;
                text.style.display = DisplayStyle.None;
                foreach (var choice in story.currentChoices) {
                    var choiceBox = new Button(() => {
                        activeChoice = choice.index;
                        Continue();
                    });
                    choiceBox.style.backgroundColor = new StyleColor(StyleKeyword.Initial);
                    choiceBox.text = choice.text;
                    choices.Add(choiceBox);
                    choiceMapping.Add(choice.index);
                }
                UpdateActiveChoice(0);
            } else {
                inkUI.visible = false;
                OnKnotCompleted();
            }
        }
    }

    void UpdateActiveChoice(int c) {
        choices[activeChoice].style.backgroundColor = new StyleColor(StyleKeyword.Initial);
        activeChoice = c;
        choices[activeChoice].style.backgroundColor = new StyleColor(Color.blue);
    }

    public void MoveUpdate(Vector2 move) {
        if (!handlingChoice) {
            return;
        }
        var x = Mathf.Sign(move.x);
        var y = -Mathf.Sign(move.y);

        int choice = activeChoice;
        if (Mathf.Abs(move.x) > 0.05f) {
            choice = (int)Mathf.Clamp(activeChoice + x, 0, story.currentChoices.Count - 1);
        } else if (Mathf.Abs(move.y) > 0.05f) {
            choice = (int)Mathf.Clamp(activeChoice + y, 0, story.currentChoices.Count - 1);
        }
        UpdateActiveChoice(choice);
    }

    public delegate void KnotComplete();
    protected event KnotComplete OnKnotCompleted;

    public void Run(string name, Transform transformToDisplay, KnotComplete onComplete) {
        story.ChoosePathString(name);
        inkUI.visible = true;
        inkUI.transform.position = UIManager.WorldToScreen(transformToDisplay.position) - new Vector3(inkUI.contentRect.width/4, inkUI.contentRect.height/2);
        Continue();
        OnKnotCompleted += onComplete;
    }

    protected Story story;
}
