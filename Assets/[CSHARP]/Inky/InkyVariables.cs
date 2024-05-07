using System.Collections.Generic;
using Ink.Runtime;

public interface IInkyVariable
{
    string name { get; }
    string ToString();
}

public abstract class InkyVariable<T> : IInkyVariable
{
    public string name { get; private set; }
    public T value { get; private set; }

    public InkyVariable(string name, T value)
    {
        this.name = name;
        this.value = value;
    }

    public override string ToString()
    {
        return $"{name} = {value}";
    }
}

public class InkyVariableList : InkyVariable<InkList>, IInkyVariable
{
    public InkyVariableList(string name, InkList value) : base(name, value) { }
    public override string ToString()
    {
        return value.ToString().Trim();
    }
}

public class InkyVariableObject : InkyVariable<Ink.Runtime.Object>, IInkyVariable
{
    public InkyVariableObject(string name, Ink.Runtime.Object value) : base(name, value) { }
}

public class InkyVariableHandler
{
    public Dictionary<string, IInkyVariable> variables { get; private set; } = new Dictionary<string, IInkyVariable>();
    private Story story;
    private const string saveVariablesKey = "INK_VARIABLES";

    public InkyVariableHandler(Story story)
    {
        // create the story
        this.story = story;

        // initialize the dictionary
        variables.Clear();
        foreach (string variableName in story.variablesState)
        {
            object inkValue = story.variablesState[variableName];
            if (inkValue is InkList)
            {
                InkyVariableList inkList = new InkyVariableList(variableName, inkValue as InkList);
                variables.Add(variableName, inkList);
                //Debug.Log($"{InkyStoryManager.Prefix} Initialized global dialogue variable: {variableName} = {inkValue.ToString()}");
            }
            else if (inkValue is Ink.Runtime.Object)
            {
                InkyVariableObject inkObject = new InkyVariableObject(variableName, inkValue as Ink.Runtime.Object);
                variables.Add(variableName, inkObject);
            }
            else
            {
                InkyStoryManager.Console.Log($"{InkyStoryManager.Prefix} Error: Unhandled variable type: {inkValue.GetType()}", 0);
            }
        }

        /*
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in this.story.variablesState)
        {
            Ink.Runtime.Object value = this.story.variablesState.GetVariableWithName(name);
            variables.Add(name, value);

            InkyStoryWeaver.Console.Log($"{InkyStoryWeaver.Prefix} Initialized global dialogue variable: {name} = {value}", 1);
        }
        */
    }
}
