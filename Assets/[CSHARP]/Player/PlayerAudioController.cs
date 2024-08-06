using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioController : MonoBehaviour
{
    private PlayerController playerController;


    public void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.stateMachine.OnStateChanged += OnStateChanged;     
    }

    public void OnDestroy()
    {
        MTR_AudioManager.Instance.StopRepeatingEvent();
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
            MTR_AudioManager.Instance.StopRepeatingEvent();
        }

        lastState = newState;
    }

}