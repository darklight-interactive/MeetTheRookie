using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using EasyButtons.Editor;
#endif

[RequireComponent(typeof(MTRCameraRig))]
public class MTRCameraController : MonoBehaviour, IUnityEditorListener
{
    public enum SettingType
    {
        DEFAULT,
        CLOSE,
        FAR
    }

    MTRCameraRig _rig;

    [SerializeField, ReadOnly]
    MTRSpeaker _currSpeaker;

    [Header("Camera Setting Preset")]
    [SerializeField]
    SettingType _currSettingType = SettingType.DEFAULT;

    [SerializeField, Expandable]
    MTRCameraSettingPreset _currSetting;

    [Space(10)]
    [SerializeField]
    EnumObjectLibrary<SettingType, MTRCameraSettingPreset> _settings = new EnumObjectLibrary<
        SettingType,
        MTRCameraSettingPreset
    >
    {
        ReadOnlyKey = true,
        RequiredKeys = Enum.GetValues(typeof(SettingType)) as SettingType[]
    };

    MTRStoryManager StoryManager => MTRStoryManager.Instance;
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

    void Start()
    {
        Refresh();
        SetPlayerAsFollowTarget();
    }

    void RefreshBounds()
    {
        if (Rig.Bounds == null || Rig.Bounds != MTRSceneManager.ActiveSceneData.CameraRigBounds)
        {
            Rig.SetBounds(MTRSceneManager.ActiveSceneData.CameraRigBounds);
        }
    }

    void RefreshSettings(bool ignoreSceneData = false)
    {
        // Set the required keys for the settings library
        _settings.SetRequiredKeys(Enum.GetValues(typeof(SettingType)) as SettingType[]);

        // Create or load the camera setting presets
        foreach (SettingType settingType in _settings.Keys)
        {
            if (_settings[settingType] == null)
            {
                _settings[settingType] = MTRAssetManager.CreateOrLoadCameraSettingPreset(
                    settingType
                );
            }
        }

        // Get the current setting type
        if (MTRSceneManager.ActiveSceneData != null && !ignoreSceneData)
            _currSettingType = MTRSceneManager.ActiveSceneData.OnStartCameraSetting;

        // Set the camera settings
        _settings.TryGetValue(_currSettingType, out MTRCameraSettingPreset settingsPreset);
        if (settingsPreset != null)
        {
            Rig.Settings = settingsPreset;
            _currSetting = settingsPreset;
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

    void SetSpeakerTarget(MTRSpeaker speaker)
    {
        _currSpeaker = speaker;

        // Set the Camera Target to the Player
        if (_currSpeaker == MTRSpeaker.LUPE)
        {
            MTRPlayerInteractor player = FindObjectsByType<MTRPlayerInteractor>(
                FindObjectsSortMode.None
            )[0];
            Rig.SetFollowTarget(player.transform);
            return;
        }

        // Set the Camera Target to a NPC
        MTRCharacterInteractable[] interactables = FindObjectsByType<MTRCharacterInteractable>(
            FindObjectsSortMode.None
        );
        foreach (MTRCharacterInteractable interactable in interactables)
        {
            if (interactable.SpeakerTag == _currSpeaker)
            {
                Rig.SetFollowTarget(interactable.transform);
            }
        }
    }

    public void OnEditorReloaded() => Start();

    [EasyButtons.Button]
    public void Refresh()
    {
        if (MTRSceneManager.Instance == null)
            return;
        RefreshBounds();
        RefreshSettings();
    }

    [EasyButtons.Button]
    public void SetSpeakerAsFollowTarget()
    {
        SetSpeakerTarget(_currSpeaker);
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

            _buttonsDrawer.DrawButtons(targets);
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                _script.RefreshBounds();
                _script.RefreshSettings(true);
            }
        }
    }
#endif
}
