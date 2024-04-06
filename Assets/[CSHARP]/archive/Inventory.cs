using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new();
    private int maxItems = 16;

    public void AddToInventory(Item item)
    {
        if (item)
        {
            items.Add(item);
        }
        else
        {
            Debug.LogError("Cannot add null reference to Inventory.", this);
        }
    }

    public bool AreItemsInInventory(List<Item> input_items)
    {
        foreach (Item item in input_items)
        {
            if (!items.Contains(item)) { return false; }
        }
        return true;
    }
}
