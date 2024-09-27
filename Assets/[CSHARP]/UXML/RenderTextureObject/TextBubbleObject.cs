using System.Linq;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextBubbleObject : UXML_RenderTextureObject
{
    TextBubble _textBubble;

    readonly string DEFAULT_TEXT = "This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

    [SerializeField, ShowOnly] string _currText;

    protected override void OnInitialized()
    {
        Debug.Log($"TextBubbleObject.OnInitialized()", this);
        base.OnInitialized();

        _textBubble = ElementQuery<TextBubble>();
        _textBubble.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            //float fullTextHeight = evt.newRect.height;
            //float fullTextWidth = evt.newRect.width;

            //_textBubble.style.height = fullTextHeight;
            //_textBubble.style.width = fullTextWidth;

            //_textBubble.SetFullText(currentText);
            //StartCoroutine(SpeechBubbleRollingTextRoutine(text, 0.025f));

            TextureUpdate();
            Debug.Log($"TextBubbleObject.OnInitialized() - ChangeEvent<string>", this);
        });
    }

    public void SetText(string text)
    {
        _textBubble.SetFullText(text);
        _textBubble.InstantCompleteText();
        _currText = text;

        TextureUpdate();
    }

    public void Select()
    {
        _textBubble.AddToClassList("selected");
    }

    public void Deselect()
    {
        _textBubble.RemoveFromClassList("selected");
    }

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

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}