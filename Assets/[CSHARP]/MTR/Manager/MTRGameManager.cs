using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.Behaviour;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Darklight.UnityExt.Editor;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UniversalInputManager))]
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
        get
        {
            return Instance._playerController = FindFirstObjectByType<MTRPlayerController>();
        }
    }

    [SerializeField, Expandable] MTRPrefabLibrary _prefabLibrary;
    [SerializeField, ShowOnly] MTRCameraController _cameraController;
    [SerializeField, ShowOnly] MTRPlayerController _playerController;



    IEnumerator SceneChangeRoutine(Scene newScene)
    {
        if (MTRSceneManager.ActiveSceneData != null)
        {
            if (MTRStoryManager.IsInitialized == false)
                yield return new WaitUntil(() => MTRStoryManager.IsInitialized);

            MTRStoryManager.GoToKnotOrStitch(MTRSceneManager.ActiveSceneData.SceneKnot);
            MTR_AudioManager.Instance.PlaySceneBackgroundMusic(newScene.name);
        }
        else
        {
            Debug.LogError($"{Prefix} >> SceneChangeRoutine: Scene data not found for scene {newScene.name}");
        }
        yield return null;
    }

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
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(FindObjectsSortMode.None);
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag == speaker)
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

