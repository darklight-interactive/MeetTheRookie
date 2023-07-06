using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour, IInteractable
{
    public void Interact(){
        Vector3 target= new Vector3(10, 0 , 0);
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime);
    }
}