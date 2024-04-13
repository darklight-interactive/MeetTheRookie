using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkyGlobalVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }
    private Story story;
    private const string saveVariablesKey = "INK_VARIABLES";

    public InkyGlobalVariables(Story story)
    {
        // create the story
        this.story = story;

        // if we have saved data, load it
        // if (PlayerPrefs.HasKey(saveVariablesKey))
        // {
        //     string jsonState = PlayerPrefs.GetString(saveVariablesKey);
        //     globalVariablesStory.state.LoadJson(jsonState);
        // }

        // initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in this.story.variablesState)
        {
            Ink.Runtime.Object value = this.story.variablesState.GetVariableWithName(name);
            variables.Add(name, value);

            InkyKnotThreader.Console.Log($"{InkyKnotThreader.Prefix} Initialized global dialogue variable: {name} = {value}", 1);
        }
    }


    public void SaveVariables()
    {
        if (story != null)
        {
            // Load the current state of all of our variables to the globals story
            VariablesToStory(story);
            // NOTE: eventually, you'd want to replace this with an actual save/load method
            // rather than using PlayerPrefs.
            PlayerPrefs.SetString(saveVariablesKey, story.state.ToJson());
        }
    }

    public void StartListening(Story story)
    {
        // it's important that VariablesToStory is before assigning the listener!
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;

    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        // only maintain variables that were initialized from the globals ink file
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
