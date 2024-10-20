using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{


    #region ---- [[ SELECTABLE VISUAL ELEMENT ]] ----
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableVisualElement : VisualElement, ISelectableElement
    {
        public class SelectableVisualElementFactory : UxmlFactory<SelectableVisualElement> { }
        public VisualElement Element { get; protected set; }
        public event Action OnSelect;
        public SelectableVisualElement()
        {
            Element = this;
            this.focusable = true;
        }

        public virtual void Select()
        {
            this.AddToClassList("selected");
            this.Focus();
            OnSelect?.Invoke();
        }

        public virtual void Deselect()
        {
            this.RemoveFromClassList("selected");
        }
        public virtual void Enable()
        {
            this.RemoveFromClassList("disabled");
        }

        public virtual void Disable()
        {
            this.AddToClassList("disabled");
        }
    }
    #endregion


}
