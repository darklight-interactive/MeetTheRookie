using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(MTRPlayerInputController))]
public class PlayerAudioController : MonoBehaviour
{
    private MTRPlayerInputController playerController;


    public void Start()
    {
        playerController = GetComponent<MTRPlayerInputController>();
        playerController.StateMachine.OnStateChanged += OnStateChanged;
    }

    private PlayerState lastState = PlayerState.NULL;
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
            MTR_AudioManager.Instance.StopRepeatingEvent();
        }

        lastState = newState;
    }

}