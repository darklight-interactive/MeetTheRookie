using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML.Element
{
    [UxmlElement]
    public partial class SelectableButton : Button
    {
        public new class UxmlFactory : UxmlFactory<SelectableButton> { }

        Button internalButton;

        public SelectableButton()
        {
            internalButton = new Button();
            this.Add(internalButton);

            internalButton.text = "New Selectable Button";
        }

        /*
        public override void Select()
        {
            base.Select();
        }

        public override void Deselect()
        {
            base.Deselect();
        }
        */
    }
}