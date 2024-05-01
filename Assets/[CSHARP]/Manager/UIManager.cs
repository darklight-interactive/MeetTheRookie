using System;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of 
/// UIDocuments in the game.
/// </summary>
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    // ----- [[ WORLD SPACE UI ]] -----------------------------------
    public UXML_UIDocumentPreset worldSpaceUIPreset;
    public Material worldSpaceMaterial;
    public RenderTexture worldSpaceRenderTexture;
    public static UXML_WorldSpaceUI WorldSpaceUI;
    UXML_WorldSpaceUI GetWorldSpaceUI()
    {
        // Initialize the world space UI singleton
        UXML_WorldSpaceUI worldSpaceUI = new GameObject("WorldSpaceUI").AddComponent<UXML_WorldSpaceUI>();
        worldSpaceUI.transform.SetParent(transform);
        return worldSpaceUI;
    }

    public void Start()
    {
        // Initialize the world space UI singleton
        WorldSpaceUI = GetWorldSpaceUI();

    }


}