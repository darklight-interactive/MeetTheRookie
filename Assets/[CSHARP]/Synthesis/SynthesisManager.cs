using Darklight.UnityExt.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;




#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handle the UI and <see cref="SynthesisClueElement"/>s.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SynthesisManager : UXML_UIDocumentObject, IUnityEditorListener
{
    public void OnEditorReloaded()
    {
        Show(false);
    }

    const string LIBRARY_PATH = "Assets/Resources/Synthesis";
    const string LIBRARY_NAME = "SynthesisClueLibrary";
    public SynthesisClueLibrary clueLibrary;

    protected Dictionary<string, SynthesisClueElement> synthesisItems = new Dictionary<string, SynthesisClueElement>();
    public SelectableVectorField<VisualElement> itemsSelection = new SelectableVectorField<VisualElement>();


    /// <summary>
    /// Our group for showing the objects visually.
    /// </summary>
    VisualElement mystery1Container;
    VisualElement synthesizeButton;
    bool synthesisActive = false;


    void Start()
    {
        Show(false);

        clueLibrary = ScriptableObjectUtility.CreateOrLoadScriptableObject<SynthesisClueLibrary>(LIBRARY_PATH, LIBRARY_NAME);
        clueLibrary.LoadMysteryClues();

        mystery1Container = ElementQuery<GroupBox>("mystery1");
        synthesizeButton = ElementQuery<VisualElement>("title");
        itemsSelection.Add(synthesizeButton);

        Initialize();
    }

    ///oijqwdoijqwodijqwd
    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }

        //UniversalInputManager.OnMoveInputStarted += SelectMove;
        UniversalInputManager.OnPrimaryInteract += Select;
        //InkyStoryManager.GlobalStoryObject.BindExternalFunction("playerAddItem", AddItem);

        InkyStoryManager.GlobalStoryObject.StoryValue.BindExternalFunction("AddSynthesisClue", (string clue) => AddClue(clue));

        InkyStoryManager.GlobalStoryObject.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.GlobalStoryObject.BindExternalFunction("playerHasItem", HasItem);
    }

    void SelectMove(Vector2 move)
    {
        if (synthesisActive){
            move.y = -move.y;
            if (itemsSelection.CurrentSelection != null)
            {
                itemsSelection.CurrentSelection.RemoveFromClassList("highlight");
            }
            var selected = itemsSelection.GetElementInDirection(move);
            if (selected != null) {
                selected.AddToClassList("highlight");
            }
        }
    }

    [Button]
    public void AddClue(string clue)
    {
        Debug.Log("Synthesis Adding clue: " + clue);
        SynthesisClueElement newClue = new SynthesisClueElement();
        newClue.text = clue;

        //mystery1Container = ElementQuery<GroupBox>("mystery1");
        //mystery1Container.Add(newClue);
    }

    HashSet<VisualElement> toSynthesize = new HashSet<VisualElement>();
    void Select()
    {
        if (synthesisActive && itemsSelection.CurrentSelection != null)
        {
            var s = itemsSelection.CurrentSelection;
            if (s == synthesizeButton) {
                Synthesize();
                return;
            }

            if (s.ClassListContains("selected")) {
                s.RemoveFromClassList("selected");
                toSynthesize.Remove((VisualElement)s);
            } else if (toSynthesize.Count < 3) { // Don't allow us to select more than three.
                s.AddToClassList("selected");
                toSynthesize.Add((VisualElement)s);
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

        InkyStoryManager.GlobalStoryObject.RunExternalFunction("synthesize", args.ToArray());
    }

    /*
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
        object newItem = InkyStoryManager.Instance.GlobalStoryObject.RunExternalFunction("combine", final);
        if (newItem.GetType() == typeof(string)) {

            RemoveItem(new[] { a });
            RemoveItem(new[] { b });
            return AddItem(new[] { newItem });
        } else {
            return newItem;
        }
    }
    */

    public object AddItem(string itemName)
    {
        if (synthesisItems.ContainsKey(itemName))
        {
            return false;
        }
        var synthesisObj = new SynthesisClueElement();
        synthesisItems.Add(itemName, synthesisObj);
        mystery1Container.Add(synthesisObj);
        itemsSelection.Add(synthesisObj);
        return true;
    }

    public object RemoveItem(object[] args) {
        Debug.Log(args[0]);
        Debug.Log(HasItem(args));
        if ((bool)HasItem(args)) {
            var name = (string)args[0];
            Debug.Log(name);
            Debug.Log(synthesisItems[name]);
            synthesisItems[name].visible = false;
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
    public SynthesisClueElement OverlappingObject(VisualElement synthesisObj)
    {
        var rect = synthesisObj.worldBound;
        foreach (var obj in synthesisItems) {
            if (obj.Value != synthesisObj && obj.Value.worldBound.Overlaps(rect, true)) {
                return obj.Value;
            }
        }
        return null;
    }

    public void Show(bool visible)
    {
        synthesisActive = visible;
        Debug.Log("SynthesisManager: Show(" + visible + ")");
        //VisualElement container = ElementQuery<VisualElement>("synthesis-container");
        //container.visible = visible;
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
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Toggle Visibility"))
        {
            _script.ToggleVisibility();
        }

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
