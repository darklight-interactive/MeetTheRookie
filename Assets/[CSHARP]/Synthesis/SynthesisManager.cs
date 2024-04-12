using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SynthesisManager : MonoBehaviour, IGameSingleton<SynthesisManager>
{
    [SerializeField]
    protected UIDocument synthesisUI;

    protected Dictionary<string, SynthesisObject> synthesisItems;

    void Awake() {
        (this as IGameSingleton<SynthesisManager>).Initialize();

        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);

        synthesisUI.rootVisualElement.visible = false;
    }

    public void Show(bool visible) {
        synthesisUI.rootVisualElement.visible = visible;
    }

    public object AddItem(object[] args) {
        string name = (string)args[0];
        var newObj = new SynthesisObject();
        newObj.noteHeader.text = name;
        synthesisUI.rootVisualElement.Add(newObj);
        return synthesisItems.TryAdd(name, newObj);
    }

    public object RemoveItem(object[] args) {
        return synthesisItems.Remove((string)args[0]);
    }

    public object HasItem(object[] args) {
        return synthesisItems.ContainsKey((string)args[0]);
    }
}
