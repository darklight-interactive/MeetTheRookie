using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestlineManager : MonoBehaviour
{

    public List <Quest> questLine;
    public Quest currentQuest;
    public string level; //basically an ID for where this


    //what do we use for when the quest line is complete??

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //is called every time there is a new item added to the inventory.
    void CheckItem(){

    }
    //is called every time an object has been interacted w/
    void CheckInteraction(){

    }
    //progresses the current quest
    void progressQuest(){

    }
    //progresses the quest line, setting the previous quest to complete & inactive, & 
    void ProgressQuestline(){

    }
}
