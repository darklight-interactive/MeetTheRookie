using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using Ink.Runtime;
using UnityEngine;


// << KNOTS >>
[System.Serializable]
public class InkyKnot
{
    [ShowOnly] public string name;
    [ShowOnly] public List<string> stitches;
}

/// <summary>
/// Scriptable Object to store Ink stories and their associated data.
/// </summary>
[CreateAssetMenu(menuName = "Darklight/Inky/StoryObject")]
public class InkyStoryObject : ScriptableObject
{
    #region ----- [[ STATIC METHODS ]] ----- >>

    /// <summary>
    /// Creates an Ink story from a TextAsset.
    /// </summary>
    /// <param name="inkTextAsset">
    ///     The TextAsset containing the Ink story. Typically, this is a generated .json file.
    /// </param>
    public static Story CreateStory(TextAsset inkTextAsset)
    {
        return new Story(inkTextAsset.text);
    }

    /// <summary>
    /// Retrieves all knots in an Ink story.
    /// </summary>
    /// <param name="story">
    ///     The Ink story from which to extract knots.
    /// </param>
    /// <returns>
    ///     A list of knot names.
    /// </returns>
    public static List<string> GetAllKnots(Story story)
    {
        return story.mainContentContainer.namedContent.Keys.ToList();
    }

    /// <summary>
    /// Retrieves all stitches in a knot from an Ink story.
    /// </summary>
    /// <param name="story">
    ///     The Ink story from which to extract stitches.
    /// </param>
    /// <param name="knot">
    ///     The knot from which to extract stitches.
    /// </param>
    /// <returns>
    ///     A list of stitch names.
    /// </returns>
    public static List<string> GetAllStitchesInKnot(Story story, string knot)
    {
        Container container = story.KnotContainerWithName(knot);
        List<string> stitches = new List<string>();
        foreach (string stitch in container.namedContent.Keys.ToList())
        {
            stitches.Add($"{knot}.{stitch}");
        }
        return stitches;
    }

    /// <summary>
    /// Retrieves all variables from an Ink story and wraps them in a dictionary.
    /// </summary>
    /// <param name="story">
    ///    The Ink story from which to extract variables.
    /// </param>
    /// <returns>
    ///    A dictionary of variable names and their values.
    ///    The key is the variable name and the value is the variable value.
    ///    The variable value is an object, so it must be cast to the appropriate type.
    ///    For example, if the variable is an integer, cast it to an integer.
    /// </returns>
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
    #endregion

    // ------------------------------ [[ PRIVATE FIELDS ]] ------------------------------ >>
    private Story _story;
    [SerializeField] private TextAsset _inkyTextAsset;
    [SerializeField] private List<InkyKnot> _knots;
    [SerializeField] private List<InkyVariable> _variables;
    [SerializeField, ShowOnly] private List<string> _globalTags;
    private List<string> _boundFunctions = new List<string>();

    // ------------------------------ [[ PUBLIC METHODS ]] ------------------------------ >>

    /// <summary>
    /// Public reference to the Ink story inside the StoryObject.
    /// </summary>
    public Story StoryValue
    {
        get
        {
            if (_story == null)
            {
                _story = CreateStory(_inkyTextAsset);
            }
            return _story;
        }
        set
        {
            _story = value;
        }
    }

    /// <summary>
    /// Initializes the story object with data from the given Inky TextAsset.
    /// </summary>
    /// <param name="textAsset">The Inky TextAsset containing the Ink story.</param>
    public void Initialize(TextAsset textAsset = null)
    {
        // Set the ink text asset if the parameter is not null
        if (textAsset != null)
        {
            this._inkyTextAsset = textAsset;
        }

        // return if no ink story set
        if (this._inkyTextAsset == null)
        {
            Debug.LogError("InkyStoryObject: Ink story not set.");
            return;
        }

        this.name = _inkyTextAsset.name; // << set ScriptableObject name
        this._story = CreateStory(_inkyTextAsset);
        this._knots = GetAllKnots(_story).Select(knot => new InkyKnot
        {
            name = knot,
            stitches = GetAllStitchesInKnot(_story, knot)
        }).ToList();
        this._variables = GetVariables(_story);
        this._globalTags = _story.globalTags;

        // Set up error handling
        _story.onError += (message, lineNum) => Debug.LogError($"Ink Error: {message} at line {lineNum}");
    }

    public void UpdateVariables()
    {
        _variables = GetVariables(_story);
    }

    public InkyVariable GetVariableByName(string variableName)
    {
        return _variables.Find(variable => variable.Key == variableName);
    }

    /// <summary>
    /// Binds an external function to the Ink story.
    /// </summary>
    /// <param name="funcName">The name of the function in the Ink story.</param>
    /// <param name="function">Reference to the external function.</param>
    /// <param name="lookaheadSafe"></param>
    public void BindExternalFunction(string funcName, Story.ExternalFunction function, bool lookaheadSafe = false)
    {
        if (_boundFunctions.Contains(funcName))
        {
            Debug.LogWarning($"Function {funcName} is already bound.");
            return;
        }
        _boundFunctions.Add(funcName);
        StoryValue.BindExternalFunctionGeneral(funcName, function, lookaheadSafe);
    }

    /// <summary>
    /// Runs an function in the Ink story.
    /// </summary>
    /// <param name="func">The name of the function in the Ink story.</param>
    /// <param name="args">The arguments to the function.</param>
    /// <returns>An Object containing the return value of the function.</returns>
    public object RunExternalFunction(string func, object[] args)
    {
        if (StoryValue.HasFunction(func))
        {
            return StoryValue.EvaluateFunction(func, args);
        }
        else
        {
            Debug.LogError("Could not find function: " + func);
            return null;
        }
    }
}