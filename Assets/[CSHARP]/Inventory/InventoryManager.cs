using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour, IGameSingleton<InventoryManager>
{
    [SerializeField]
    protected UIDocument inventoryUI;

    protected HashSet<string> inventoryItems;

    void Awake() {
        (this as IGameSingleton<InventoryManager>).Initialize();

        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);
    }

    public object AddItem(object[] args) {
        return inventoryItems.Add((string)args[0]);
    }

    public object RemoveItem(object[] args) {
        return inventoryItems.Remove((string)args[0]);
    }

    public object HasItem(object[] args) {
        return inventoryItems.Contains((string)args[0]);
    }
}
