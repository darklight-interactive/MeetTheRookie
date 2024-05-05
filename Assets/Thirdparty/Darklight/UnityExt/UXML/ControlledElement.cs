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
            this.focusable = true;
        }
    }

    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class ControlledButton : ControlledElement<Button>
    {
        public Button buttonElement => Element;
        public bool selected;
        public string Text
        {
            get => Element.text;
            set => Element.text = value;
        }

        public ControlledButton()
        {
            Text = "controlled-button";
        }

        public void Select()
        {
            selected = true;
            Element.AddToClassList("selected");
            Element.Focus();
        }

        public void Deselect()
        {
            selected = false;
            Element.RemoveFromClassList("selected");
        }

        public void Activate()
        {
            using (ClickEvent clickEvent = ClickEvent.GetPooled())
            {
                // Set necessary properties of clickEvent
                clickEvent.target = buttonElement;

                // Send the event
                buttonElement.SendEvent(clickEvent);
            }
        }
    }
}
