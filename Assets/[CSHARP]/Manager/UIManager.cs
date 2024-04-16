using System;
using Darklight.Game.Utility;
using UnityEngine;
using UnityEngine.UIElements;

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of 
/// UIDocuments in the game.
/// </summary>
public class UIManager : MonoBehaviourSingleton<UIManager>
{

    // ----- [[ INTERACTION UI ]] -----------------------------------
    public UXML_UIDocumentPreset interactionUIPreset;
    public static UXML_InteractionUI InteractionUI;

    public void Start()
    {
        // Initialize the interaction UI singleton
        InteractionUI = new GameObject("InteractionUI").AddComponent<UXML_InteractionUI>();
        InteractionUI.transform.SetParent(transform);
    }
}