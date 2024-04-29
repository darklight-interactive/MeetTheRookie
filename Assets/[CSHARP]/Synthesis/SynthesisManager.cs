using Darklight.Game.Utility;
using Darklight.UnityExt.Input;
using System;
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
    protected VisualTreeAsset synthesisUIDocument;
    protected VisualElement synthesisUI;

    protected Dictionary<string, SynthesisObject> synthesisItems = new Dictionary<string, SynthesisObject>();
    public SelectableVectorField<VisualElement> itemsSelection = new SelectableVectorField<VisualElement>();

    /// <summary>
    /// Our group for showing the objects visually.
    /// </summary>
    VisualElement objects;

    VisualElement synthesizeButton;
    public override void Awake()
    {
        base.Awake();
        synthesisUI = synthesisUIDocument.Instantiate();
        GetComponent<UIDocument>().rootVisualElement.Add(synthesisUI);
        objects = synthesisUI.Q("objects");
        synthesizeButton = synthesisUI.Q("title");
        itemsSelection.Add(synthesizeButton);

        synthesisUI.visible = false;
    }

    bool synthesisActive = false;
    void Start() {
        Invoke("Initialize", 0.1f);
    }

    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }

        UniversalInputManager.MoveInputAction.performed += SelectMove;
        UniversalInputManager.PrimaryInteractAction.performed += Select;

        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);
    }

    public void Show(bool visible) {
        synthesisActive = visible;
        synthesisUI.visible = visible;
    }

    void SelectMove(InputAction.CallbackContext context) {
        if (!synthesisActive) { return; }
        Vector2 move = UniversalInputManager.MoveInputAction.ReadValue<Vector2>();
        move.y = -move.y;
        if (itemsSelection.currentlySelected != null) {
            itemsSelection.currentlySelected.RemoveFromClassList("highlight");
        }
        var selected = itemsSelection.getFromDir(move);
        if (selected != null) {
            selected.AddToClassList("highlight");
        }
    }

    HashSet<SynthesisObject> toSynthesize = new HashSet<SynthesisObject>();
    void Select(InputAction.CallbackContext context) {
        if (!synthesisActive) { return; }
        if (itemsSelection.currentlySelected != null) {
            var s = itemsSelection.currentlySelected;
            if (s == synthesizeButton) {
                Synthesize();
                return;
            }

            if (s.ClassListContains("selected")) {
                s.RemoveFromClassList("selected");
                toSynthesize.Remove((SynthesisObject)s);
            } else if (toSynthesize.Count < 3) { // Don't allow us to select more than three.
                s.AddToClassList("selected");
                toSynthesize.Add((SynthesisObject)s);
            }
        }
    }

    void Synthesize() {
        List<string> args = new List<string>();

        foreach (var item in toSynthesize) {
            item.RemoveFromClassList("selected");
            args.Add(item.name);
        }
        toSynthesize.Clear();

        args = args.OrderBy(s => s).ToList();

        if (args.Count == 2) {
            args.Add("");
        }

        InkyStoryManager.Instance.RunExternalFunction("synthesize", args.ToArray());
    }

    [Obsolete("Synthesis is handled by Synthesize instead.")]
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
        if (args.Length < 2) {
            Debug.LogError("playerAddItem args: [synthesisItemType] [itemID]");
            return null;
        }
        string objType = (string)args[0];
        string id = (string)args[1];

        object[] remainingArgs = new object[args.Length - 2];
        Array.Copy(args, 2, remainingArgs, 0, args.Length - 2);

        VisualTreeAsset obj = (VisualTreeAsset)Resources.Load("Synthesis/" + objType);
        var tree = obj.Instantiate();
        
        SynthesisObject newObj = tree.Q<SynthesisObject>();
        newObj.name = id;
        newObj.Configure(remainingArgs);

        objects.Add(newObj);
        itemsSelection.Add(newObj);
        return synthesisItems.TryAdd(id, newObj);
    }

    public object RemoveItem(object[] args) {
        if ((bool)HasItem(args)) {
            var name = (string)args[0];
            synthesisItems[name].RemoveFromHierarchy();
            itemsSelection.Remove(synthesisItems[name]);
            return synthesisItems.Remove(name);
        }
        return false;
    }

    public object HasItem(object[] args) {
        return synthesisItems.ContainsKey((string)args[0]);
    }

    [Obsolete("Dragging should not be used for synthesis items.")]
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
