using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public List<GameObject> items;


    public GameObject item;
    private GameObject selectedObject;
    public GameObject inventoryPanel;
    public float dragSpeed = 100f; // Dragging speed of inventory items
    public float releaseDistanceThreshold = 100f; // Distance threshold to release the object being dragged
    
    private Vector3 offset;

    private void Awake()
    {
        items = new List<GameObject>();
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        items = new List<GameObject>();

        //inventoryPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
            if (Input.GetMouseButton(0) && selectedObject != null)
            {
                HandleObjectDrag();
            }
            if (Input.GetMouseButtonUp(0) && selectedObject != null)
            {
                selectedObject = null;
            }
    }
    public void AddToInventory(GameObject item)
    {
        items.Add(item);
        inventoryUI.AddItemToUI(item);
        // Optionally, you can disable or hide the collected item in the scene
        Debug.Log("ADDED OBJECT TO INVENTORY: "+ item.GetComponent<Collectible>().collectibleName);
        Debug.Log("OBJECT DESCRIPTION: "+ item.GetComponent<Collectible>().collectibleDescription);
        //item.SetActive(true);
    }

    public void RemoveFromInventory(GameObject item)
    {
        items.Remove(item);
        // Optionally, you can enable or show the removed item in the scene
        item.SetActive(true);
    }

    public bool ContainsItem(GameObject item)
    {
        return items.Contains(item);
    }

    public void ClearInventory()
    {
        items.Clear();
    }

    public int InvCount(){
        return items.Count;
    }
    private void HandleMouseClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            // Check if the clicked object is an inventory item
            if (items.Contains(clickedObject))
            {
                selectedObject = clickedObject;
                offset = clickedObject.transform.position - mousePosition;

                // Bring the selected object to the front by increasing its sorting order
                SpriteRenderer spriteRenderer = selectedObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = inventoryPanel.GetComponent<Canvas>().sortingOrder + 1;
            }
        }
    }
private GameObject[] FindOverlappingObjects()
{
    List<GameObject> overlappingObjects = new List<GameObject>();

    foreach (GameObject item in items)
    {
        if (item != selectedObject && IsOverlapping(selectedObject, item))
        {
            overlappingObjects.Add(item);
        }
    }

    return overlappingObjects.ToArray();
}

private void BringSelectedObjectToFront()
{
    // Find the closest object in front of the selected object
    GameObject closestObject = null;
    float closestDistance = Mathf.Infinity;
    foreach (GameObject item in items)
    {
        if (item != selectedObject)
        {
            float itemDistance = Vector3.Distance(selectedObject.transform.position, item.transform.position);
            if (itemDistance < closestDistance && !IsOverlapping(selectedObject, item))
            {
                closestObject = item;
                closestDistance = itemDistance;
            }
        }
    }

    // Find the highest sorting order among the items
    int highestSortingOrder = int.MinValue;
    foreach (GameObject item in items)
    {
        int sortingOrder = item.GetComponent<SpriteRenderer>().sortingOrder;
        if (sortingOrder > highestSortingOrder)
            highestSortingOrder = sortingOrder;
    }

    // Set the sorting order of the selected object to be one higher than the highest
    selectedObject.GetComponent<SpriteRenderer>().sortingOrder = highestSortingOrder + 1;

    // Decrease the sorting order of other items
    foreach (GameObject item in items)
    {
        if (item != selectedObject && item.GetComponent<SpriteRenderer>().sortingOrder > 0) item.GetComponent<SpriteRenderer>().sortingOrder--;
    }
}


private void HandleObjectDrag()
{
    if (selectedObject == null)
        return;

    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // Calculate the item's position with respect to the inventory panel
    Vector3 newPosition = mousePosition + offset;

    // Get the box collider of the inventory panel
    BoxCollider2D panelCollider = inventoryPanel.GetComponent<BoxCollider2D>();

    // Calculate the clamped position with respect to the panel's bounds
    Vector3 clampedPosition = panelCollider.bounds.ClosestPoint(newPosition);

    // Check the distance between the mouse position and the object being dragged
    float distance = Vector3.Distance(mousePosition, selectedObject.transform.position);

    if (distance > releaseDistanceThreshold)
    {
        selectedObject = null;
        return;
    }

    // Apply dragging speed to the clamped position (only change x and y positions)
    Vector3 finalPosition = Vector3.MoveTowards(selectedObject.transform.position, clampedPosition, dragSpeed * Time.deltaTime);
    finalPosition.z = selectedObject.transform.position.z; // Retain the original z position
    selectedObject.transform.position = finalPosition;

    GameObject[] overlappingObjects = FindOverlappingObjects();
    if (overlappingObjects.Length == 0)
    {
        BringSelectedObjectToFront();
    }
}


private bool IsOverlapping(GameObject objectA, GameObject objectB)
{
    Collider2D colliderA = objectA.GetComponent<Collider2D>();
    Collider2D colliderB = objectB.GetComponent<Collider2D>();

    if (colliderA != null && colliderB != null)
    {
        return colliderA.bounds.Intersects(colliderB.bounds);
    }

    return false;
}





}
