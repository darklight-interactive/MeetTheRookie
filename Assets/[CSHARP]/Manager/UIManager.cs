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
[ExecuteAlways]
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    #region ======= [[ STATIC METHODS ]] ============================================= >>>>

    /// <summary>
    /// Creates a new UIDocumentObject from a given preset.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    [EasyButtons.Button]
    public static TDocument CreateUIDocumentObject<TDocument>(UXML_UIDocumentPreset preset) where TDocument : UXML_UIDocumentObject
    {
        GameObject go = new GameObject($"UXMLUIDocument : {preset.name}");
        go.hideFlags = HideFlags.DontSaveInEditor;
        TDocument uiDocument = go.AddComponent<TDocument>();
        uiDocument.Initialize(preset);
        return uiDocument;
    }

    public static UXML_RenderTextureObject CreateRenderTextureObject(UXML_UIDocumentPreset preset, Material material, RenderTexture renderTexture)
    {
        GameObject go = new GameObject($"UXMLRenderTexture : {preset.name}");
        go.hideFlags = HideFlags.DontSaveInEditor;
        UXML_RenderTextureObject renderTextureObject = go.AddComponent<UXML_RenderTextureObject>();
        renderTextureObject.Initialize(preset, null, material, renderTexture);
        return renderTextureObject;
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

        Debug.Log($"Set Element Position: {screenPosition}");
    }


    // ---------------- [[ SCREEN SCALING ]] ---------------->
    private static int lastScreenWidth;
    private static int lastScreenHeight;

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

    // ----- [[ PUBLIC FIELDS ]] ------------------------------------>
    private GameUIController _gameUI;
    public GameUIController gameUIController
    {
        get
        {
            // Find the GameUIController if it exists
            if (_gameUI != null) return _gameUI;
            _gameUI = FindAnyObjectByType<GameUIController>();
            if (_gameUI != null) return _gameUI;

            // Create a new GameUIController if it doesn't
            _gameUI = CreateUIDocumentObject<GameUIController>(_gameUIPreset);
            return _gameUI;
        }
    }

    private SynthesisManager _synthesisManager;
    public SynthesisManager synthesisManager
    {
        get
        {
            // Find the SynthesisManager if it exists
            if (_synthesisManager != null) return _synthesisManager;
            _synthesisManager = FindAnyObjectByType<SynthesisManager>();
            if (_synthesisManager != null) return _synthesisManager;

            // Create a new SynthesisManager if it doesn't
            _synthesisManager = CreateUIDocumentObject<SynthesisManager>(_synthesisUIPreset);
            return _synthesisManager;
        }
    }

    // ----- [[ SERIALIZED FIELDS ]] ------------------------------------>
    [SerializeField] UXML_UIDocumentPreset _gameUIPreset;
    [SerializeField] UXML_UIDocumentPreset _synthesisUIPreset;
    [SerializeField] Material _renderTextureMaterial;
    [SerializeField] RenderTexture _renderTexture;



    // ----- [[ UNITY METHODS ]] ------------------------------------>

    public override void Awake()
    {
        base.Awake(); // << Update the Singleton instance
        gameUIController?.HideInteractIcon(); // Hide the interact icon if it's visible        
        synthesisManager?.Prepare(); // Prepare the Synthesis Manager if it exists
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
            Debug.Log($"Screen Size Updated: {lastScreenWidth} x {lastScreenHeight}");
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
