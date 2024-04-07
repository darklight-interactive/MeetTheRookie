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
    public struct Settings
    {
        [SerializeField, TextArea(3, 10)] public string dialogueText;
        public Sprite bubbleSprite;
        public Material material_prefab;
        public RenderTexture renderTexture_prefab;
        public Settings
        (
            string dialogueText = "",
            Sprite bubbleSprite = null,
            Material material_prefab = null,
            RenderTexture renderTexture_prefab = null
        )
        {
            this.dialogueText = dialogueText;
            this.bubbleSprite = bubbleSprite;
            this.material_prefab = material_prefab;
            this.renderTexture_prefab = renderTexture_prefab;
        }
    }

    public Settings settings = new Settings();
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

    public void Update()
    {
        string text = settings.dialogueText;
        Sprite sprite = settings.bubbleSprite;
        if (text == null || sprite == null)
        {
            Debug.LogError("Text or Sprite is null", this);
            return;
        }

        // Set Values
        if (inkyLabel == null) return;
        if (bubble == null) return;
        if (text != inkyLabel.text || bubble.style.backgroundImage != new StyleBackground(sprite))
        {
            inkyLabel.SetText(text);
            bubble.style.backgroundImage = new StyleBackground(sprite);

            Debug.Log("Updating Text and Bubble Sprite");

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
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(settings.material_prefab);
            meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;
        }
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

