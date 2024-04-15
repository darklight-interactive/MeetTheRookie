using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/InkyData_Knot")]
public class InkyData_Knot : ScriptableObject
{
    public string knotID;
    [TextArea(3, 10)]
    public string storyText;
    public InkyData_Choice[] choices;
}

[System.Serializable]
public class InkyData_Choice
{
    public string text;
    public InkyData_Knot nextKnot;
}