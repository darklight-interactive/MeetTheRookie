using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Class to load all Ink stories from the Resources/Inky directory.
/// </summary>
public class InkyStoryLoader : MonoBehaviour
{
    [Button("Load All Stories")]
    public void LoadButton() => LoadAllStories();

    private const string PATH = "Inky/";
    [SerializeField, ShowOnly] List<string> _storyNames = new List<string>();
    [SerializeField] List<InkyStory> _stories = new List<InkyStory>();

    public void Awake()
    {
        LoadAllStories();
    }

    /// <summary>
    /// Loads all Ink story files found in the Resources/Inky directory.
    /// </summary>
    void LoadAllStories()
    {
        _storyNames.Clear();
        _stories.Clear();

        // Load all text assets from the Inky resources directory
        UnityEngine.Object[] storyAssets = Resources.LoadAll(PATH, typeof(TextAsset));
        foreach (TextAsset storyAsset in storyAssets)
        {
            _storyNames.Add(storyAsset.name);
            _stories.Add(new InkyStory(storyAsset.name, new Story(storyAsset.text)));
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(InkyStoryLoader))]
public class InkyStoryLoaderCustomEditor : Editor
{
    SerializedObject _serializedObject;
    InkyStoryLoader _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (InkyStoryLoader)target;
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
