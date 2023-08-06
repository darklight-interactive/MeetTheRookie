using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/*
base Quest class contains default Quest structure 
create new child classes for ea new questline with new names, required items and rewards.
*/
[System.Serializable]
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest")]
public class QuestStep : ScriptableObject
{
    public GameObject gameManager;

    //locked == if the quest is locked or not, active == if quest is active or not, 
    //completed == if the quest is completed or not
    public string description, title;

    public string ID;

    public UnityEvent testEvent = new UnityEvent();

    public void onComplete(){
        testEvent?.Invoke();
    }

}
