using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/Library/ChoiceBubbleLibrary")]
public class ChoiceBubbleLibrary : EnumObjectScriptableLibrary<Spatial2D.AnchorPoint, Sprite>
{
    protected override EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> CreateNewLibrary()
    {
        return new EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite>()
        {
            ReadOnlyKey = true,
            ReadOnlyValue = false,
        };
    }
}