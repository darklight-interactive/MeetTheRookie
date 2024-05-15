using Darklight.UXML;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// This handles all of the Game UI elements like interactions and speech bubbles.
/// </summary>
public class GameUIController : UXML_UIDocumentObject
{
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    public void ShowInteractIcon(Vector3 worldPos)
    {
        VisualElement icon = ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        UIManager.ScaleElementToScreenSize(icon, 0.05f);
        UIManager.SetWorldToScreenPoint(icon, worldPos);
        icon.SetEnabled(true);
        icon.visible = true;
    }

    public void HideInteractIcon()
    {
        VisualElement icon = ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        icon.visible = false;
    }
}