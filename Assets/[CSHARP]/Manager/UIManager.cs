using System;
using Darklight.Game.Utility;
using Darklight.UnityExt.UXML;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    // ----- [[ INTERACTION UI ]] ----------------------------------->>
    [Header("Interaction UI")]
    public UXML_UIDocumentPreset interactionUIPreset;
    public UXML_ScreenSpaceUI interactionUI;


    // ----- [[ WORLD SPACE UI ]] -----------------------------------
    [Space(10), Header("World Space UI")]
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

    [Space(10), Header("Synthesis UI")]
    public UXML_UIDocumentPreset synthesisUIPreset;
    public SynthesisManager synthesisManager { get; private set; }


    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public void Start()
    {
        /*
        interactionUI = new GameObject("InteractionUI").AddComponent<UXML_ScreenSpaceUI>();
        interactionUI.transform.SetParent(transform);
        interactionUI.Initialize(interactionUIPreset, new string[] { "interactPrompt", "choiceGroup" });

        // Initialize the world space UI singleton
        WorldSpaceUI = GetWorldSpaceUI();
        */
    }

    public MonoBehaviour CreateUIObject(UXML_UIDocumentPreset preset, string[] tags)
    {
        UXML_ScreenSpaceUI ui = new GameObject($"UIObject : {preset.name}").AddComponent<UXML_ScreenSpaceUI>();
        ui.Initialize(preset, tags);
        return ui;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIManager))]
public class UIManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    UIManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (UIManager)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        EditorGUILayout.Space(10);
        if (_script.interactionUI == null && GUILayout.Button("Initialize Interaction UI"))
        {
            _script.interactionUI = _script.CreateUIObject(_script.interactionUIPreset, new string[] { "interactPrompt", "choiceGroup" }) as UXML_ScreenSpaceUI;
        }
        else if (_script.interactionUI != null && GUILayout.Button("Destroy Interaction UI"))
        {
            DestroyImmediate(_script.interactionUI.gameObject);
            _script.interactionUI = null;
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif