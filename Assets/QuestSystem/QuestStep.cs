using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestStepType { NONE, FETCH , TALK_TO , GO_TO , MINIGAME }

/*
base Quest class contains default Quest structure 
create new child classes for ea new questline with new names, required items and rewards.
*/

[System.Serializable]
public abstract class QuestStep : ScriptableObject
{
    public QuestStepType stepType = QuestStepType.NONE;
    public string description, title;
    public int ID;
    

    [Header("State")]
    public bool locked = true;
    public bool completed = false;

    [Space(20), Header("On Complete")]
    public UnityEvent OnComplete = new UnityEvent();
}

[CreateAssetMenu(fileName = "NewFetchQuestStep", menuName = "Fetch Quest")]
public class FetchQuestStep : QuestStep
{
    // Default constructor to set default values
    public FetchQuestStep()
    {
        stepType = QuestStepType.FETCH;
        title = "Fetch Quest";
        ID = 1001;
    }

}
