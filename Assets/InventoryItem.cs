using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private GameObject item;
    private bool isDragging;
    private Vector3 startPosition;

    public void SetItem(GameObject newItem)
    {
        item = newItem;
        // newItem.transform.SetParent(transform);
        // newItem.transform.localPosition = Vector3.zero;
    }
}
