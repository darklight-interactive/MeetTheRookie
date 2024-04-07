using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour, ISceneSingleton<UIManager>
{
    UIDocument doc;
    VisualElement root;
    private void Awake() {
        (this as ISceneSingleton<UIManager>).Initialize();
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;
        interactPrompt = root.Query<VisualElement>("interactPrompt");
    }

    public VisualElement GetUIComponent(string name) {
        return root.Query(name);
    }

    VisualElement interactPrompt;
    Transform hoverTransform;
    public void DisplayInteractPrompt(bool show, Transform objectToHover = null) {
        interactPrompt.visible = show;
        hoverTransform = objectToHover;
    }

    public static Vector3 WorldToScreen(Vector3 worldPos)
    {
        if (Camera.main == null) throw new System.Exception("No main camera found.");

        Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
        // Per https://forum.unity.com/threads/forcing-a-ui-element-to-follow-a-character-in-3d-space.1010968/
        pos.y = Camera.main.pixelHeight - pos.y;
        pos.z = 0;
        return pos;
    }

    void Update() {
        if (hoverTransform != null) {
            //Debug.Log("Hovering over " + hoverTransform.name + $" at {hoverTransform.position}");

            interactPrompt.transform.position = WorldToScreen(hoverTransform.position);
        }
    }
}
