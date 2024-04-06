using System;
using Ink.Runtime;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UXML_InkyLabel : Label
{

    [UxmlAttribute, CreateProperty]
    public Texture2D bubbleTexture { get; set; }

    public UXML_InkyLabel()
    {
        AddToClassList("inky-label");
        this.text = "Inky Label";

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

            // Set the background image if bubbleSprite is assigned
            if (bubbleTexture != null)
            {
                Texture2D texture = bubbleTexture;
                this.style.backgroundImage = new StyleBackground(texture);
                //Debug.Log("Set background image");
            }
        });
    }
}

public class NPC_InkDialogue : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    public TextAsset inkJSONAsset;
    public Texture2D bubbleTexture;
    public RenderTexture renderTexture;
    public string inkString = "NPC_InkDialogue";
    Story story;
    VisualElement root;
    UXML_InkyLabel inkyLabel;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        inkyLabel = root.Q<UXML_InkyLabel>();
        root.Q<UXML_InkyLabel>().dataSource = this;

        inkyLabel.bubbleTexture = bubbleTexture;
        inkyLabel.text = inkString;
    }

    void OnDisable()
    {
    }

    void Awake()
    {
        StartStory();
    }

    void Update()
    {
        if (story != null)
        {
            inkyLabel.text = inkString;

        }
    }

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
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
