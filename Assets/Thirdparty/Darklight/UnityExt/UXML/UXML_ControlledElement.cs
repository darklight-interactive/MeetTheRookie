using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    interface I_VisualElement
    {
        /// <summary>
        /// The VisualElement reference
        /// </summary>
        VisualElement element { get; }

        /// <summary>
        /// The tag of the element
        /// </summary>
        string tag { get; }

        /// <summary>
        /// Override the visibility of the element
        /// </summary>
        bool visible { get; }
    }

    /// <summary>
    /// The base class for creating eaily controllable Visual Elements
    /// </summary>
    /// 
    public class UXML_ControlledVisualElement : VisualElement, I_VisualElement
    {
        public VisualElement element { get; private set; }
        public string tag { get; private set; }

        public UXML_ControlledVisualElement(VisualElement element, string tag)
        {
            this.element = element as VisualElement;
            this.tag = tag;
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
            element.style.left = screenPosition.x;
            element.style.top = screenPosition.y;
        }

        // TODO : Center the Element
    }
}

