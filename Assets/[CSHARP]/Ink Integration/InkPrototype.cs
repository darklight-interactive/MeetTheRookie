using System;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UXML_InkyLabel : Label
{
    [UxmlAttribute]
    public TextAsset inkJsonAsset { get; set; }

    [UxmlAttribute]
    public Texture2D bubbleTexture { get; set; }

    public UXML_InkyLabel()
    {


        // Wait for next frame
        this.schedule.Execute(() =>
        {
            // Assign default stylings ------------------------------ >
            this.style.whiteSpace = WhiteSpace.Normal; // textWrap
            this.style.unityTextAlign = TextAnchor.MiddleLeft;
            this.style.fontSize = 60;

            int padding = 100;
            this.style.paddingBottom = padding;
            this.style.paddingTop = padding;
            this.style.paddingLeft = padding;
            this.style.paddingRight = padding;

            this.style.minHeight = 250;
            this.text = "Inky Label";


            // Set the background image if bubbleSprite is assigned
            if (bubbleTexture != null)
            {
                Texture2D texture = bubbleTexture;
                this.style.backgroundImage = new StyleBackground(texture);
                //Debug.Log("Set background image");
            }

            // Get the Ink JSON Asset
            if (inkJsonAsset != null)
            {
                Story story = new Story(inkJsonAsset.text);
                story.Continue();
                this.text = story.currentText;
            }
            else
            {
                this.text = "No Ink JSON Asset assigned!";
            }
        });
    }
}


public class InkPrototype : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    [SerializeField] private TextAsset inkJSONAsset = null;
    public Story story;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collided with {other.name}");
    }
}
