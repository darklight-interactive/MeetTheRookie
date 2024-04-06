using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class StoryManager
{
    private static StoryManager instance;
    
    public static StoryManager Instance {
        get {
            if (instance == null) {
                instance = new StoryManager();
            }
            return instance;
        }
    }

    protected GameObject dialogPrefab;
    VisualElement inkUI;
    Label speaker;
    Label text;
    private StoryManager() {
        story = new Story(((TextAsset)Resources.Load("[INKY]/test")).text);
        dialogPrefab = (GameObject)Resources.Load("[INKY]/DialogBubble");
    }

    public void Continue() {
        var storyText = story.Continue();
        text.text = storyText;
    }

    public void Run(string name, Transform transformToDisplay) {
        if (inkUI == null) {
            inkUI = ISceneSingleton<UIManager>.Instance.GetUIComponent("inkDialog");
            speaker = inkUI.Query<Label>("speaker");
            text = inkUI.Query<Label>("inkText");
        }
        story.ChoosePathString(name);
        inkUI.visible = true;
        inkUI.transform.position = UIManager.WorldToScreen(transformToDisplay.position);
        Continue();
    }

    protected Story story;
}
