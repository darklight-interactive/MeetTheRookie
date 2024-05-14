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
[RequireComponent(typeof(UXML_UIDocumentObject))]
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    #region ======= [[ STATIC METHODS ]] ======= >>>>
    private static int lastScreenWidth;
    private static int lastScreenHeight;
    /// <summary>
    /// Sets the position of a UI Toolkit element to correspond to a world position.
    /// Optionally centers the element on the screen position.
    /// </summary>
    /// <param name="element">The UI Toolkit element to position.</param>
    /// <param name="worldPosition">The world position to map to screen space.</param>
    /// <param name="center">Optional parameter to center the element at the screen position (default false).</param>
    public static void SetWorldToScreenPoint(VisualElement element, Vector3 worldPosition, bool center = false)
    {
        Camera cam = Camera.main;
        if (cam == null) throw new System.Exception("No main camera found.");

        // Convert world position to screen position
        Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
        screenPosition.y = cam.pixelHeight - screenPosition.y;  // UI Toolkit uses top-left origin
        screenPosition.z = 0;

        if (center)
        {
            // Adjust position to center the element
            screenPosition.x -= element.resolvedStyle.width / 2;
            screenPosition.y -= element.resolvedStyle.height / 2;
        }

        // Set positions using left and top in style
        element.style.left = screenPosition.x;
        element.style.top = screenPosition.y;
    }

    /// <summary>
    /// Adjusts the size of the given VisualElement based on the current screen size.
    /// </summary>
    /// <param name="element">The VisualElement to adjust.</param>
    public static void ScaleElementToScreenSize(VisualElement element, float scale = 1f)
    {
        float maxDimension = Mathf.Max(lastScreenWidth, lastScreenHeight);

        // Adjust the size of the element based on the smaller dimension of the screen
        float newSize = maxDimension * scale;
        element.style.width = new Length(newSize, LengthUnit.Pixel);
        element.style.height = new Length(newSize, LengthUnit.Pixel);

        Debug.Log($"Screen Size: {lastScreenWidth} x {lastScreenHeight}, New Element Size: {newSize}");
    }
    #endregion <<< ======= [[ STATIC METHODS ]] =======

    [Header("UXML Document Objects")]
    [SerializeField] UXML_UIDocumentPreset interactionUIPreset;
    public UXML_UIDocumentObject interactionUIDocument => GetComponent<UXML_UIDocumentObject>();


    [Space(10), Header("Synthesis UI")]
    public UXML_UIDocumentPreset synthesisUIPreset;
    public SynthesisManager synthesisManager { get; private set; }


    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public override void Awake()
    {
        base.Awake(); // << Update the Singleton instance
        UpdateScreenSize();

        interactionUIDocument.Initialize(interactionUIPreset, new string[] { INTERACT_PROMPT_TAG, SPEECH_BUBBLE_TAG });

    }

    public void ShowInteractIcon(Vector3 worldPos)
    {
        UpdateScreenSize();

        VisualElement icon = interactionUIDocument.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        SetWorldToScreenPoint(icon, worldPos, true);
        ScaleElementToScreenSize(icon, 0.1f);
        icon.SetEnabled(true);
        icon.visible = true;
    }
    public void HideInteractIcon()
    {
        VisualElement icon = interactionUIDocument.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        icon.visible = false;
    }


    void Update()
    {
        UpdateScreenSize();
    }

    void UpdateScreenSize()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }






}
