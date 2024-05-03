using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SideColumn : VisualElement
{
    const string ACTIVE_COLUMN_CLASS = "column-active";
    const string INACTIVE_COLUMN_CLASS = "column-inactive";
    bool _active;


    public new class UxmlFactory : UxmlFactory<SideColumn> { }

    [UxmlAttribute("Active")]
    public bool isActive
    {
        get { return _active; }
        set
        {
            _active = value;
            if (_active)
            {
                AddToClassList(ACTIVE_COLUMN_CLASS);
                RemoveFromClassList(INACTIVE_COLUMN_CLASS);
            }
            else
            {
                AddToClassList(INACTIVE_COLUMN_CLASS);
                RemoveFromClassList(ACTIVE_COLUMN_CLASS);
            }
        }
    }

    public SideColumn()
    {
        isActive = false;
    }
}