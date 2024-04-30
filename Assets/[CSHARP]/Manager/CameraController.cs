using Darklight.Game;
using Darklight.Game.Camera;
using UnityEngine;

[RequireComponent(typeof(CameraRig3D))]
public class CameraController : MonoBehaviour
{
    private CameraRig3D cameraRig3D => GetComponent<CameraRig3D>();

    public void Update()
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController == null) return;

        // Change the Camera State based on the Player State
        PlayerStateMachine playerStateMachine = playerController.stateMachine;
        IInteract target = playerController.playerInteractor.ActiveInteractable;
        Interactable interactable = target as Interactable;

        switch (playerStateMachine.CurrentState)
        {
            case PlayerState.IDLE:
            case PlayerState.WALK:
                cameraRig3D.SetLookTarget(playerController.transform, Vector3.up);
                cameraRig3D.StateMachine.GoToState(CameraState.FOLLOW_TARGET);
                break;

            case PlayerState.INTERACTION:
                if (interactable != null)
                {
                    cameraRig3D.SetLookTarget(interactable.transform, Vector3.zero);
                }
                cameraRig3D.StateMachine.GoToState(CameraState.CLOSE_UP);
                break;
        }
    }



}