using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;

/// <summary>
/// Scriptable Object to store Ink stories and their associated data.
/// </summary>
[CreateAssetMenu(menuName = "Darklight/Inky/Story")]
public class InkyStoryObject : ScriptableObject
{
    [SerializeField] private Story _story;
    [SerializeField] private TextAsset _inkFile;
    [SerializeField, ShowOnly] private List<string> _knotAndStitchKeys;
    [SerializeField] private List<InkyVariable> _variables;
    [SerializeField, ShowOnly] private List<string> _globalTags;
    public List<string> knotAndStitchKeys => _knotAndStitchKeys;


    public void Initialize(TextAsset inkTextAsset)
    {
        if (inkTextAsset == null)
        {
            Debug.LogError("Ink TextAsset is null.");
            return;
        }

        this._inkFile = inkTextAsset;
        this.name = inkTextAsset.name; // ScriptableObject name
        this._story = CreateStory();
        this._knotAndStitchKeys = GetKnotAndStitches(_story);
        this._variables = GetVariables(_story);
        this._globalTags = _story.globalTags;

        // Set up error handling
        _story.onError += (message, lineNum) => Debug.LogError($"Ink Error: {message} at line {lineNum}");
    }

    public Story CreateStory()
    {
        return new Story(_inkFile.text);
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
            if (knot.Contains("global")) return; // Skip the global declaration knot
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
}