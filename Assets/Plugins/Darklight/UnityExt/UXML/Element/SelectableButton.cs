using System;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    [UxmlElement]
    public partial class SelectableButton : Button, ISelectable
    {

        public event Action OnSelect;
        public event Action OnClick;
        public SelectableButton()
        {
            text = "selectable-button";
            this.clickable.clicked += () => InvokeClickAction();


        }

        public void SetSelected()
        {
            AddToClassList("selected");
            OnSelect?.Invoke();
        }
        public void Deselect() => RemoveFromClassList("selected");
        public void InvokeClickAction() => OnClick?.Invoke();

        public class SelectableButtonFactory : UxmlFactory<SelectableButton> { }

    }
}