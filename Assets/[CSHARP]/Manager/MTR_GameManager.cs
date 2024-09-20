using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.Behaviour;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(UniversalInputManager))]
[RequireComponent(typeof(MTR_SceneManager))]
[RequireComponent(typeof(InkyStoryManager))]
[RequireComponent(typeof(MTR_UIManager))]
[RequireComponent(typeof(MTR_InteractionManager))]
[RequireComponent(typeof(MTR_AudioManager))]
public class MTR_GameManager : MonoBehaviourSingleton<MTR_GameManager>
{
    public static UniversalInputManager InputManager => UniversalInputManager.Instance;
    public static MTR_SceneManager SceneManager => MTR_SceneManager.Instance as MTR_SceneManager;
    public static InkyStoryManager StoryManager => InkyStoryManager.Instance;
    public static MTR_UIManager UIManager => MTR_UIManager.Instance;
    public static MTR_InteractionManager InteractionManager => MTR_InteractionManager.Instance;
    public static MTR_AudioManager AudioManager => MTR_AudioManager.Instance;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        Cursor.visible = false;

        SceneManager.OnSceneChanged += OnSceneChanged;



        InkyStoryManager.GlobalStoryObject.StoryValue.BindExternalFunction("PlaySpecialAnimation", (string speaker) =>
        {
            PlaySpecialAnimation(speaker);
        });
        InkyStoryManager.GlobalStoryObject.StoryValue.BindExternalFunction("PlaySFX", (string sfx) =>
        {
            PlayInkySFX(sfx);
        });
    }

    public void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        //InputManager.Reset();
        InputManager.Awake();

        MTR_SceneData newSceneData = SceneManager.GetSceneData(newScene.name);
        InkyStoryManager.Iterator.GoToKnotOrStitch(newSceneData.knot);
        MTR_AudioManager.Instance.PlaySceneBackgroundMusic(newScene.name);
    }

    public void PlaySpecialAnimation(string speakerName)
    {
        // Set the Camera Target to a NPC
        NPC_Interactable[] interactables = FindObjectsByType<NPC_Interactable>(FindObjectsSortMode.None);
        foreach (NPC_Interactable interactable in interactables)
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
}

