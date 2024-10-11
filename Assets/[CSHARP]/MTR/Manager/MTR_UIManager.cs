using System.Collections;
using System.Collections.Generic;

using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;

using Ink.Runtime;

using NaughtyAttributes;

using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Core2D;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of
/// UIDocuments in the game.
/// </summary>
public class MTR_UIManager : MonoBehaviourSingleton<MTR_UIManager>
{
    PauseMenuController _gameUI;
    SynthesisManager _synthesisManager;



    [SerializeField, ShowOnly] Vector2 _screenSize;
    [SerializeField, ShowOnly] float _screenAspectRatio;
    [SerializeField] CharacterColors _characterColors;

    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] UXML_UIDocumentPreset _mainMenuPreset;
    [SerializeField] UXML_UIDocumentPreset _gameUIPreset;
    [SerializeField] UXML_UIDocumentPreset _synthesisUIPreset;


    // ======== [[ PROPERTIES ]] ================================== >>>>
    public PauseMenuController gameUIController
    {
        get
        {
            // Find the GameUIController if it exists
            if (_gameUI != null)
                return _gameUI;
            _gameUI = FindAnyObjectByType<PauseMenuController>();
            if (_gameUI != null)
                return _gameUI;

            // Create a new GameUIController if it doesn't
            _gameUI = UXML_Utility.CreateUIDocumentObject<PauseMenuController>(_gameUIPreset);
            _gameUI.transform.SetParent(transform);
            return _gameUI;
        }
    }

    public SynthesisManager synthesisManager
    {
        get
        {
            // Find the SynthesisManager if it exists
            if (_synthesisManager != null)
                return _synthesisManager;
            _synthesisManager = FindAnyObjectByType<SynthesisManager>();
            if (_synthesisManager != null)
                return _synthesisManager;

            // Create a new SynthesisManager if it doesn't
            _synthesisManager = UXML_Utility.CreateUIDocumentObject<SynthesisManager>(_synthesisUIPreset);
            return _synthesisManager;
        }
    }

    public MTRSceneTransitionController SceneTransitionController
    {
        get
        {
            return FindAnyObjectByType<MTRSceneTransitionController>();
        }
    }


    #region ------ [[ DIALOGUE BUBBLE ]] ------------------------ >>
    public void CreateDialogueBubbleAtSpeaker(string speaker, string text)
    {
        List<MTRDialogueReciever> dialogueHandlers = FindObjectsByType<MTRDialogueReciever>(FindObjectsSortMode.InstanceID).ToList();

        MTRDialogueReciever dialogueHandler = null;
        foreach (MTRDialogueReciever d in dialogueHandlers)
        {
            d.DestroySpeechBubble();

            // Sometimes the speaker tag is formatted as "Speaker.{speakerName}"
            if (d.SpeakerTag.Contains(speaker))
            {
                dialogueHandler = d;
            }
        }
        if (dialogueHandler == null)
        {
            Debug.LogError("No Dialogue Handler found for speaker: " + speaker);
            return;
        }

        dialogueHandler.CreateNewSpeechBubble(text);
        Debug.Log("Creating Speech Bubble for speaker: " + speaker);
    }


    public void DestroyAllSpeechBubbles()
    {
        List<MTRDialogueReciever> dialogueHandlers = FindObjectsByType<MTRDialogueReciever>(FindObjectsSortMode.InstanceID).ToList();

        foreach (MTRDialogueReciever d in dialogueHandlers)
        {
            d.DestroySpeechBubble();
        }
    }
    #endregion

    #region ------ [[ CHOICE BUBBLE ]] ------------------------ >>

    #endregion



    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public override void Initialize() { }

    // ----- [[ PRIVATE METHODS ]] ------------------------------------>


#if UNITY_EDITOR
    [CustomEditor(typeof(MTR_UIManager))]
    public class UIManagerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        MTR_UIManager _script;
        SerializedProperty _screenSizeProperty;
        SerializedProperty _screenAspectRatioProperty;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTR_UIManager)target;

            _screenSizeProperty = _serializedObject.FindProperty("_screenSize");
            _screenAspectRatioProperty = _serializedObject.FindProperty("_screenAspectRatio");
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            // Set the Screen Size Property value
            _screenSizeProperty.vector2Value = ScreenInfoUtility.ScreenSize;
            _screenAspectRatioProperty.floatValue = ScreenInfoUtility.GetScreenAspectRatio();

            base.OnInspectorGUI();

            _serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
