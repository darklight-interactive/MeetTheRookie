using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Behaviour;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MTRInputManager))]
[RequireComponent(typeof(MTRSceneManager))]
[RequireComponent(typeof(MTRInteractionSystem))]
[RequireComponent(typeof(MTRStoryManager))]
[RequireComponent(typeof(MTR_UIManager))]
[RequireComponent(typeof(MTR_AudioManager))]
public class MTRGameManager : MonoBehaviourSingleton<MTRGameManager>, IUnityEditorListener
{
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
        get { return Instance._playerController = FindFirstObjectByType<MTRPlayerController>(); }
    }

    [SerializeField, Expandable]
    MTRPrefabLibrary _prefabLibrary;

    [SerializeField, ShowOnly]
    MTRCameraController _cameraController;

    [SerializeField, ShowOnly]
    MTRPlayerController _playerController;

    public void OnEditorReloaded()
    {
#if UNITY_EDITOR
        Initialize();

        InkyStoryManager.Instance.Awake();
#endif
    }

    public override void Initialize()
    {
        Cursor.visible = false;

        if (_prefabLibrary == null)
            _prefabLibrary = MTRAssetManager.CreateOrLoadScriptableObject<MTRPrefabLibrary>();
        if (_cameraController == null)
            _cameraController = FindFirstObjectByType<MTRCameraController>();
    }

    public void PlaySpecialAnimation(MTRSpeaker speaker)
    {
        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(
            FindObjectsSortMode.None
        );
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag == speaker)
            {
                interactable
                    .GetComponent<NPC_Controller>()
                    .stateMachine.GoToState(NPCState.PLAY_ANIMATION);
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
