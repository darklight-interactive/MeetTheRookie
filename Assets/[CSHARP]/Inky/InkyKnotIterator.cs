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
    public InkyKnotIterator(Story storyParent, string knotName, State initialState = State.NULL) : base(initialState)
    {
        this.story = storyParent;
        this.knotName = knotName;

        try
        {
            storyParent.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            InkyStoryWeaver.Console.Log($"{Prefix} Error: {e.Message}", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: {e.Message}");
        }
        finally
        {
            InkyStoryWeaver.Console.Log($"{Prefix} Moved to Knot: {knotName}");
        }
    }

    public void ContinueKnot()
    {
        if (story == null)
        {
            InkyStoryWeaver.Console.Log($"{Prefix} Story is null", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: Story is null");
            return;
        }

        if (story.canContinue)
        {
            ChangeState(State.DIALOGUE);

            string text = story.Continue();
            HandleTags();

            InkyStoryWeaver.Console.Log($"{Prefix} Continue Dialogue: {text}");
        }
        else if (story.currentChoices.Count > 0)
        {
            ChangeState(State.CHOICE);
            InkyStoryWeaver.Console.Log($"{Prefix} Choices: {story.currentChoices.Count}", 1);

            foreach (Ink.Runtime.Choice choice in story.currentChoices)
            {
                choiceMap.Add(choice, choice.index);
                InkyStoryWeaver.Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }
        else
        {
            ChangeState(State.END);

            OnKnotCompleted.Invoke();

            Debug.Log($"{Prefix} End of Knot");
        }
    }

    public void ChooseChoice(int choiceIndex)
    {
        Ink.Runtime.Choice choice = story.currentChoices[choiceIndex];
        story.ChooseChoiceIndex(choice.index);
        choiceMap.Clear();
        ContinueKnot();
    }

    public delegate void KnotComplete();
    protected event KnotComplete OnKnotCompleted;
    public void Run(string name, KnotComplete onComplete)
    {
        story.ChoosePathString(name);

        //InkyDecryptor dialogue = storyParent.Continue();

        OnKnotCompleted += onComplete;
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
                InkyStoryWeaver.Console.Log($"{Prefix} Tag is not formatted correctly: {tag}", 0, LogSeverity.Error);
            }
            else
            {
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                // handle the tag
                switch (tagKey)
                {
                    default:
                        InkyStoryWeaver.Console.Log($"{Prefix} Tag came in but is not currently being handled: {tag}", 0, LogSeverity.Warning);
                        break;
                }
            }

        }
    }
}

