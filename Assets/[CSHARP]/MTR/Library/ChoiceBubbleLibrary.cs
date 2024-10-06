using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;

[CreateAssetMenu(menuName = "MeetTheRookie/Library/ChoiceBubbleLibrary")]
public class ChoiceBubbleLibrary : EnumObjectScriptableLibrary<Spatial2D.AnchorPoint, Sprite>
{
    [SerializeField]
    EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> _dataLibrary = new EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite>
    {
        ReadOnlyKey = false,
        ReadOnlyValue = false,
        RequiredKeys = new Spatial2D.AnchorPoint[0],
    };

    public override EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> DataLibrary => _dataLibrary;
}