using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioController : MonoBehaviour
{
    private PlayerController playerController;
    private string reverb = "Reverb"; // Based on what it's called in FMOD

    public void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(reverb, 0.0f);
        playerController = GetComponent<PlayerController>();
        playerController.stateMachine.OnStateChanged += OnStateChanged;     
    }

    public void OnDestroy()
    {
        MTR_AudioManager.Instance.StopFootstepEvent();
    }

    private PlayerState lastState = PlayerState.NONE;
    public void OnStateChanged(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.WALK:
                MTR_AudioManager.Instance.StartFootstepEvent();
                break;
        }

        if (lastState == PlayerState.WALK && newState != PlayerState.WALK)
        {
            MTR_AudioManager.Instance.StopFootstepEvent();
        }

        lastState = newState;
    }

}