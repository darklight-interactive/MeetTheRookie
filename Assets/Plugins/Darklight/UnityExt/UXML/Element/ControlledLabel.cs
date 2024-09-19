using UnityEngine;
using UnityEngine.UIElements;

using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using System.Data;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        const string DEFAULT_BKG_PATH = "Assets/Plugins/Darklight/_csharp/UnityExt/UXML/Element/ControlledLabel.uxml";


        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }
        VisualElement _innerContainer;
        float _rollingTextPercentValue = 1;
        int _currentIndex;
        int _fontSize = 20;

        protected Label label;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        // -- (( UXML Attributes )) -------- >> 
        [UxmlAttribute, Range(12, 240)]
        public int fontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                label.style.fontSize = value;
            }
        }


        [Header("Text Properties")]
        [UxmlAttribute, TextArea(3, 10)]
        public string fullText =
            "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

        [UxmlAttribute, ShowOnly]
        public string text
        {
            get { return label.text; }
            set { label.text = value; }
        }

        [UxmlAttribute, Range(0, 1)]
        public float rollingTextPercentage
        {
            get { return _rollingTextPercentValue; }
            set
            {
                _rollingTextPercentValue = value;
                _currentIndex = Mathf.FloorToInt(fullText.Length * _rollingTextPercentValue);
                SetTextToIndex(_currentIndex);
            }
        }

        [UxmlAttribute]
        public TextAnchor textAlign
        {
            get { return label.style.unityTextAlign.value; }
            set { label.style.unityTextAlign = value; }
        }

        // ======== [[ CONSTRUCTORS ]] =============================== >>>>
        public ControlledLabel()
        {
            _innerContainer = new VisualElement
            {
                name = "controlledLabel-container",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Column,
                    overflow = Overflow.Hidden,
                    justifyContent = Justify.FlexStart
                }
            };

            label = new Label
            {
                name = "controlledLabel",
                text = fullText,
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    overflow = Overflow.Hidden,
                    alignSelf = Align.FlexEnd
                }
            };

            // Add the label to the container
            _innerContainer.Add(label);
            this.Add(_innerContainer);


            label.style.fontSize = _fontSize;
        }

        public void SetFullText(string fullText)
        {
            this.fullText = fullText;
            SetTextToIndex(0);
        }

        public void RollingTextStep()
        {
            SetTextToIndex(_currentIndex + 1);
        }

        void SetTextToIndex(int index)
        {
            int ind = Mathf.Min(index, fullText.Length);

            // Skip over any HTML tags
            if (fullText.Length > ind && fullText[ind] == '<')
            {
                while (ind < fullText.Length && fullText[ind] != '>')
                {
                    ind++;
                }
                ind++; // Move past the '>'
            }

            // Ensure _currentIndex stays within bounds
            _currentIndex = Mathf.Min(ind, fullText.Length);
            this.text = fullText.Substring(0, _currentIndex);
        }


        public void InstantCompleteText()
        {
            SetTextToIndex(fullText.Length);
        }
    }
}
