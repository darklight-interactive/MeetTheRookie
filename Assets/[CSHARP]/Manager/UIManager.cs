using Darklight.UXML;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of 
/// UIDocuments in the game.
/// </summary>
[ExecuteAlways]
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    const string INTERACT_PROMPT_TAG = "icon-interact";
    private int lastScreenWidth;
    private int lastScreenHeight;

    // ----- [[ INTERACTION UI ]] ----------------------------------->>
    [Header("Interaction UI")]
    public UXML_UIDocumentPreset interactionUIPreset;
    private UXML_UIDocumentObject _interactionUI;
    public UXML_UIDocumentObject interactionUI
    {
        get
        {
            if (_interactionUI == null)
            {
                _interactionUI = new GameObject("UIDocument : InteractionUI").AddComponent<UXML_UIDocumentObject>();
                _interactionUI.Initialize(interactionUIPreset, new string[] { INTERACT_PROMPT_TAG });
            }
            return _interactionUI;
        }
    }

    // ----- [[ WORLD SPACE UI ]] -----------------------------------
    [Space(10), Header("World Space UI")]
    public UXML_UIDocumentPreset worldSpaceUIPreset;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;
    private UXML_WorldSpaceUI _worldSpaceUI;
    public UXML_WorldSpaceUI worldSpaceUI
    {
        get
        {
            if (_worldSpaceUI == null)
            {
                _worldSpaceUI = new GameObject("UIDocument : WorldSpaceUI").AddComponent<UXML_WorldSpaceUI>();
                _worldSpaceUI.Initialize(worldSpaceUIPreset, new string[] { "inkyLabel" }, worldSpaceMaterial, worldSpaceRenderTexture);
            }
            return _worldSpaceUI;
        }
    }

    [Space(10), Header("Synthesis UI")]
    public UXML_UIDocumentPreset synthesisUIPreset;
    public SynthesisManager synthesisManager { get; private set; }


    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public override void Awake()
    {
        base.Awake();
    }

    public void ShowInteractionPromptInWorld(Vector3 worldPos)
    {
        VisualElement icon = interactionUI.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        interactionUI.SetWorldToScreenPoint(icon, worldPos, true);
        icon.SetEnabled(true);
        ScaleToScreenSize(icon, 0.05f);
        icon.visible = true;
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    /// <summary>
    /// Adjusts the size of the given VisualElement based on the current screen size.
    /// </summary>
    /// <param name="element">The VisualElement to adjust.</param>
    void ScaleToScreenSize(VisualElement element, float scale = 1f)
    {
        float minDimension = Mathf.Min(lastScreenWidth, lastScreenHeight);

        // Adjust the size of the element based on the smaller dimension of the screen
        float newSize = minDimension * scale;
        element.style.width = new Length(newSize, LengthUnit.Pixel);
        element.style.height = new Length(newSize, LengthUnit.Pixel);

        Debug.Log($"Screen Size: {lastScreenWidth}x{lastScreenHeight}, New Element Size: {newSize}");
    }

    public void HideInteractPrompt()
    {
        VisualElement icon = interactionUI.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        icon.visible = false;
    }


}
