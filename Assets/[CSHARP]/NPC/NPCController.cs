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
using UnityEngine.InputSystem.LowLevel;


[RequireComponent(typeof(NPCAnimator))]
public class NPCController : MonoBehaviour
{
    public NPCStateMachine stateMachine = new NPCStateMachine(NPCState.IDLE);
    private NPCAnimator animationManager;
    private int walkDirection;
    private bool movingInFollowState = false;
    private float currentFollowDistance = 0;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player;
    [Range(0.1f, 1f)] public float npcSpeed = .2f;
    [Range(0.1f, 1f)] public float followSpeed = .5f;
    public float leftBound;
    public float rightBound;
    public float followDistance = 1;
    [Range(0f, 10f)] public float idleMaxDuration;
    [Range(0f, 10f)] public float walkMaxDuration;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        animationManager = GetComponent<NPCAnimator>();

        // NPC starts walking in a random direction at the start
        walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;
        GoToState(NPCState.FOLLOW);
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
        if (stateMachine.CurrentState == NPCState.IDLE)             // IDLE UPDATE
        {
            // When Idle, the npc doesnt move, so we'll just return
            return;
        }
        // When Moving
        else if (stateMachine.CurrentState == NPCState.WALK)        // WALK UPDATE
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

            // If we are already out of bounds, we want to walk back in bounds
            if (transform.position.x <= leftBound)
            {
                walkDirection = 1;
                movement = walkDirection * npcSpeed;
                targetX = transform.position.x + movement;
            }
            if (transform.position.x >= rightBound)
            {
                walkDirection = -1;
                movement = walkDirection * npcSpeed;
                targetX = transform.position.x + movement;
            }

            // move the character
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

            // Update the Animation
            if (animationManager == null || animationManager.FrameAnimationPlayer == null) { Debug.Log("Player Controller has no FrameAnimationPlayer"); }
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(-walkDirection, 0));
        }
        else if (stateMachine.CurrentState == NPCState.SPEAK)       // SPEAK UPDATE
        {
            // Set NPC to face player when speaking
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(player.transform.position.x > transform.position.x ? -1 : 1, 0));
        }
        else if (stateMachine.CurrentState == NPCState.FOLLOW)       // FOLLOW UPDATE
        {
            if (movingInFollowState)
            {
                int followDirection = (currentFollowDistance < 0) ? -1 : 1;

                float movement = followDirection * followSpeed;
                float targetX = transform.position.x + movement;

                // move the character
                transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);
                animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(-followDirection, 0));
            }
        }
    }

    private IEnumerator FollowCheck()
    {
        for (;;)
        {
            // check 20 times per second
            yield return new WaitForSeconds(1 / 20);

            // If NPC is moving and they're within distance, stop
            // If NPC not moving and they're outside distance, start
            currentFollowDistance = player.transform.position.x - transform.position.x;
            bool beyondFollowDistance = Mathf.Abs(currentFollowDistance) > followDistance;
            if (!movingInFollowState && beyondFollowDistance)
            {
                movingInFollowState = true;
                animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.WALK));
            } else if (movingInFollowState && !beyondFollowDistance)
            {
                movingInFollowState = false;
                animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.IDLE));
            }
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
        else if (stateMachine.CurrentState == NPCState.FOLLOW)  // FOLLOW EXIT
        {
            StopCoroutine(FollowCheck());
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
            // N/A
        }
        else if (stateMachine.CurrentState == NPCState.FOLLOW)  // FOLLOW ENTER
        {
            StartCoroutine(FollowCheck());
        }
    }

    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, idleMaxDuration));
        GoToState(NPCState.WALK);
    }

    private IEnumerator WalkTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, walkMaxDuration));
        GoToState(NPCState.IDLE);
    }

    #endregion 
}


