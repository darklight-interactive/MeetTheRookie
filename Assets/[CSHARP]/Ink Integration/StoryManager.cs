using Ink.Runtime;
using UnityEngine;

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
    private StoryManager() {
        story = new Story(((TextAsset)Resources.Load("[INKY]/inkyTest")).text);
        dialogPrefab = (GameObject)Resources.Load("[INKY]/DialogBubble");
    }

    public void Continue() {

    }

    public void Run(string name, Transform transformToDisplay) {
        story.ChoosePathString(name);
    }

    protected Story story;
}
