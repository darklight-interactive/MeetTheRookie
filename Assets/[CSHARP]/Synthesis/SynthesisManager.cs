using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SynthesisManager : MonoBehaviour, IGameSingleton<SynthesisManager>
{
    [SerializeField]
    protected UIDocument inventoryUI;

    protected HashSet<string> inventoryItems;

    void Awake() {
        (this as IGameSingleton<SynthesisManager>).Initialize();

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
