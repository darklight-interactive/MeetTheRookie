using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Audio : MonoBehaviour, IInteractable
{
    public bool isLocked = false;
    string prefix = "((I_AUDIO)) ";

    public void Interact()
    {
        if(isLocked){
            Debug.Log("can't audio it locked");
            return;
        }
        Debug.Log(prefix + " Audio Trigger");
    }



}
