using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.Behaviour;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UniversalInputManager))]
[RequireComponent(typeof(MTRSceneManager))]
[RequireComponent(typeof(MTRInteractionSystem))]
[RequireComponent(typeof(InkyStoryManager))]
[RequireComponent(typeof(MTR_UIManager))]
[RequireComponent(typeof(MTR_AudioManager))]
public class MTRGameManager : MonoBehaviourSingleton<MTRGameManager>
{
    public static MTRSceneManager SceneManager => MTRSceneManager.Instance;
    public static MTRPrefabLibrary PrefabLibrary => Instance._prefabLibrary;
    public static MTRCameraController CameraController
    {
        get
        {
            if (Instance._cameraController == null)
            {
                Instance._cameraController = FindFirstObjectByType<MTRCameraController>();
            }
            return Instance._cameraController;
        }
    }
    public static MTRPlayerController PlayerController
    {
        get
        {
            return Instance._playerController = FindFirstObjectByType<MTRPlayerController>();
        }
    }

    [SerializeField, Expandable] MTRPrefabLibrary _prefabLibrary;
    [SerializeField, ShowOnly] MTRCameraController _cameraController;
    [SerializeField, ShowOnly] MTRPlayerController _playerController;

    public override void Initialize()
    {
        Cursor.visible = false;

        if (_prefabLibrary == null)
            _prefabLibrary = MTRAssetManager.CreateOrLoadScriptableObject<MTRPrefabLibrary>();
        if (_cameraController == null)
            _cameraController = FindFirstObjectByType<MTRCameraController>();



        if (Application.isPlaying)
        {
            InkyStoryManager.GlobalStoryObject.StoryValue.BindExternalFunction("PlaySpecialAnimation", (string speaker) =>
            {
                PlaySpecialAnimation(speaker);
            });
            InkyStoryManager.GlobalStoryObject.StoryValue.BindExternalFunction("PlaySFX", (string sfx) =>
            {
                PlayInkySFX(sfx);
            });
            SceneManager.OnSceneChanged += OnSceneChanged;

        }

    }

    public void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        MTRSceneData newSceneData = SceneManager.GetSceneData(newScene.name);
        InkyStoryManager.Iterator.GoToKnotOrStitch(newSceneData.knot);
        MTR_AudioManager.Instance.PlaySceneBackgroundMusic(newScene.name);
    }

    public void PlaySpecialAnimation(string speakerName)
    {
        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(FindObjectsSortMode.None);
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag.Contains(speakerName))
            {
                interactable.GetComponent<NPC_Controller>().stateMachine.GoToState(NPCState.PLAY_ANIMATION);
            }
        }
    }

    public void PlayInkySFX(string eventName)
    {
        string eventPath = "event:/SFX/" + eventName;
        FMODUnity.RuntimeManager.PlayOneShot(eventPath);
    }

    public Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) * 0.5f;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRGameManager))]
    public class MTRGameManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRGameManager _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRGameManager)target;

            if (!Application.isPlaying)
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
}

