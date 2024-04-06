using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Location { NONE, GS_INTERIOR, GS_FRONT }

[System.Serializable]
[CreateAssetMenu(fileName = "Location_[NAME]", menuName = "QuestComplete/Location")]
public class Location_QuestComplete : QuestComplete
{
    public Location locationType;
}
