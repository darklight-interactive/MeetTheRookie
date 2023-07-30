using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Collectible : MonoBehaviour, IInteractable
{   
    public bool isLocked = false;
    public string collectibleName= "this wasn't given a name yet lul";
    public string collectibleDescription= "this wasn't given a description yet lul";
    GameObject player;
    public Sprite physical;
    public Sprite inInventory;
    private SpriteRenderer sr;

    public void Interact()
    {   
        if(isLocked){
            Debug.Log("can't pick up the collectible! it locked");
            //ADD "lockedText" HERE!
            return;
        }
        //ADD LOGIC FOR POPUP
        AddToInventory();
    }


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = physical;
        
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void AddToInventory()
    {


        // Check if the Player has the Inventory component
        Inventory inventory = player.GetComponent<Inventory>();

        if (inventory != null)
        {
            // Add this collectible object to the inventory list
            inventory.AddToInventory(gameObject);

            // Optionally, you can disable or hide the object after collecting it
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Player does not have the Inventory component.");
        }
    }
}
