using System.Collections;
using System.Collections.Generic;
using Darklight.Console;
using Darklight.Game;
using Ink.Runtime;
using UnityEngine;

public class InkyKnotIterator : StateMachine<InkyKnotIterator.State>
{
    public enum State { NULL, START, DIALOGUE, CHOICE, END }
    string Prefix => "[InkyKnot] >> ";
    Story story;
    string knotName;
    Dictionary<Ink.Runtime.Choice, int> choiceMap = new Dictionary<Ink.Runtime.Choice, int>();
    List<string> tags;
    List<Ink.Runtime.Choice> Choices => story.currentChoices;
    public string currentText => story.currentText;

    public InkyKnotIterator(Story storyParent, string knotName, State initialState = State.NULL) : base(initialState)
    {
        this.story = storyParent;
        this.knotName = knotName;

        // ( MOVE TO KNOT ) --------------- >>
        try
        {
            storyParent.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            InkyStoryManager.Console.Log($"{Prefix} Error: {e.Message}", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: {e.Message}");
        }
        finally
        {
            ChangeState(State.START);
            InkyStoryManager.Console.Log($"{Prefix} Moved to Knot: {knotName}");
        }
    }

    public void ContinueKnot()
    {

        // Check if null
        if (story == null || CurrentState == State.NULL)
        {
            InkyStoryManager.Console.Log($"{Prefix} Story is null", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: Story is null");
            return;
        }

        // Check if end
        if (CurrentState == State.END)
        {
            InkyStoryManager.Console.Log($"{Prefix} Knot has ended", 0, LogSeverity.Warning);
            Debug.LogWarning($"{Prefix} Knot has ended");
            return;
        }

        // -- ( DIALOGUE STATE ) --------------- >>
        if (story.canContinue)
        {
            ChangeState(State.DIALOGUE);

            string text = story.Continue();
            HandleTags();

            InkyStoryManager.Console.Log($"{Prefix} Continue Dialogue: {text}");
        }
        // -- ( CHOICE STATE ) --------------- >>
        else if (story.currentChoices.Count > 0)
        {
            ChangeState(State.CHOICE);
            InkyStoryManager.Console.Log($"{Prefix} Choices: {story.currentChoices.Count}", 1);

            foreach (Choice choice in story.currentChoices)
            {
                choiceMap.Add(choice, choice.index);
                InkyStoryManager.Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }

        // -- ( END STATE ) --------------- >>
        else
        {
            ChangeState(State.END);
            InkyStoryManager.Console.Log($"{Prefix} End of Knot");
            Debug.Log($"{Prefix} End of Knot");
        }
    }

    public void ChooseChoice(int choiceIndex)
    {
        Choice choice = story.currentChoices[choiceIndex];
        story.ChooseChoiceIndex(choice.index);
        choiceMap.Clear();
        ContinueKnot();
    }


    private void HandleTags()
    {
        List<string> currentTags = story.currentTags;
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                InkyStoryManager.Console.Log($"{Prefix} Tag is not formatted correctly: {tag}", 0, LogSeverity.Error);
            }
            else
            {
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                // handle the tag
                switch (tagKey)
                {
                    default:
                        InkyStoryManager.Console.Log($"{Prefix} Tag came in but is not currently being handled: {tag}", 0, LogSeverity.Warning);
                        break;
                }
            }

        }
    }
}

