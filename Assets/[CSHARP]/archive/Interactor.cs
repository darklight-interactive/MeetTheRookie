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
public class Interactor : MonoBehaviour
{
    private void Awake(){
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){

            collision.GetComponentInChildren<PlayerInteraction>().interactIcon.transform.position = transform.position;
            collision.GetComponentInChildren<PlayerInteraction>().OpenInteractableIcon();
            //LOGIC FOR DECIDING WHAT ICON SHOULD SHOW UP HERE USING VARIABLE type
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            collision.GetComponentInChildren<PlayerInteraction>().CloseInteractableIcon();
        }
    }
    private void PickUp(){
        Destroy(gameObject);
    }
}
