using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    #region ---- [[ CONTROLLED LABEL ]] ----
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }

        [UxmlAttribute, TextArea(1, 10)]
        public string fullText;

        [UxmlAttribute]
        public float fontSize
        {
            get { return _label.style.fontSize.value.value; }
            set { _label.style.fontSize = value; }
        }

        public string text
        {
            get { return _label.text; }
            set { _label.text = value; }
        }


        private VisualElement _container;
        private Label _label;
        private int _currentIndex;

        public ControlledLabel()
        {
            _container = new VisualElement();
            _container.style.backgroundColor = Color.white;
            _container.style.width = new Length(500, LengthUnit.Pixel);
            _container.style.height = new Length(500, LengthUnit.Pixel);
            _container.style.flexWrap = Wrap.Wrap;

            _label = new Label();
            _container.Add(_label);
            Add(_container);
        }

        public void InitializeRollingText(string fullText)
        {
            this.fullText = fullText;
            _currentIndex = 0;
            UpdateText();
        }

        public void Step()
        {
            _currentIndex = (_currentIndex + 1) % fullText.Length;
            UpdateText();
        }

        private void UpdateText()
        {
            this.text = fullText.Substring(_currentIndex) + fullText.Substring(0, _currentIndex);
        }
    }
    #endregion
}
