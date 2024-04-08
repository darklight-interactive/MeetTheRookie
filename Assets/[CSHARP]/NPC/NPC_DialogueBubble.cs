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
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    public void SetBubble(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}

[ExecuteAlways]
public class NPC_DialogueBubble : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        [SerializeField, TextArea(3, 10)] public string inkyLabel = "Hello from the default settings on the NPC_DialogueBubble. >>";
        public Sprite bubbleSprite;
        public Material material_prefab;
        public RenderTexture renderTexture_prefab;
        public Settings() { }
        public Settings(string dialogueText, Sprite bubbleSprite, Material material_prefab, RenderTexture renderTexture_prefab)
        {
            this.inkyLabel = dialogueText;
            this.bubbleSprite = bubbleSprite;
            this.material_prefab = material_prefab;
            this.renderTexture_prefab = renderTexture_prefab;
        }
    }

    public Settings settings = new Settings();
    NPC_UIHandler uiHandler;
    UIDocument uiDocument;
    PanelSettings panelSettings;
    VisualElement root;
    UXML_InkyLabel inkyLabel;
    VisualElement bubble;
    MeshRenderer meshRenderer;

    public void Awake()
    {
        // UXML Components
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement; // << get the root visual element from the UXML file
        panelSettings = uiDocument.panelSettings;

        // INKY Label
        inkyLabel = root.Q<UXML_InkyLabel>();
        bubble = root.Q<VisualElement>("BubbleSprite");
        root.Q<UXML_InkyLabel>().dataSource = this; // set data source for binding attributes

        // Quad
        meshRenderer = GetComponentInChildren<MeshRenderer>();


    }

    public void ManualUpdate()
    {
        NPC_UIHandler uiHandler = GetComponentInParent<NPC_UIHandler>();
        settings = new Settings("Test: This is where the ink script would connect to and set the current dialogue", settings.bubbleSprite,
            uiHandler.defaultBubbleSettings.material_prefab, uiHandler.defaultBubbleSettings.renderTexture_prefab);

        inkyLabel.SetText(settings.inkyLabel);
        bubble.style.backgroundImage = new StyleBackground(settings.bubbleSprite);

        // Destroy old render texture
        if (panelSettings.targetTexture != null)
        {
            panelSettings.targetTexture.Release();

            // destroy the old render texture in play and edit mode
            if (Application.isPlaying)
            {
                Destroy(panelSettings.targetTexture);
            }
            else
            {
                DestroyImmediate(panelSettings.targetTexture);
            }
        }

        // create a new render texture
        panelSettings.targetTexture = new RenderTexture(512, 512, 24);

        // assign the render texture to the material
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        // Set this bubble's settings to the default bubble material and texture
        settings.material_prefab = uiHandler.defaultBubbleSettings.material_prefab;
        settings.renderTexture_prefab = uiHandler.defaultBubbleSettings.renderTexture_prefab;

        // Set the material and texture
        meshRenderer.sharedMaterial = new Material(settings.material_prefab);
        meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;
    }
}



/*
#if UNITY_EDITOR
[CustomEditor(typeof(DialogueBubbleHandler))]
public class DialogueBubbleEditor : Editor
{
    private void OnEnable()
    {
        DialogueBubbleHandler dialogueBubble = (DialogueBubbleHandler)target;
        dialogueBubble.Awake();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueBubbleHandler dialogueBubble = (DialogueBubbleHandler)target;
        if (GUILayout.Button("Update Text"))
        {
            dialogueBubble.Update();
        }
    }
}
#endif
*/

/*
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
*/

