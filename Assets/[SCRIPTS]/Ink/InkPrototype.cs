using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class InkPrototype : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    void Awake()
    {
        // Remove the default message
        //RemoveChildren();
        StartStory();
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        RefreshView();
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. 
    // If there are no choices, the story is finished!
    void RefreshView()
    {

        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
        }

    }


    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;

    [SerializeField]
    private Canvas canvas = null;

    // UI Prefabs
    [SerializeField]
    private Text textPrefab = null;
    [SerializeField]
    private Button buttonPrefab = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collided with {other.name}");
    }
}
