using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_NextObjective : MonoBehaviour, IInteractable
{
    public bool isLocked = false;
    
    public void Interact(){
        if(isLocked){
            Debug.Log("can't nextObjective it locked");
            return;
        }
    }
}