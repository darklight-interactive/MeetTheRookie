using Darklight.UnityExt.UXML;
using EasyButtons;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MTRSceneTransitionController : UXML_UIDocumentObject
{
    const string FULLSCREEN_BLACK_TAG = "fullscreen-black";
    const string FADE_IN_CLASS = "fade-in";
    const string FADE_OUT_CLASS = "fade-out";

    VisualElement _fullscreenBlackElement;
    bool _fadeIn;
    bool _fadeOut;

    public override void Initialize(UXML_UIDocumentPreset preset, string[] tags = null)
    {
        base.Initialize(preset, tags);

        _fullscreenBlackElement = ElementQuery<VisualElement>(FULLSCREEN_BLACK_TAG);
    }

    #region ( EDITOR UPDATE ) <PRIVATE_METHODS> ================================================
    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

    void EditorUpdate()
    {
        // This ensures smooth updates in the editor
        if (!Application.isPlaying)
        {
            _fullscreenBlackElement.MarkDirtyRepaint();
        }
    }
    #endregion

    [Button]
    public void StartFadeIn()
    {
        _fadeIn = true;
        _fadeOut = false;

        _fullscreenBlackElement.RemoveFromClassList(FADE_OUT_CLASS);
        _fullscreenBlackElement.AddToClassList(FADE_IN_CLASS);

        Debug.Log("Start Fade In" + $"{_fullscreenBlackElement}");
    }

    [Button]
    public void StartFadeOut()
    {
        _fadeIn = false;
        _fadeOut = true;

        _fullscreenBlackElement.RemoveFromClassList(FADE_IN_CLASS);
        _fullscreenBlackElement.AddToClassList(FADE_OUT_CLASS);

        Debug.Log("Start Fade In" + $"{_fullscreenBlackElement}");

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRSceneTransitionController))]
    public class SceneTransitionControllerCustomEditor : UXML_UIDocumentObjectCustomEditor
    {
        SerializedObject _serializedObject;
        MTRSceneTransitionController _sceneTransitionScript;
        public override void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _sceneTransitionScript = (MTRSceneTransitionController)target;
            _sceneTransitionScript.Initialize(_sceneTransitionScript.preset);
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (_sceneTransitionScript._fadeIn)
            {
                if (GUILayout.Button("Start Fade Out"))
                {
                    _sceneTransitionScript.StartFadeOut();
                }
            }
            else
            {
                if (GUILayout.Button("Start Fade In"))
                {
                    _sceneTransitionScript.StartFadeIn();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}