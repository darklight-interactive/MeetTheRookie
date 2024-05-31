using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioController : MonoBehaviour
{
    public PlayerController playerController => GetComponent<PlayerController>();


    public void Awake()
    {
        playerController.stateMachine.OnStateChanged += OnStateChanged;     
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