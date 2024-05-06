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

    // ------ [[ SERIALIZED FIELDS ]] ------
    [Button("Load All Stories")]
    public void Load() => LoadAllStories();
    [SerializeField, ShowOnly] string[] _nameKeys = new string[0];
    [SerializeField] InkyStory[] _storyValues = new InkyStory[0];
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
    void LoadAllStories()
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
            InkyStory inkyStory = new InkyStory(storyAsset.name, story);
            stories.Add(storyAsset.name, inkyStory);
        }

        _stories = stories;
        _nameKeys = stories.Keys.ToArray();
        _storyValues = stories.Values.ToArray();
    }

    public InkyStory GetStory(string key)
    {
        if (_stories.ContainsKey(key))
        {
            return _stories[key];
        }
        else
        {
            Debug.LogError($"Story with key {key} not found.");
            return null;
        }
    }
}
