using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    /*
    private Inventory inventory;
    public GameObject inventoryPanel;
    public GameObject nameAndDescription;

    public void AddItemToUI(GameObject newItem)
    {
        // Get the SpriteRenderer component from the collectible object
        SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            // Set the sorting order of the item's SpriteRenderer component
            newItem.GetComponent<SpriteRenderer>().sortingOrder = inventoryPanel.GetComponent<SpriteRenderer>().sortingOrder + 1;

            // Set the parent of the new item as the UI parent object
            newItem.transform.SetParent(transform);

            // Set the sprite of the item to the inInventory sprite
            //sr.sprite = newItem.GetComponent<I_Collectible>().inInventory;

            // Add a BoxCollider2D component to the item object
            BoxCollider2D collider = newItem.AddComponent<BoxCollider2D>();
            collider.size = sr.bounds.size;

            // Set the item's position to a random spot within the UI panel
            RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();
            float x = Random.Range(panelRect.rect.xMin, panelRect.rect.xMax);
            float y = Random.Range(panelRect.rect.yMin, panelRect.rect.yMax);
            Vector2 randomPosition = new Vector2(x, y);
            newItem.transform.position = randomPosition;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer component not found on the collectible object.");
        }
    }

    public void displayObjectInfo(GameObject selectedObject){
        //clear current object info
        nameAndDescription.GetComponent<Text>().text = "";
        Debug.Log( nameAndDescription.GetComponent<Text>().text);
        nameAndDescription.GetComponent<Text>().text = selectedObject.GetComponent<I_Collectible>().collectibleName + ": " + selectedObject.GetComponent<I_Collectible>().collectibleDescription;
        //display name and description of selected item
    }
    */
}
