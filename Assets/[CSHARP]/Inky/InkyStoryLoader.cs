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
    private Dictionary<string, InkyStoryObject> _stories = new Dictionary<string, InkyStoryObject>();

    // ------ [[ SERIALIZED FIELDS ]] ------ >>
    [Button("Load All Stories")]
    public void Load() => LoadAllStories();
    [SerializeField, ShowOnly] string[] _nameKeys = new string[0];
    [SerializeField] InkyStoryObject[] _storyValues = new InkyStoryObject[0];


    // ------ [[ PUBLIC ACCESSORS ]] ------ >>
    public bool allStoriesLoaded { get; private set; }

    /// <summary>
    /// Loads all Ink story files found in the Resources/Inky directory.
    /// </summary>
    public void LoadAllStories()
    {
        _nameKeys = new string[0];
        _storyValues = new InkyStoryObject[0];

        // Load all text assets from the Inky resources directory
        Dictionary<string, InkyStoryObject> stories = new Dictionary<string, InkyStoryObject>();
        UnityEngine.Object[] storyAssets = Resources.LoadAll("Inky/", typeof(TextAsset));
        foreach (TextAsset storyAsset in storyAssets)
        {
            InkyStoryObject inkyStory = ScriptableObject.CreateInstance<InkyStoryObject>();
            inkyStory.Initialize(storyAsset);
            SaveInkyStoryObject(storyAsset);

            stories.Add(storyAsset.name, inkyStory);
        }

        _stories = stories;
        _nameKeys = stories.Keys.ToArray();
        _storyValues = stories.Values.ToArray();
        allStoriesLoaded = true;
    }

    public InkyStoryObject GetStory(string key)
    {
        if (!allStoriesLoaded) LoadAllStories();
        return _stories[key];
    }


#if UNITY_EDITOR
    public InkyStoryObject SaveInkyStoryObject(TextAsset textAsset)
    {
        string path = $"Assets/Resources/Inky/StoryObjects/{textAsset.name}.asset";
        InkyStoryObject inkyStory = AssetDatabase.LoadAssetAtPath<InkyStoryObject>(path);

        if (inkyStory == null)
        {
            inkyStory = ScriptableObject.CreateInstance<InkyStoryObject>();
            AssetDatabase.CreateAsset(inkyStory, path);
        }

        inkyStory.Initialize(textAsset);
        EditorUtility.SetDirty(inkyStory);
        AssetDatabase.SaveAssets();

        return inkyStory;
    }
#endif
}
