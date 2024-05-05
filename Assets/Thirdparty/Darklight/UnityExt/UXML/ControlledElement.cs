using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class ControlledElement<TElement> : VisualElement where TElement : VisualElement, new()
    {
        // Allows implicit conversion from ControlledElement to the generic type TElement.
        public static implicit operator TElement(ControlledElement<TElement> controlledElement)
        {
            return controlledElement.Element;
        }

        // Publicly accessible element instance.
        public TElement Element { get; private set; }

        public ControlledElement()
        {
            Element = new TElement();
            this.Add(Element);

            this.AddToClassList("controlled-element");
            this.focusable = true;
        }
    }

    // Specific implementation for a generic VisualElement.
    [UxmlElement]
    public partial class ControlledElement : ControlledElement<VisualElement>
    {
        public ControlledElement()
        {
            this.AddToClassList("controlled-element");
        }
    }

    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class ControlledButton : ControlledElement<Button>
    {
        public event Action onClick;

        public string Text
        {
            get => Element.text;
            set => Element.text = value;
        }

        public ControlledButton()
        {
            this.AddToClassList("controlled-button");
            Element.clicked += Element_Clicked;
            Text = " Controlled Button";
        }

        private void Element_Clicked()
        {
            onClick?.Invoke();
        }
    }
}
