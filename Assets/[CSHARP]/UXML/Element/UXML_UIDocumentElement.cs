using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Data structure for an UXML VisualElement.
/// </summary>
public class UXML_UIDocumentElement
{
    public string tag;
    public VisualElement visualElement;
    public UXML_UIDocumentElement(VisualElement element, string tag)
    {
        this.tag = tag;
        this.visualElement = element;
        this.visualElement.visible = false; // << Hide it by default
    }

    public void SetVisible(bool visible)
    {
        visualElement.visible = visible;
    }

    public void SetWorldToScreenPosition(Vector3 worldPosition)
    {
        Camera cam = Camera.main;
        if (cam == null) throw new System.Exception("No main camera found.");

        Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
        screenPosition.y = cam.pixelHeight - screenPosition.y;
        screenPosition.z = 0;

        // Convert from screen position to a coordinate appropriate for UI Toolkit
        // UI Toolkit origin is top-left, so invert the y-coordinate
        float correctY = cam.pixelHeight + screenPosition.y;

        // Set positions using left and top in style
        visualElement.style.left = screenPosition.x;
        visualElement.style.top = screenPosition.y;
    }
}


