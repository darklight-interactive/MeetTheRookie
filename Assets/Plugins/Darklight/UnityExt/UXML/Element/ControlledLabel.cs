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
        const string TAG = "ControlledLabel";
        const string PATH_TO_DEFAULTBKG = "Assets/Plugins/Darklight/_textures/DRKL_TextBubble_Default_0.png";

        VisualElement _labelContainer;
        Label _label;
        Sprite _backgroundImage;

        float _rollingTextPercentValue = 1;
        int _currentIndex;
        int _fontSize = 100;

        protected VisualElement labelContainer { get { return _labelContainer; } set { _labelContainer = value; } }
        protected Label label { get { return _label; } set { _label = value; } }

        #region ======== [[ PROPERTIES ]] ================================== >>>>

        [Header("[CONTROLLED_LABEL] ================ >>>>")]
        [UxmlAttribute, TextArea(3, 10)]
        public string FullText =
            "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

        [UxmlAttribute, ShowOnly]
        public string CurrentText
        {
            get { return label.text; }
            set { label.text = value; }
        }

        [UxmlAttribute, Range(0, 1)]
        public float RollingTextPercentage
        {
            get { return _rollingTextPercentValue; }
            set
            {
                _rollingTextPercentValue = value;
                _currentIndex = Mathf.FloorToInt(FullText.Length * _rollingTextPercentValue);
                SetTextToIndex(_currentIndex);
            }
        }

        [Header("(( Text Style )) ---- >>")]
        [UxmlAttribute, Range(12, 240)]
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                label.style.fontSize = value;
            }
        }

        [UxmlAttribute]
        public TextAnchor TextAlign
        {
            get { return label.style.unityTextAlign.value; }
            set { label.style.unityTextAlign = value; }
        }

        [Header("(( Background Image )) ---- >>")]
        [UxmlAttribute]
        public Sprite BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                _labelContainer.style.backgroundImage = new StyleBackground(_backgroundImage);
            }
        }

        [UxmlAttribute]
        public Color BackgroundColor
        {
            get { return _labelContainer.style.unityBackgroundImageTintColor.value; }
            set { _labelContainer.style.unityBackgroundImageTintColor = value; }
        }
        #endregion

        // ======== [[ CONSTRUCTORS ]] =============================== >>>>
        public ControlledLabel()
        {
            _labelContainer = new VisualElement
            {
                name = $"{TAG}-container",
                style =
                {
                    overflow = Overflow.Hidden,

                    paddingTop = Length.Percent(5),
                    paddingBottom = Length.Percent(5),
                    paddingLeft = Length.Percent(5),
                    paddingRight = Length.Percent(5),


                    unityBackgroundImageTintColor = Color.white
                }
            };

            label = new Label
            {
                name = $"{TAG}-label",
                text = FullText,
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    overflow = Overflow.Hidden,
                    alignSelf = Align.Stretch
                }
            };

            // Add the label to the container
            _labelContainer.Add(label);
            this.Add(_labelContainer);


            label.style.fontSize = _fontSize;
        }

        public void SetFullText(string fullText)
        {
            this.FullText = fullText;
            SetTextToIndex(0);
        }

        public void RollingTextStep()
        {
            SetTextToIndex(_currentIndex + 1);
        }

        void SetTextToIndex(int index)
        {
            int ind = Mathf.Min(index, FullText.Length);

            // Skip over any HTML tags
            if (FullText.Length > ind && FullText[ind] == '<')
            {
                while (ind < FullText.Length && FullText[ind] != '>')
                {
                    ind++;
                }
                ind++; // Move past the '>'
            }

            // Ensure _currentIndex stays within bounds
            _currentIndex = Mathf.Min(ind, FullText.Length);
            this.CurrentText = FullText.Substring(0, _currentIndex);
        }


        public void InstantCompleteText()
        {
            SetTextToIndex(FullText.Length);
        }

        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }
    }
}
