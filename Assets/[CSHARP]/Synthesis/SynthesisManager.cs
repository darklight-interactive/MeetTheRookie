using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Handle the UI and <see cref="SynthesisObject"/>s.
/// </summary>
public class SynthesisManager : MonoBehaviour, IGameSingleton<SynthesisManager>
{
    [SerializeField]
    protected UIDocument synthesisUI;

    protected Dictionary<string, SynthesisObject> synthesisItems = new Dictionary<string, SynthesisObject>();

    /// <summary>
    /// Our group for showing the objects visually.
    /// </summary>
    VisualElement objects;
    /// <summary>
    /// The <see cref="VirtualMouse"/> image we move around.
    /// </summary>
    VisualElement cursor;

    void Awake() {
        (this as IGameSingleton<SynthesisManager>).Initialize();

        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);

        synthesisUI.rootVisualElement.visible = false;
        objects = synthesisUI.rootVisualElement.Q("objects");
        cursor = synthesisUI.rootVisualElement.Q("cursor");
    }

    void Start() {
        AddItem(new[] { "Test" });
    }

    void Update() {
        cursor.transform.position = ISceneSingleton<VirtualMouse>.Instance.position;
    }

    public void Show(bool visible) {
        synthesisUI.rootVisualElement.visible = visible;

        // Unhook when we're not visible.
        ISceneSingleton<VirtualMouse>.Instance.HookTo(visible ? synthesisUI.rootVisualElement : null);
    }

    public object AddItem(object[] args) {
        string name = (string)args[0];
        var newObj = new SynthesisObject();
        newObj.noteHeader.text = name;
        objects.Add(newObj);
        //ISceneSingleton<VirtualMouse>.Instance.HookTo(newObj);
        return synthesisItems.TryAdd(name, newObj);
    }

    public object RemoveItem(object[] args) {
        var name = (string)args[0];
        objects.Remove(synthesisItems[name]);
        return synthesisItems.Remove(name);
    }

    public object HasItem(object[] args) {
        return synthesisItems.ContainsKey((string)args[0]);
    }
}
