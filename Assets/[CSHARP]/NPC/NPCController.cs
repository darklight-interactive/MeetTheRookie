/*
 * Last Edited by Garrett Blake
 * 4/9/2024
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Darklight;
using Darklight.UnityExt.Input;
using UnityEngine.InputSystem;
using Darklight.Game.SpriteAnimation;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;


[RequireComponent(typeof(NPCAnimator))]
public class NPCController : MonoBehaviour
{
    public NPCStateMachine stateMachine = new NPCStateMachine(NPCState.IDLE);
    private int walkDirection;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player;
    [Range(0.1f, 1f)] public float npcSpeed = .2f;
    public Vector2 moveVector = Vector2.zero;
    public float leftBound;
    public float rightBound;
    [Range(0f, 10f)] public float idleMaxDuration;
    [Range(0f, 10f)] public float walkMaxDuration;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        // NPC starts walking in a random direction at the start
        walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;
        GoToState(NPCState.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftBound, this.GetComponent<Transform>().position.y, 0), new Vector3(rightBound, this.GetComponent<Transform>().position.y, 0));
    }

    // ================ [ CUSTOM METHODS ] =================== //

    // The main state function, handles each states behaviours


    #region ================== [ HANDLE STATE UPDATES ] ==================
    private void HandleState()
    {
        // When Idle, the npc doesnt move, so we'll just return
        if (stateMachine.CurrentState == NPCState.IDLE)
        {
            return;
        }
        // When Moving
        else if (stateMachine.CurrentState == NPCState.WALK)
        {
            float movement = walkDirection * npcSpeed;
            float targetX = transform.position.x + movement;

            // If we're going out of bounds, flip directions
            if (targetX < leftBound || targetX > rightBound)
            {
                walkDirection *= -1;
                movement = walkDirection * npcSpeed;
                targetX = transform.position.x + movement;
            }

            // move the character
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

            // Update the Animation
            NPCAnimator animationManager = GetComponent<NPCAnimator>();
            if (animationManager == null || animationManager.FrameAnimationPlayer == null) { Debug.Log("Player Controller has no FrameAnimationPlayer"); }
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(-walkDirection, 0));
        }
        else if (stateMachine.CurrentState == NPCState.SPEAK)
        {
            // Set NPC to face player when speaking
            NPCAnimator animationManager = GetComponent<NPCAnimator>();
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(player.transform.position.x > transform.position.x ? -1 : 1, 0));
        }
    }

    #endregion


    #region ================== [ HANDLE STATE CHANGES ] ==================

    // General State Change function, implementing enter and exit
    public void GoToState(NPCState state)
    {
        // Do the process of leaving the current state
        if (stateMachine.CurrentState == NPCState.IDLE)         // IDLE EXIT
        {
            StopCoroutine(IdleTimer());
        }
        else if (stateMachine.CurrentState == NPCState.WALK)    // WALK EXIT
        {

            StopCoroutine(WalkTimer());
        } 
        else if (stateMachine.CurrentState == NPCState.SPEAK)   // SPEAK EXIT
        {
            // N/A
        }

        // Change the state
        stateMachine.ChangeState(state);

        // Execute the OnEnter of the new state
        if (stateMachine.CurrentState == NPCState.IDLE)         // IDLE ENTER
        {
            StartCoroutine(IdleTimer());
        }
        else if (stateMachine.CurrentState == NPCState.WALK)    // WALK ENTER
        {
            if (walkMaxDuration == 0) { GoToState(NPCState.IDLE); }

            // When walking, it can be either direction randomly
            walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;
            StartCoroutine(WalkTimer());
        }
        else if (stateMachine.CurrentState == NPCState.SPEAK)   // SPEAK ENTER
        {
        }
    }

    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, idleMaxDuration));
        GoToState(NPCState.WALK);
    }

    private IEnumerator WalkTimer()
    {
        Debug.Log("WALK STATE START");
        yield return new WaitForSeconds(Random.Range(0, walkMaxDuration));
        Debug.Log("WALK STATE END");
        GoToState(NPCState.IDLE);
    }

    #endregion 
}


