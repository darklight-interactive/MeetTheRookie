using Darklight.Game.Utility;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handle the UI
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SynthesisManager : MonoBehaviour
{
    [SerializeField] private UXML_UIDocumentPreset _preset;
    protected UIDocument document => GetComponent<UIDocument>();
    protected Dictionary<string, VisualElement> synthesisItems = new Dictionary<string, VisualElement>();
    public SelectableVectorField<VisualElement> itemsSelection = new SelectableVectorField<VisualElement>();

    /// <summary>
    /// Our group for showing the objects visually.
    /// </summary>
    VisualElement objects;
    VisualElement synthesizeButton;
    public void Awake()
    {
        document.visualTreeAsset = _preset.VisualTreeAsset;
        document.panelSettings = _preset.PanelSettings;
    }

    bool synthesisActive = false;
    void Start()
    {
        document.rootVisualElement.visible = false;

        objects = document.rootVisualElement.Q("objects");

        synthesizeButton = document.rootVisualElement.Q("title");
        itemsSelection.Add(synthesizeButton);

        Invoke("Initialize", 0.1f);
    }
    ///oijqwdoijqwodijqwd
    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }

        UniversalInputManager.MoveInputAction.performed += SelectMove;
        UniversalInputManager.PrimaryInteractAction.performed += Select;

        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);
    }

    /*public void Show(bool visible) {
        synthesisActive = visible;
        synthesisUI.rootVisualElement.visible = synthesisActive;
    }*/

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

    HashSet<VisualElement> toSynthesize = new HashSet<VisualElement>();
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
                toSynthesize.Remove(s);
            } else if (toSynthesize.Count < 3) { // Don't allow us to select more than three.
                s.AddToClassList("selected");
                toSynthesize.Add(s);
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


        VisualTreeAsset obj = (VisualTreeAsset)Resources.Load("Synthesis/" + objType);
        var tree = obj.Instantiate();

        foreach (var child in tree.Children()) {
            var dummyValue = (SynthesisBinding)((SynthesisBinding)child.dataSource).Clone();
            if (args.Length > 2) {
                dummyValue.setValue((string)args[2]);
            }
            child.dataSource = dummyValue;
        }

        tree.name = id;
        tree.AddToClassList("synthesis-object");

        objects.Add(tree);
        itemsSelection.Add(tree);
        return synthesisItems.TryAdd(id, tree);
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
    public VisualElement OverlappingObject(VisualElement synthesisObj) {
        /*var rect = synthesisObj.worldBound;
        foreach (var obj in synthesisItems) {
            if (obj.Value != synthesisObj && obj.Value.worldBound.Overlaps(rect, true)) {
                return obj.Value;
            }
        }*/
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SynthesisManager))]
public class SynthesisManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    SynthesisManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (SynthesisManager)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
