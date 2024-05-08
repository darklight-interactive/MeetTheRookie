using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Editor;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Class to load all Ink stories from the Resources/Inky directory.
/// </summary>
public class InkyStoryLoader : MonoBehaviour
{
    private const string PATH = "Inky/";
    private Dictionary<string, InkyStory> _stories = new Dictionary<string, InkyStory>();

    // ------ [[ SERIALIZED FIELDS ]] ------ >>
    [Button("Load All Stories")]
    public void Load() => LoadAllStories();
    [SerializeField, ShowOnly] string[] _nameKeys = new string[0];
    [SerializeField] InkyStory[] _storyValues = new InkyStory[0];


    // ------ [[ PUBLIC ACCESSORS ]] ------ >>
    public bool allStoriesLoaded { get; private set; }
    public string[] NameKeys
    {
        get
        {
            List<string> keys = _nameKeys.ToList();
            keys.Remove("mtr_global");
            return keys.ToArray();
        }
    }

    /// <summary>
    /// Loads all Ink story files found in the Resources/Inky directory.
    /// </summary>
    public void LoadAllStories()
    {
        _nameKeys = new string[0];
        _storyValues = new InkyStory[0];

        // Load all text assets from the Inky resources directory
        Dictionary<string, InkyStory> stories = new Dictionary<string, InkyStory>();
        UnityEngine.Object[] storyAssets = Resources.LoadAll(PATH, typeof(TextAsset));
        foreach (TextAsset storyAsset in storyAssets)
        {
            // Load the story from the text asset
            Story story = new Story(storyAsset.text);
            InkyStory inkyStory = ScriptableObject.CreateInstance<InkyStory>();
            stories.Add(storyAsset.name, inkyStory);
            SaveStoryData(inkyStory, storyAsset.name);
        }

        _stories = stories;
        _nameKeys = stories.Keys.ToArray();
        _storyValues = stories.Values.ToArray();
        allStoriesLoaded = true;
    }

    public InkyStory GetStory(string key)
    {
        if (!allStoriesLoaded) LoadAllStories();
        LoadStory(key);
        return _stories[key];
    }

    void LoadStory(string key)
    {
        if (_stories.ContainsKey(key)) return;
        TextAsset storyAsset = Resources.Load<TextAsset>(PATH + key);
        if (storyAsset == null)
        {
            Debug.LogError($"Story with key {key} not found.");
            return;
        }

        Story story = new Story(storyAsset.text);
        InkyStory inkyStory = new InkyStory(key, story);
        _stories.Add(key, inkyStory);
        _nameKeys = _stories.Keys.ToArray();
        _storyValues = _stories.Values.ToArray();
    }

#if UNITY_EDITOR

    private void SaveStoryData(InkyStory data, string name)
    {
        string path = "Assets/Resources/Inky/StoryObjects/" + name + ".asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
    }
#endif
}
