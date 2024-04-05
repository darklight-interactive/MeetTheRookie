using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[UxmlElement]
public partial class UXML_InkyLabel : Label
{
    [UxmlAttribute]
    public Sprite bubbleSprite { get; set; }

    public UXML_InkyLabel()
    {
        AddToClassList("inky-label");

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

public class DialogueBubble : MonoBehaviour
{
    private UIDocument uiDocument;
    private PanelSettings panelSettings;
    public VisualElement root;
    public UXML_InkyLabel inkyLabel;

    [TextArea(3, 10)]
    public string dialogueText = "Hello, World!";

    public MeshRenderer meshRenderer;
    public Material material;
    public RenderTexture renderTexture;

    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        panelSettings = uiDocument.panelSettings;
        inkyLabel = root.Q<UXML_InkyLabel>();
        root.Q<UXML_InkyLabel>().dataSource = this;

        Update();
    }

    public void Update()
    {

        // Update the text only if it has changed
        if (inkyLabel.text != dialogueText)
        {
            inkyLabel.UpdateText(dialogueText);
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
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(material);
            meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;

        }
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueBubble))]
public class DialogueBubbleEditor : Editor
{
    private void OnEnable()
    {
        DialogueBubble dialogueBubble = (DialogueBubble)target;
        dialogueBubble.Awake();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueBubble dialogueBubble = (DialogueBubble)target;
        if (GUILayout.Button("Update Text"))
        {
            dialogueBubble.Update();
        }
    }
}
#endif
