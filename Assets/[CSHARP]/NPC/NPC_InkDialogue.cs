using System;

using Darklight.UnityExt;
using Darklight.Game.Grid2D;

using Ink.Runtime;

using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class UXML_InkyLabel : Label
{
    public UXML_InkyLabel()
    {
        AddToClassList("inky-label");
        this.text = "Inky Label";

        // Wait for next frame
        this.schedule.Execute(() =>
        {

        });
    }

    public void UpdateText(string text)
    {
        this.text = text;
    }
}

public class NPC_InkDialogue : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    public TextAsset inkJSONAsset;
    public Texture2D bubbleTexture;
    public RenderTexture renderTexture;
    public string inkString = "NPC_InkDialogue";
    public Grid2D<int> grid2D = new Grid2D<int>(new Vector2Int(3, 3), 1);
    Story story;
    VisualElement root;
    UXML_InkyLabel inkyLabel;

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        inkyLabel = root.Q<UXML_InkyLabel>();
        root.Q<UXML_InkyLabel>().dataSource = this;

        inkyLabel.UpdateText(inkString);

        grid2D.gridParent = this.transform;
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
        inkyLabel.UpdateText(inkString);
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

    void OnDrawGizmos()
    {
        foreach (Vector2Int vector2Int in grid2D.GetPositionKeys())
        {
            int coordinateValue = grid2D.GetCoordinateValue(vector2Int);
            Vector3 worldPosition = grid2D.GetCoordinatePositionInWorldSpace(vector2Int);
            CustomGizmos.DrawWireSquare_withLabel(
                $"{vector2Int} :: {coordinateValue}",
                worldPosition,
                grid2D.coordinateSize,
                Vector3.forward,
                Color.red,
                CustomGUIStyles.RightAlignedStyle);
        }
    }
}
