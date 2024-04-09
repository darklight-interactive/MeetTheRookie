using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface IInteractable
{
    GameObject gameObject { get; }
    public void Interact();
}

[RequireComponent(typeof(BoxCollider2D))]
public class Interaction : MonoBehaviour
{
    private void Awake(){
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
}
