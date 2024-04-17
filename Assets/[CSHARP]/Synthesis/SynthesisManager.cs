using Darklight.Game.Utility;
using Darklight.UnityExt.Input;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Handle the UI and <see cref="SynthesisObject"/>s.
/// </summary>
public class SynthesisManager : MonoBehaviourSingleton<SynthesisManager>
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

    public override void Awake()
    {
        base.Awake();

        //InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        //InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        //InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);

        synthesisUI.rootVisualElement.visible = false;
        objects = synthesisUI.rootVisualElement.Q("objects");
        cursor = synthesisUI.rootVisualElement.Q("cursor");
    }

    void Start() {
        AddItem(new[] { "Test" });
        AddItem(new[] { "OtherTest" });
    }

    void Update() {
        cursor.transform.position = VirtualMouse.Instance.position;
    }

    public void Show(bool visible) {
        synthesisUI.rootVisualElement.visible = visible;

        // Unhook when we're not visible.
        VirtualMouse.Instance.HookTo(visible ? synthesisUI.rootVisualElement : null);
    }

    public object CombineItems(object[] args) {
        if (args.Length != 2) {
            Debug.LogError("Could not get 2 items to combine from " + args);
            return null;
        }
        string a = (string)args[0];
        string b = (string)args[1];
        // Sort our combination items so we don't have to worry about multiple cases in our combine function:
        List<string> sortArr = new List<string> { a, b };
        sortArr.Sort();
        sortArr.Reverse();
        var final = sortArr.ToArray<object>();
        object newItem = InkyStoryManager.Instance.RunExternalFunction("combine", final);
        if (newItem.GetType() == typeof(string)) {

            RemoveItem(new[] { a });
            RemoveItem(new[] { b });
            return AddItem(new[] { newItem });
        } else {
            return newItem;
        }
    }

    public object AddItem(object[] args) {
        string name = (string)args[0];
        var newObj = new SynthesisObject();
        newObj.noteHeader.text = name;
        newObj.name = name;
        objects.Add(newObj);
        //ISceneSingleton<VirtualMouse>.Instance.HookTo(newObj);
        return synthesisItems.TryAdd(name, newObj);
    }

    public object RemoveItem(object[] args) {
        if ((bool)HasItem(args)) {
            var name = (string)args[0];
            synthesisItems[name].RemoveFromHierarchy();
            return synthesisItems.Remove(name);
        }
        return false;
    }

    public object HasItem(object[] args) {
        return synthesisItems.ContainsKey((string)args[0]);
    }

    public SynthesisObject OverlappingObject(VisualElement synthesisObj) {
        var rect = synthesisObj.worldBound;
        foreach (var obj in synthesisItems) {
            if (obj.Value != synthesisObj && obj.Value.worldBound.Overlaps(rect, true)) {
                return obj.Value;
            }
        }
        return null;
    }
}
