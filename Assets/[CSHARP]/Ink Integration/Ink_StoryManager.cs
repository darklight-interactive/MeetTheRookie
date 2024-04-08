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
public class Ink_StoryManager
{
    private static Ink_StoryManager instance;
    public static Ink_StoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Ink_StoryManager();
            }
            return instance;
        }
    }
    private Ink_StoryManager()
    {
        story = new Story(((TextAsset)Resources.Load("[INKY]/test")).text);

        story.onError += (message, type) =>
        {
            Debug.LogError("[Ink] " + type + " " + message);
        };
    }

    /// <summary>
    /// Look for [SpeakerName:] at the beginning of story text when finding a speaker.
    /// </summary>
    Regex dialogueReader = new Regex(@"^(\[(?<speaker>.+):\]){0,1}(?<dialog>.*)");

    /// <summary>
    /// Because Ink doesn't give us choice indices 1:1, we have this mapping instead.
    /// </summary>
    List<int> choiceMapping = new List<int>();
    bool handlingChoice = false;
    int activeChoice = 0;


    public struct InkDialogue
    {
        public string speakerName;
        public string textBody;
    }


    public InkDialogue Continue()
    {
        InkDialogue newDialogue = new InkDialogue();

        // if player is handling a choice, choose the choice and continue
        if (handlingChoice)
        {
            story.ChooseChoiceIndex(choiceMapping[activeChoice]);
            choiceMapping.Clear();
            handlingChoice = false;
        }

        // continue the story text
        if (story.canContinue)
        {
            string storyText = story.Continue();

            // Get the token values from the dialogueReader
            Match dialogueTokens = dialogueReader.Match(storyText);
            if (dialogueTokens.Success)
            {
                // if speaker is found, set the speaker text
                if (dialogueTokens.Groups["speaker"].Success)
                {
                    newDialogue.speakerName = dialogueTokens.Groups["speaker"].Value;
                }

                newDialogue.textBody = dialogueTokens.Groups["dialog"].Value;
            }
            else
            {
                Debug.LogError("Regex match for dialog not found.");
                newDialogue.textBody = storyText;
            }
        }
        return newDialogue;



        /*

        // << CHECK FOR CHOICES  >>
        else
        {
            if (story.currentChoices.Count > 0)
            {
                handlingChoice = true;
                foreach (var choice in story.currentChoices)
                {
                    Button choiceBox = new Button(() =>
                    {
                        activeChoice = choice.index;
                        Continue();
                    });
                    choiceBox.style.backgroundColor = new StyleColor(StyleKeyword.Initial);
                    choiceBox.text = choice.text;
                    choices.Add(choiceBox);
                    choiceMapping.Add(choice.index);
                }
                UpdateActiveChoice(0);
            }
            else
            {
                inkUI.visible = false;
                OnKnotCompleted();
            }
        }
        */
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

    public void Run(string name, Ink_Interaction interactionOrigin, KnotComplete onComplete)
    {
        story.ChoosePathString(name);
        InkDialogue newDialogue = Continue();
        OnKnotCompleted += onComplete;

        UXML_InteractionUI interactionUI = ISceneSingleton<UXML_InteractionUI>.Instance;
        if (interactionUI != null)
            interactionUI.DisplayDialogueBubble(interactionOrigin.transform.position, newDialogue.textBody);
        else
            Debug.LogError("No interaction UI found.");

    }

    protected Story story;
}
