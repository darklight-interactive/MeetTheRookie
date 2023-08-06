using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Questline : ScriptableObject
{

    public List <QuestStep> quests;
    public QuestStep currentQuest;

    public string description, title;

    public string ID;

    
    //what do we use for when the quest line is complete??


    //progresses the quest line, setting the previous quest to complete & inactive, & 
    void Progress(){

    }
}
