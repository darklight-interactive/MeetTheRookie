using Darklight.UnityExt.Core2D;
using Darklight.UnityExt.Library;
using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.UXML;


#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "MeetTheRookie/Library/DialogueBubbleLibrary")]
public class TextBubbleLibrary : EnumObjectScriptableLibrary<Spatial2D.AnchorPoint, Sprite>
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

