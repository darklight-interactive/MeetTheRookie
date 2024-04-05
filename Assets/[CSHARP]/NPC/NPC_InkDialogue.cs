using System;

using Darklight.UnityExt;
using Darklight.Game.Grid2D;

using Ink.Runtime;

using UnityEngine;
using UnityEngine.UIElements;


public class NPC_DialogueBubble : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    public TextAsset inkJSONAsset;
    public Texture2D bubbleTexture;
    public RenderTexture renderTexture;
    public string inkString = "NPC_InkDialogue";
    Story story;

    void Awake()
    {
        StartStory();
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
        if (inkJSONAsset == null)
        {
            Debug.LogError("No ink story assigned;");
            return;
        }
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        inkString = story.Continue();
        RefreshView();
    }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collided with {other.name}");
    }



}
