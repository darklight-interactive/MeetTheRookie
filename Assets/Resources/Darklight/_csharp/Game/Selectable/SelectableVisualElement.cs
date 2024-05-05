using System;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.Selectable
{

    public interface ISelectable
    {
        void Select();
        void Deselect();
    }

    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableVisualElement<TElement> : VisualElement where TElement : VisualElement, new()
    {
        // Publicly accessible element instance.
        public TElement Element { get; private set; }
        public Rect Rect => Element.worldBound;
        public Vector2 CenterPosition => Element.worldBound.center;
        public SelectableVisualElement()
        {
            Element = new TElement();
            this.Add(Element);
            this.focusable = true;
        }

        /// <summary>
        /// Allows implicit conversion from SelectableElement to the generic class, typeof(TElement) : VisualElement.
        /// </summary>
        /// <param name="wrapper"></param>
        public static implicit operator TElement(SelectableVisualElement<TElement> wrapper)
        {
            return wrapper.Element;
        }
    }

    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : SelectableVisualElement<Button>, ISelectable
    {
        public Button Button => Element as Button;
        public bool selected;
        public string Text
        {
            get => Element.text;
            set => Element.text = value;
        }

        public SelectableButton()
        {
            Text = "controlled-button";
            Button.AddToClassList("controlled-button");
            Button.clicked += () =>
            {
                if (!selected)
                {
                    Select();
                }
                else
                {
                    Deselect();
                }
            };
        }

        public void Select()
        {
            selected = true;

            Debug.Log($"Selected: {Element.name} {Element.style.position}");
        }

        public void Deselect()
        {
            selected = false;
            Button.RemoveFromClassList("selected");
        }

        public void Activate()
        {
            using (ClickEvent clickEvent = ClickEvent.GetPooled())
            {
                // Set necessary properties of clickEvent
                clickEvent.target = Button;

                // Send the event
                Button.SendEvent(clickEvent);

                Debug.Log($"Activated: {Element.name}");
            }
        }
    }
}
