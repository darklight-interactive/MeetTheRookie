using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;
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
        if (story.canContinue) {
            var storyText = story.Continue();
            text.text = storyText;
        } else {
            if (story.currentChoices.Count > 0) {
                text.text = "CHOICE";
            } else {
                inkUI.visible = false;
                OnKnotCompleted();
            }
        }
    }

    public delegate void KnotComplete();
    protected event KnotComplete OnKnotCompleted;
    public void Run(string name, Transform transformToDisplay, KnotComplete onComplete) {
        if (inkUI == null) {
            inkUI = ISceneSingleton<UIManager>.Instance.GetUIComponent("inkDialog");
            speaker = inkUI.Query<Label>("speaker");
            text = inkUI.Query<Label>("inkText");
        }
        story.ChoosePathString(name);
        inkUI.visible = true;
        inkUI.transform.position = UIManager.WorldToScreen(transformToDisplay.position) - new Vector3(inkUI.contentRect.width/4, inkUI.contentRect.height/2);
        Continue();
        OnKnotCompleted += onComplete;
    }

    protected Story story;
}
