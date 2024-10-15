using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using Darklight.UnityExt.Core2D;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextBubbleObject : UXML_RenderTextureObject
{
    TextBubble _textBubble;

    readonly string DEFAULT_TEXT = "This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

    [SerializeField, ShowOnly] string _currText;

    private bool _isTransitioning = false;

    protected override void OnInitialized()
    {

        base.OnInitialized();


        root.style.flexGrow = 1;
        root.style.flexDirection = FlexDirection.Column;
        root.style.alignSelf = Align.Stretch;

        _textBubble = ElementQuery<TextBubble>();

        // Register for text change event
        _textBubble.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (_currText != evt.newValue)
            {
                _currText = evt.newValue;
            }
        });

        // Register for geometry changes (size/layout)
        _textBubble.RegisterCallback<GeometryChangedEvent>(evt =>
        {
        });

        // Register for animation events
        _textBubble.RegisterCallback<TransitionRunEvent>(evt =>
        {
            _isTransitioning = true;
        });

        _textBubble.RegisterCallback<TransitionEndEvent>(evt =>
        {
            _isTransitioning = false;
        });
    }

    public void SetText(string text)
    {
        if (_currText != text)
        {
            _currText = text;
            _textBubble.SetFullText(text);
            _textBubble.InstantCompleteText();
        }
    }



    public void Select()
    {
        _textBubble.Select();
    }

    public void Deselect()
    {
        _textBubble.Deselect();
    }

    /*
    #if UNITY_EDITOR
        [CustomEditor(typeof(TextBubbleObject))]
        public class TextBubbleObjectCustomEditor : UXML_RenderTextureObjectCustomEditor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                base.OnInspectorGUI();

                if (GUILayout.Button("Select"))
                {
                    (target as TextBubbleObject).Select();
                }

                if (GUILayout.Button("Deselect"))
                {
                    (target as TextBubbleObject).Deselect();
                }

                if (GUILayout.Button("SetOriginToBottom"))
                {
                    (target as TextBubbleObject)._textBubble.OriginPoint = Spatial2D.AnchorPoint.BOTTOM_CENTER;
                }

                if (GUILayout.Button("SetOriginToTop"))
                {
                    (target as TextBubbleObject)._textBubble.OriginPoint = Spatial2D.AnchorPoint.TOP_CENTER;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    #endif
    */
}
