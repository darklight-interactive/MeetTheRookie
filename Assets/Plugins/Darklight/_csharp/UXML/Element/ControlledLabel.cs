using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UXML.Element
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }

        private VisualElement _container;
        private Label _label;
        private Vector2 _screenSize;
        private float _ratio;
        private float _rollingTextPercentValue = 1;
        private int _currentIndex;

        [UxmlAttribute, ShowOnly]
        public Vector2 screenSize
        {
            get
            {
                _screenSize = ScreenUtility.ScreenSize;
                return _screenSize;
            }
            set
            {
                _screenSize = value;
            }
        }

        [UxmlAttribute, ShowOnly]
        public int fontSize
        {
            get { return (int)_label.style.fontSize.value.value; }
            set { _label.style.fontSize = value; }
        }

        [UxmlAttribute, Range(0.01f, 0.5f)]
        public float screenSizeRatio
        {
            get { return _ratio; }
            set { 
                _ratio = value; 
                fontSize = GetScreenFontSize(_ratio);
            }
        }

        [UxmlAttribute, TextArea(3, 10)]
        public string fullText =
            "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

        [UxmlAttribute, ShowOnly]
        public string text
        {
            get { return _label.text; }
            set { _label.text = value; }
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


        public int GetScreenFontSize(float ratio)
        {
            screenSize = ScreenUtility.ScreenSize;
            float minScreenSize = Mathf.Min(screenSize.x, screenSize.y);
            int screenFontSize = Mathf.FloorToInt(minScreenSize * ratio);
            return screenFontSize;
        }

        public ControlledLabel()
        {
            _container = new VisualElement();
            _container.style.overflow = Overflow.Hidden;
            _container.style.flexWrap = Wrap.Wrap;

            _label = new Label();

            _container.Add(_label);
            Add(_container);
        
            fontSize = GetScreenFontSize(_ratio);
        }

        public void Initialize(string fullText)
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
            _currentIndex = Mathf.Min(index, fullText.Length);
            this.text = fullText.Substring(0, _currentIndex);
        }
    }
}
