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
    [SerializeField]
    EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> _dataLibrary;

    public override EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite> DataLibrary
    {
        get
        {
            if (_dataLibrary == null)
            {
                _dataLibrary = new EnumObjectLibrary<Spatial2D.AnchorPoint, Sprite>()
                {
                    ReadOnlyKey = true,
                    ReadOnlyValue = false
                };
            }
            else
            {
                _dataLibrary.ReadOnlyKey = true;
                _dataLibrary.ReadOnlyValue = false;
            }
            _dataLibrary.SetRequiredKeys(new List<Spatial2D.AnchorPoint>
                {
                    Spatial2D.AnchorPoint.TOP_LEFT,
                    Spatial2D.AnchorPoint.TOP_CENTER,
                    Spatial2D.AnchorPoint.TOP_RIGHT,

                    Spatial2D.AnchorPoint.BOTTOM_LEFT,
                    Spatial2D.AnchorPoint.BOTTOM_CENTER,
                    Spatial2D.AnchorPoint.BOTTOM_RIGHT
                });
            return _dataLibrary;
        }
    }
}

