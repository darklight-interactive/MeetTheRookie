using System.Collections.Generic;
using Darklight.UnityExt.Core2D;
using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/SpeechBubbleDataObject")]
public class SpeechBubbleDataObject : ScriptableObject
{
    [System.Serializable]
    public class BubbleSprite
    {
        public Spatial2D.AnchorPoint anchorPoint;
        public Sprite sprite;
    }
    public List<BubbleSprite> bubbleSprites;

    public Sprite GetSprite(Spatial2D.AnchorPoint anchorPoint)
    {
        foreach (BubbleSprite bubbleSprite in bubbleSprites)
        {
            if (bubbleSprite.anchorPoint == anchorPoint)
            {
                return bubbleSprite.sprite;
            }
        }
        Debug.LogError($"No sprite found for anchor point: {anchorPoint}");
        return null;
    }
}