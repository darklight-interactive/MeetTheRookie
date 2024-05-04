using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML.Element
{
    interface ISelectable
    {
        void Select();
        void Deselect();
    }

    public abstract class SelectableAbstract : VisualElement, ISelectable
    {
        const string BASE_CLASS = "selectable";
        const string SELECTED_CLASS = "selectable:selected";

        // TODO Selectable Events 

        public SelectableAbstract()
        {
            AddToClassList(BASE_CLASS);
        }
        public virtual void Select()
        {
            AddToClassList(SELECTED_CLASS);
        }
        public virtual void Deselect()
        {
            RemoveFromClassList(SELECTED_CLASS);
        }
    }

    [UxmlElement]
    public partial class SelectableElement : SelectableAbstract, ISelectable
    {
        public new class UxmlFactory : UxmlFactory<SelectableElement> { }
    }
}


