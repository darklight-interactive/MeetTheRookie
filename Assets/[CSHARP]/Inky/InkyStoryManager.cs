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
public class InkyStoryManager
{
    private static InkyStoryManager instance;
    public static InkyStoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InkyStoryManager();
            }
            return instance;
        }
    }
    private InkyStoryManager()
    {
        story = new Story(((TextAsset)Resources.Load("Inky/test")).text);
        story.onError += (message, type) =>
        {
            Debug.LogError("[Ink] " + type + " " + message);
        };
    }

    public InkyDialogue currentInkDialog { get; private set; }

    /// <summary>
    /// Because Ink doesn't give us choice indices 1:1, we have this mapping instead.
    /// </summary>
    List<int> choiceMapping = new List<int>();
    bool handlingChoice = false;
    int activeChoice = 0;
    List<Choice> choices = new List<Choice>();
    public InkyDialogue Continue()
    {
        // if player is handling a choice, choose the choice and continue
        if (handlingChoice)
        {
            story.ChooseChoiceIndex(choiceMapping[activeChoice]);
            choiceMapping.Clear();
            handlingChoice = false;
        }

        // >> CONTINUE STORY --------------------------------
        if (story.canContinue)
        {
            currentInkDialog = new InkyDialogue(story.Continue());
            return currentInkDialog;
        }

        // >> CHECK FOR CHOICES -----------------------------------
        else if (story.currentChoices.Count > 0)
        {
            handlingChoice = true;

            // >> Get Choice Group Box
            UXML_InteractionUI.UXML_Element choiceGroupElement = UXML_InteractionUI.Instance.GetUIElement("choiceGroup");
            GroupBox groupBox = (GroupBox)choiceGroupElement.visualElement;

            // >> Iterate through choices
            foreach (Choice choice in story.currentChoices)
            {
                choices.Add(choice);

                // >> Create choice elements
                Button choiceBox = new Button(() =>
                {
                    activeChoice = choice.index;
                    Continue();
                });

                groupBox.Add(choiceBox);
                groupBox.visible = true;

                choiceBox.style.backgroundColor = new StyleColor(StyleKeyword.Initial);
                choiceBox.text = choice.text;
                choiceMapping.Add(choice.index);
            }
            UpdateActiveChoice(0);
        }
        else
        {
            OnKnotCompleted();
        }

        return null;
    }

    void UpdateActiveChoice(int c)
    {
        //choices[activeChoice].style.backgroundColor = new StyleColor(StyleKeyword.Initial);
        //activeChoice = c;
        //choices[activeChoice].style.backgroundColor = new StyleColor(Color.blue);
    }

    public void MoveUpdate(Vector2 move)
    {
        if (!handlingChoice)
        {
            return;
        }
        float x = Mathf.Sign(move.x);
        float y = -Mathf.Sign(move.y);

        int choice = activeChoice;
        if (Mathf.Abs(move.x) > 0.05f)
        {
            choice = (int)Mathf.Clamp(activeChoice + x, 0, story.currentChoices.Count - 1);
        }
        else if (Mathf.Abs(move.y) > 0.05f)
        {
            choice = (int)Mathf.Clamp(activeChoice + y, 0, story.currentChoices.Count - 1);
        }
        UpdateActiveChoice(choice);
    }

    public delegate void KnotComplete();
    protected event KnotComplete OnKnotCompleted;

    public void Run(string name, KnotComplete onComplete)
    {
        story.ChoosePathString(name);

        InkyDialogue dialogue = Continue();

        OnKnotCompleted += onComplete;
    }

    protected Story story;
}

/// <summary>
/// Ink Dialogue class to 
/// </summary>
public class InkyDialogue
{
    /// <summary>
    /// Look for [SpeakerName:] at the beginning of story text when finding a speaker.
    /// </summary>
    Regex dialogueReader = new Regex(@"^(\[(?<speaker>.+):\]){0,1}(?<dialog>.*)");

    public string speakerName = "[ Unknown ]";
    public string textBody = " default text body";

    public InkyDialogue(string storyText)
    {
        // Get the token values from the dialogueReader
        Match dialogueTokens = dialogueReader.Match(storyText);
        if (dialogueTokens.Success)
        {
            // if speaker is found, set the speaker text
            if (dialogueTokens.Groups["speaker"].Success)
            {
                this.speakerName = dialogueTokens.Groups["speaker"].Value;
            }

            this.textBody = dialogueTokens.Groups["dialog"].Value;
        }
        else
        {
            Debug.LogError("Regex match for dialog not found.");
            this.textBody = storyText;
        }
    }
}

public class StoryChoice
{

}