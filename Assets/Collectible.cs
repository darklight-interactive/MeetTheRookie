using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{   
    public string collectibleName= "this wasn't given a name yet lul";
    public string collectibleDescription= "this wasn't given a description yet lul";

    public Sprite physical;
    public Sprite inInventory;
    private SpriteRenderer sr;

    public void Interact()
    {
        //ADD LOGIC FOR POPUP
        AddToInventory();
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = physical;
    }
    
    private void AddToInventory()
    {
        // Find the Player object using any method appropriate for your game
        GameObject player = GameObject.FindGameObjectWithTag("Player");

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
