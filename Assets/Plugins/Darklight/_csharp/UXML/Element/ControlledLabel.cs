using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }
        private VisualElement _container;
        private Label _label;
        private float _rollingTextPercentValue = 1;
        private int _currentIndex;


        [UxmlAttribute, TextArea(3, 10)]
        public string fullText = "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

        [UxmlAttribute]
        [Range(8, 128)]
        public float fontSize
        {
            get { return _label.style.fontSize.value.value; }
            set { _label.style.fontSize = value; }
        }

        public string text
        {
            get{ return _label.text; }
            set { _label.text = value;}
        }

        [UxmlAttribute, Range(0, 1)] 
        public float rollingTextPercentage
        {
            get
            {
                return _rollingTextPercentValue;
            }
            set
            {
                _rollingTextPercentValue = value;
                _currentIndex = Mathf.FloorToInt(fullText.Length * _rollingTextPercentValue);
                SetTextToIndex(_currentIndex);
            }
        }

        public ControlledLabel()
        {
            _container = new VisualElement();
            _container.style.overflow = Overflow.Hidden;
            _container.style.flexWrap = Wrap.Wrap;

            _label = new Label();
            
            _container.Add(_label);
            Add(_container);
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
