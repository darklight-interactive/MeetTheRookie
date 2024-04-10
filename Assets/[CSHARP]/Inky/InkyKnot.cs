using System.Collections;
using System.Collections.Generic;
using Darklight;
using Ink.Runtime;
using UnityEngine;

public enum KnotState { NULL, START, DIALOGUE, CHOICE, END }
public class InkyKnot : StateMachine<KnotState>
{
    string Prefix => "[InkyKnot] >> ";
    Story story;
    string knotName;
    Dictionary<Choice, int> choiceMap = new Dictionary<Choice, int>();
    List<string> tags;

    public string currentText => story.currentText;
    public InkyKnot(Story storyParent, string knotName, KnotState initialState = KnotState.NULL) : base(initialState)
    {
        this.story = storyParent;
        this.knotName = knotName;

        try
        {
            storyParent.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{Prefix} Error: {e.Message}");
        }
    }

    public void ContinueStory()
    {
        if (story.canContinue)
        {
            ChangeState(KnotState.DIALOGUE);

            string text = story.Continue();
            Debug.Log($"{Prefix} Continue: {text}");
        }
        else if (story.currentChoices.Count > 0)
        {
            ChangeState(KnotState.CHOICE);
            Debug.Log($"{Prefix} Choices: {story.currentChoices.Count}");

            foreach (Choice choice in story.currentChoices)
            {
                choiceMap.Add(choice, choice.index);
                Debug.Log($"{Prefix} Choice: {choice.text}");
            }
        }
        else
        {
            ChangeState(KnotState.END);

            OnKnotCompleted.Invoke();

            Debug.Log($"{Prefix} End of Knot");
        }
    }

    public void ChooseChoice(int choiceIndex)
    {
        Choice choice = story.currentChoices[choiceIndex];
        story.ChooseChoiceIndex(choice.index);
        choiceMap.Clear();
        ContinueStory();
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
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }
}

