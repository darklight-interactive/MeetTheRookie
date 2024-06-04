using System;
using Darklight.UnityExt.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    #region ---- [[ SELECTABLE BUTTON ]] ----
    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : Button, ISelectable
    {
        public class SelectableButtonFactory : UxmlFactory<SelectableButton> { }

        public event Action OnSelect;
        public event Action OnClick;
        public SelectableButton()
        {
            text = "selectable-button";
            //ElementButton.clickable.clicked += () => InvokeClickAction();
        }

        public void SetSelected()
        {
            AddToClassList("selected");
            OnSelect?.Invoke();
        }
        public void Deselect() => RemoveFromClassList("selected");
        public void InvokeClickAction() => OnClick?.Invoke();
    }
    #endregion
}