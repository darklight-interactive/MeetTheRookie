using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(MTRPlayerInput))]
public class PlayerAudioController : MonoBehaviour
{
    private MTRPlayerController playerController;


    public void Start()
    {
        playerController = GetComponent<MTRPlayerController>();
        playerController.StateMachine.OnStateChanged += OnStateChanged;
    }

    private MTRPlayerState lastState = MTRPlayerState.NULL;
    public void OnStateChanged(MTRPlayerState newState)
    {
        switch (newState)
        {
            case MTRPlayerState.FREE_WALK:
                MTR_AudioManager.Instance.StartFootstepEvent();
                break;
        }

        if (lastState == MTRPlayerState.FREE_WALK && newState != MTRPlayerState.FREE_WALK)
        {
            MTR_AudioManager.Instance.StopFootstepEvent();
        }

        lastState = newState;
    }

}