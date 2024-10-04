using System;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    [UxmlElement]
    public partial class SelectableButton : Button, ISelectable
    {
        const string SELECTED_CLASS = "selected";
        const string DISABLED_CLASS = "disabled";

        public event Action OnSelect;
        public event Action OnClick;
        public SelectableButton()
        {
            text = "selectable-button";
            this.clickable.clicked += () => InvokeClickAction();


        }

        public void Select()
        {
            AddToClassList(SELECTED_CLASS);
            OnSelect?.Invoke();
        }
        public void Deselect() => RemoveFromClassList(SELECTED_CLASS);

        public void Enable() => RemoveFromClassList(DISABLED_CLASS);
        public void Disable() => AddToClassList(DISABLED_CLASS);

        public void InvokeClickAction() => OnClick?.Invoke();
        public class SelectableButtonFactory : UxmlFactory<SelectableButton> { }

    }
}