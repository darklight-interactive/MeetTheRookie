using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using NaughtyAttributes;
using UnityEngine;
using EasyButtons.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MTRCameraRig))]
public class MTRCameraController : MonoBehaviour, IUnityEditorListener
{
    MTRCameraRig _rig;

    MTRStoryManager StoryManager => MTRStoryManager.Instance;
    List<string> _speakerList => StoryManager.SpeakerList;
    public MTRCameraRig Rig
    {
        get
        {
            if (_rig == null)
            {
                _rig = GetComponent<MTRCameraRig>();
            }
            return _rig;
        }
    }


    [Dropdown("_speakerList"), SerializeField, ShowOnly] public string currentSpeaker;

    public void OnEditorReloaded()
    {
        Start();
    }

    void Start()
    {
        if (MTRSceneManager.Instance == null) return;
        MTRSceneManager.Instance.CameraBoundsLibrary.GetActiveCameraBounds(out MTRCameraRigBounds cameraBounds);
        if (cameraBounds != null && Rig.Bounds != cameraBounds)
        {
            Rig.SetBounds(cameraBounds);
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        // Observe the current speaker variable in the story
        MTRStoryManager.OnNewSpeaker += SetSpeakerTarget;
    }

    void OnDestroy()
    {
        MTRStoryManager.OnNewSpeaker -= SetSpeakerTarget;
    }

    [EasyButtons.Button]
    public void SetSpeakerAsFollowTarget()
    {
        SetSpeakerTarget(currentSpeaker);
    }

    void SetSpeakerTarget(string speaker)
    {
        currentSpeaker = speaker;

        // Set the Camera Target to the Player
        if (currentSpeaker == "Lupe")
        {
            MTRPlayerInteractor player = FindObjectsByType<MTRPlayerInteractor>(FindObjectsSortMode.None)[0];
            Rig.SetFollowTarget(player.transform);
            return;
        }

        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(FindObjectsSortMode.None);
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag.Contains(currentSpeaker))
            {
                Rig.SetFollowTarget(interactable.transform);
            }
        }
    }

    public void SetPlayerAsFollowTarget()
    {
        MTRPlayerInteractor player = MTRInteractionSystem.PlayerInteractor;
        if (player != null)
            Rig.SetFollowTarget(player.transform);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTRCameraController))]
    public class MTRCameraControllerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        MTRCameraController _script;
        ButtonsDrawer _buttonsDrawer;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTRCameraController)target;
            _buttonsDrawer = new ButtonsDrawer(target);

            _script.Start();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();
            _buttonsDrawer.DrawButtons(targets);

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
