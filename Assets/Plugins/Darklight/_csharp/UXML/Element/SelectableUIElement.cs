using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    public interface ISelectableUIElement
    {
        public event Action OnSelect;
        public event Action OnClick;
        void Select();
        void Click();
        void Deselect();
    }

    #region ---- [[ SELECTABLE VISUAL ELEMENT ]] ----
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableUIElement<TElement> : VisualElement, ISelectableUIElement where TElement : VisualElement, new()
    {
        // Publicly accessible element instance.
        public TElement Element { get; private set; }
        public Rect Rect => Element.worldBound;
        public Vector2 CenterPosition => Element.worldBound.center;
        public event Action OnSelect;
        public event Action OnClick;
        public SelectableUIElement()
        {
            Element = this as TElement;
            this.focusable = true;
        }

        public virtual void Select()
        {
            this.AddToClassList("selected");
            this.Focus();
            OnSelect?.Invoke();
        }

        public virtual void Click()
        {
            this.RemoveFromClassList("selected");
            this.AddToClassList("clicked");
            OnClick?.Invoke();
        }

        public virtual void Deselect()
        {
            this.RemoveFromClassList("selected");
            this.RemoveFromClassList("clicked");
        }

        /// <summary>
        /// Allows implicit conversion from SelectableElement to the generic class, typeof(TElement) : VisualElement.
        /// </summary>
        /// <param name="wrapper"></param>
        public static implicit operator TElement(SelectableUIElement<TElement> wrapper)
        {
            return wrapper.Element;
        }
    }
    #endregion


}
