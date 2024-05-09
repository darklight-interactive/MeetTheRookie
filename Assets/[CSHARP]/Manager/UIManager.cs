using Darklight.UXML;
using UnityEngine;
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
    const string INTERACT_PROMPT_TAG = "icon-interact";

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
    private SynthesisManager _synthesisManager;
    public SynthesisManager synthesisManager {
        get {
            if (_synthesisManager == null) {
                _synthesisManager = new GameObject("UIDocument : SynthesisManager").AddComponent<SynthesisManager>();
                _synthesisManager.Initialize(synthesisUIPreset);
            }
            return _synthesisManager;
        }
    }


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
        icon.visible = true;

    }

    public void HideInteractPrompt()
    {
        VisualElement icon = interactionUI.ElementQuery<VisualElement>(INTERACT_PROMPT_TAG);
        icon.visible = false;
    }

    public void ShowSynthesis(bool visible) {
        synthesisManager.Show(visible);
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

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif