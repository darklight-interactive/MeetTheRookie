using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/ComicBubble")]
public class ComicBubble : ScriptableObject
{
    public string text;
    public Sprite sprite;
    public float duration = -1;
    public bool isActive = true;
    public int weight = 1;
    public ComicBubble(string text, Sprite sprite, float duration, bool isActive, int weight)
    {
        this.text = text;
        this.sprite = sprite;
        this.duration = duration;
        this.isActive = isActive;
        this.weight = weight;
    }
}