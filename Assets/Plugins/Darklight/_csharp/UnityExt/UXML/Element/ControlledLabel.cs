using UnityEngine;
using UnityEngine.UIElements;

using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }
        private VisualElement _container;
        private float _rollingTextPercentValue;
        private int _currentIndex;
        private int _fontSize;
        private Vector2Int _fontSizeRange = new Vector2Int(96, 128);

        protected Label label;

        [UxmlAttribute, ShowOnly]
        public Vector2 screenSize
        {
            get { return ScreenInfoUtility.ScreenSize; }
            set { }
        }

        [UxmlAttribute, ShowOnly]
        public float aspectRatio
        {
            get { return ScreenInfoUtility.GetScreenAspectRatio(); }
            set { }
        }

        [UxmlAttribute, ShowOnly]
        public int fontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                label.style.fontSize = value;
            }
        }


        [UxmlAttribute, Range(12, 512)]
        public int fontSizeMin
        {
            get { return _fontSizeRange.x; }
            set
            {
                _fontSizeRange.x = value;
                UpdateFontSizeToMatchScreen();
            }
        }


        [UxmlAttribute, Range(12, 512)]
        public int fontSizeMax
        {
            get { return _fontSizeRange.y; }
            set
            {
                _fontSizeRange.y = value;
                UpdateFontSizeToMatchScreen();
            }
        }

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

        public ControlledLabel()
        {
            _container = new VisualElement
            {
                name = "contained-label",
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
                text = fullText,
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    overflow = Overflow.Hidden,
                    alignSelf = Align.FlexEnd
                }
            };

            UpdateFontSizeToMatchScreen();


            _container.Add(label);
            Add(_container);
        }

        public void SetFullText(string fullText)
        {
            this.fullText = fullText;
            SetTextToIndex(0);
        }

        public void OLDUpdateFontSizeToMatchScreen(){
            screenSize = ScreenInfoUtility.ScreenSize;
            aspectRatio = ScreenInfoUtility.GetScreenAspectRatio();

            // Get the font size based on the screen size
            int fontSizeMin = (int)_fontSizeRange.x;
            int fontSizeMax = (int)_fontSizeRange.y;

            // Divide the max font size by the aspect ratio
            float fontSizeByAspectRatio = fontSizeMax / aspectRatio;

            // Clamp the font size to the set range
            fontSize = (int)Mathf.Clamp(fontSizeByAspectRatio, _fontSizeRange.x, _fontSizeRange.y);
        }
        public void UpdateFontSizeToMatchScreen()
        {
            //assuming font size is set assuming the screen is 1920x1080
            this.style.fontSize = new Length(fontSizeMax, LengthUnit.Percent);
        }

        public void RollingTextStep()
        {
            SetTextToIndex(_currentIndex + 1);
        }

        void SetTextToIndex(int index)
        {
            _currentIndex = Mathf.Min(index, fullText.Length);
            this.text = fullText.Substring(0, _currentIndex);
        }

        public void InstantCompleteText()
        {
            SetTextToIndex(fullText.Length);
        }

        public void SetFontSizeRange(Vector2Int range)
        {
            _fontSizeRange = range;
            UpdateFontSizeToMatchScreen();
        }
    }
}
