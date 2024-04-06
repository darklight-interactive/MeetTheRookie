using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Teleport : MonoBehaviour, IInteractable
{   
    public bool isLocked = false;
    public GameObject destination;
    GameObject player;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Interact(){
        if(isLocked){ 
            Debug.Log("can't teleport it locked");
            return;
        }
        player.transform.position = destination.transform.position;
    }
}
