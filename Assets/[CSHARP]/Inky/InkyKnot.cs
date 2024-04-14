using System.Collections;
using System.Collections.Generic;
using Darklight.Console;
using Darklight.Game;
using Ink.Runtime;
using UnityEngine;

public class InkyKnot : StateMachine<InkyKnot.State>
{
    public enum State { NULL, START, DIALOGUE, CHOICE, END }
    string Prefix => "[InkyKnot] >> ";
    Story story;
    string knotName;
    Dictionary<Choice, int> choiceMap = new Dictionary<Choice, int>();
    List<string> tags;

    List<Choice> Choices => story.currentChoices;
    public InkyKnot(Story storyParent, string knotName, State initialState = State.NULL) : base(initialState)
    {
        this.story = storyParent;
        this.knotName = knotName;

        try
        {
            storyParent.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            InkyKnotThreader.Console.Log($"{Prefix} Error: {e.Message}", 0, LogSeverity.Error);
        }
        finally
        {
            InkyKnotThreader.Console.Log($"{Prefix} Created Knot: {knotName}", 1);
        }
    }

    public void ContinueKnot()
    {
        if (story.canContinue)
        {
            ChangeState(State.DIALOGUE);

            string text = story.Continue();
            HandleTags();

            InkyKnotThreader.Console.Log($"{Prefix} Continue Dialogue: {text}", 1);
        }
        else if (story.currentChoices.Count > 0)
        {
            ChangeState(State.CHOICE);
            InkyKnotThreader.Console.Log($"{Prefix} Choices: {story.currentChoices.Count}", 1);

            foreach (Choice choice in story.currentChoices)
            {
                choiceMap.Add(choice, choice.index);
                InkyKnotThreader.Console.Log($"{Prefix} Choice: {choice.text}", 1);
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
        Choice choice = story.currentChoices[choiceIndex];
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
                InkyKnotThreader.Console.Log($"{Prefix} Tag is not formatted correctly: {tag}", 0, LogSeverity.Error);
            }
            else
            {
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                // handle the tag
                switch (tagKey)
                {
                    default:
                        InkyKnotThreader.Console.Log($"{Prefix} Tag came in but is not currently being handled: {tag}", 0, LogSeverity.Warning);
                        break;
                }
            }

        }
    }
}

