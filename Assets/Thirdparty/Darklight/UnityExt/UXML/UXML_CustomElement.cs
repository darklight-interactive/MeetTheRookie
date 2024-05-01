using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{

    /// <summary>
    /// A base class for creating custom Visual Elements
    /// </summary>
    public partial class UXML_CustomElement : VisualElement
    {
        string _tag;

        public UXML_CustomElement()
        {
            visible = false; // << hide by default
        }

        public UXML_CustomElement(string tag) : this()
        {
            _tag = tag; // << set the element tag
        }

        public UXML_CustomElement(VisualElement element, string tag) : this(tag)
        {
            this.Add(element); // 
        }

        public void SetVisible(bool visible)
        {
            this.visible = visible;
        }

        public void SetWorldToScreenPosition(Vector3 worldPosition)
        {
            Camera cam = Camera.main;
            if (cam == null) throw new System.Exception("No main camera found.");

            // Convert from screen position to a coordinate appropriate for UI Toolkit
            // UI Toolkit origin is top-left, so invert the y-coordinate
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
            screenPosition.y = cam.pixelHeight - screenPosition.y;
            screenPosition.z = 0;

            // Set positions using left and top in style
            this.style.left = screenPosition.x;
            this.style.top = screenPosition.y;
        }

        // TODO : Center the Element
    }
}

