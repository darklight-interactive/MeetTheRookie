/*
 * Last Edited by Garrett Blake
 * 4/5/2024
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

/*
 * IMPORTANT NOTE
 * 
 * If you are changing the state of the NPC to speak, call the GoToSpeak() function!
 */


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
        GoToIdle();
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
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(player.transform.position.x > transform.position.x ? -1 : 1 , 0));
        }
    }

    public void GoToIdle()
    {
        stateMachine.ChangeState(NPCState.IDLE);
        StartCoroutine(IdleTimer());
    }
    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, idleMaxDuration));
        GoToWalk();
    }

    public void GoToWalk()
    {
        if (walkMaxDuration == 0) { return; }

        stateMachine.ChangeState(NPCState.WALK);
        walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;     // When walking, it can be either direction randomly
        StartCoroutine(WalkTimer());
    }
    private IEnumerator WalkTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, walkMaxDuration));
        GoToIdle();
    }

    // Entering the Speak state needs to stop the walk-idle timers, since it can last indefinitely
    public void GoToSpeak()
    {
        stateMachine.ChangeState(NPCState.SPEAK);
        StopCoroutine(IdleTimer());
        StopCoroutine(WalkTimer());
    }
}


