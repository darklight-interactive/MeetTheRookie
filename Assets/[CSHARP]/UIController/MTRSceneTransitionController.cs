using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;

public class MTRSceneTransitionController : UXML_UIDocumentObject, IUnityEditorListener
{
    const string FULLSCREEN_BLACK_TAG = "fullscreen-black";
    const string FADE_IN_CLASS = "fade-in";
    const string FADE_OUT_CLASS = "fade-out";

    ScreenTransition _screenTransitionElement;
    bool _fadeIn;
    bool _fadeOut;
    bool _wipeOpen;
    bool _wipeClose;

    public void Awake()
    {
        Initialize(preset);
    }

    public override void Initialize(UXML_UIDocumentPreset preset, bool clonePanelSettings = false)
    {
        base.Initialize(preset, clonePanelSettings);
        _screenTransitionElement = ElementQuery<ScreenTransition>();

        if (!Application.isPlaying)
        {
            StartFadeIn();
        }
    }

    [Button]
    public void StartFadeIn()
    {
        _fadeIn = true;
        _fadeOut = false;

        _screenTransitionElement = ElementQuery<ScreenTransition>();
        _screenTransitionElement.FadeIn();
    }

    [Button]
    public void StartFadeOut()
    {
        _fadeIn = false;
        _fadeOut = true;

        _screenTransitionElement = ElementQuery<ScreenTransition>();
        _screenTransitionElement.FadeOut();
    }

    [Button]
    public void StartWipeOpen()
    {
        _wipeOpen = true;
        _wipeClose = false;

        _screenTransitionElement = ElementQuery<ScreenTransition>();
        _screenTransitionElement.WipeOpen();
    }

    [Button]
    public void StartWipeClose()
    {
        _wipeOpen = false;
        _wipeClose = true;

        _screenTransitionElement = ElementQuery<ScreenTransition>();
        _screenTransitionElement.WipeClose();
    }

    public void SetAlignment(ScreenTransition.Alignment alignment)
    {
        _screenTransitionElement = ElementQuery<ScreenTransition>();
        _screenTransitionElement.screenAlignment = alignment;
    }

    /*
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
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                _sceneTransitionScript = (MTRSceneTransitionController)target;

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

                if (_sceneTransitionScript._wipeOpen)
                {
                    if (GUILayout.Button("Start Wipe Close"))
                    {
                        _sceneTransitionScript.StartWipeClose();
                    }
                }
                else
                {
                    if (GUILayout.Button("Start Wipe Open"))
                    {
                        _sceneTransitionScript.StartWipeOpen();
                    }
                }

                if (_sceneTransitionScript._screenTransitionElement != null)
                {
                    if (_sceneTransitionScript._screenTransitionElement.screenAlignment == ScreenTransition.Alignment.Left)
                    {
                        if (GUILayout.Button("Set Alignment Right"))
                        {
                            _sceneTransitionScript.SetAlignment(ScreenTransition.Alignment.Right);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Set Alignment Left"))
                        {
                            _sceneTransitionScript.SetAlignment(ScreenTransition.Alignment.Left);
                        }
                    }
                }


                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
    #endif
    */
}
