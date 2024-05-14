using Darklight.UXML;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of 
/// UIDocuments in the game.
/// </summary>
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    #region ======= [[ STATIC METHODS ]] ============================================= >>>>
    private static int lastScreenWidth;
    private static int lastScreenHeight;

    /// <summary>
    /// Creates a new UIDocumentObject from a given preset.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    public static TDocument CreateUIDocumentObject<TDocument>(UXML_UIDocumentPreset preset) where TDocument : UXML_UIDocumentObject
    {
        GameObject go = new GameObject($"UIDocument : {preset.name}");
        go.hideFlags = HideFlags.DontSaveInEditor;
        TDocument uiDocument = go.AddComponent<TDocument>();
        uiDocument.Initialize(preset);
        return uiDocument;
    }

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

    [Header("Base UIDocument")]
    private UXML_UIDocumentObject baseUI;
    private InteractableUI _interactableUI;
    [SerializeField] UXML_UIDocumentPreset interactableUIPreset;
    public InteractableUI interactableUI
    {
        get
        {
            _interactableUI = FindAnyObjectByType<InteractableUI>();
            if (_interactableUI == null)
            {
                _interactableUI = CreateUIDocumentObject<InteractableUI>(interactableUIPreset);
            }
            return _interactableUI;
        }
    }

    [Header("Synthesis UIDocument")]
    [ShowOnly] public SynthesisManager synthesisManager;
    [SerializeField] UXML_UIDocumentPreset synthesisUIPreset;


    // ----- [[ UNITY METHODS ]] ------------------------------------>

    public override void Awake()
    {
        base.Awake(); // << Update the Singleton instance
        UpdateScreenSize();


        //menuUI = CreateUIDocumentObject<MenuUI>(menuUIPreset);

        //interactableUI = CreateUIDocumentObject<InteractableUI>(interactableUIPreset);


        //synthesisManager = CreateUIDocumentObject<SynthesisManager>(synthesisUIPreset);

        synthesisManager = FindAnyObjectByType<SynthesisManager>();
        synthesisManager?.Prepare();
    }

    public void ShowInteractIcon(Vector3 worldPos)
    {
        UpdateScreenSize();

        VisualElement icon = interactableUI.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        SetWorldToScreenPoint(icon, worldPos);
        ScaleElementToScreenSize(icon, 0.05f);
        icon.SetEnabled(true);
        icon.visible = true;
    }
    public void HideInteractIcon()
    {
        VisualElement icon = interactableUI.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        icon.visible = false;
    }

    public void ShowSynthesis(bool visible)
    {
        synthesisManager.Show(visible);
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


#if UNITY_EDITOR
    [MenuItem("Tools/Darklight/Clean Hidden Documents")]
    public static void CleanHiddenDocuments()
    {
        UIDocument[] allDocuments = Resources.FindObjectsOfTypeAll<UIDocument>();
        int count = 0;

        foreach (UIDocument doc in allDocuments)
        {
            GameObject obj = doc.gameObject;
            if ((obj.hideFlags & HideFlags.DontSaveInEditor) != 0)
            {
                DestroyImmediate(obj);
                count++;
            }
        }

        Debug.Log($"{count} hidden objects have been destroyed.");
    }
#endif
}
