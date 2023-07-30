using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
base Quest class contains default Quest structure 
create new child classes for ea new questline with new names, required items and rewards.
*/

public class Quest : MonoBehaviour
{
    //locked == if the quest is locked or not, active == if quest is active or not, 
    //completed == if the quest is completed or not
    public bool locked = true, active = false, completed= false; 

    public int progress = 0, maxProgress; 
    //progress is incremented every time a successful interaction or item check occurs
    //once max progress is reached the quest is complete
    public string prefix;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
