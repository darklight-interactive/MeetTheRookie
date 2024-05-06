using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;

[System.Serializable]
public class InkyStory
{
    [SerializeField, ShowOnly] private string _name;
    [SerializeField, ShowOnly] private Story _story;
    [SerializeField, ShowOnly] private List<string> _knotAndStitchKeys;
    [SerializeField] private List<InkyVariable> _variables;
    [SerializeField, ShowOnly] private List<string> _globalTags;
    public InkyStory(string name, Story story)
    {
        this._name = name;
        this._story = story;
        this._knotAndStitchKeys = GetKnotAndStitches(story);
        this._variables = GetVariables(story);
        this._globalTags = story.globalTags;

        // Set up error handling
        story.onError += (message, lineNum) => Debug.LogError($"Ink Error: {message} at line {lineNum}");
    }

    /// <summary>
    /// Returns a list of all knot and stitch keys in the given story.
    /// </summary>
    /// <param name="story"></param>
    /// <returns></returns>
    static List<string> GetKnotAndStitches(Story story)
    {
        var output = new List<string>();
        var knots = story.mainContentContainer.namedContent.Keys;
        knots.ToList().ForEach((knot) =>
        {
            output.Add(knot);

            var container = story.KnotContainerWithName(knot);
            var stitchKeys = container.namedContent.Keys;
            stitchKeys.ToList().ForEach((stitch) =>
            {
                output.Add(knot + "." + stitch);
            });
        });
        return output;
    }


    /// <summary>
    /// Retrieves all variables from an Ink story and wraps them in a dictionary.
    /// </summary>
    /// <param name="story">The Ink story from which to extract variables.</param>
    /// <returns>A dictionary with variable names as keys and wrapped variables as values.</returns>
    public static List<InkyVariable> GetVariables(Story story)
    {
        List<InkyVariable> output = new List<InkyVariable>();
        foreach (string variableName in story.variablesState)
        {
            object variableValue = story.variablesState[variableName];
            InkyVariable inkyVariable = new InkyVariable(variableName, variableValue);
            if (inkyVariable is null) { Debug.LogWarning($"Variable {variableName} is null."); continue; }
            output.Add(inkyVariable);
        }
        return output;
    }

    public static implicit operator Story(InkyStory story)
    {
        return story._story;
    }
}