/*
 * Last Edited by Garrett Blake
 * 4/10/2024
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
    public NPCStateMachine stateMachine;
    private NPCAnimator animationManager;

    // Follow State
    private bool movingInFollowState = false;
    private float currentFollowDistance = 0;

    // Hide State
    private Hideable_Object[] hideableObjects;
    private GameObject closestHideableObject;
    private bool areThereHideableObjects = false;
    private bool movingInHideState = false;
    private float validHideDistance = 0.01f;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player;
    [Range(0.1f, 1f)] public float npcSpeed = .2f;
    [Range(0.1f, 1f)] public float followSpeed = .5f;
    [Range(0.1f, 1f)] public float hideSpeed = .5f;
    [Range(0.1f, 1f)] public float chaseSpeed = .5f;
    public float leftBound;
    public float rightBound;
    public float followDistance = 1;
    public float chaseSpeakDistance = .75f;
    [Range(0f, 10f)] public float idleMaxDuration = 3f;
    [Range(0f, 10f)] public float walkMaxDuration = 3f;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        // Create instances of the states
        IdleState idleState = new(this, ref idleMaxDuration);
        WalkState walkState = new(this, ref npcSpeed, ref walkMaxDuration, ref leftBound, ref rightBound);

        // Create dictionary to hold the possible states
        Dictionary<NPCState, IState<NPCState>> possibleStates = new()
        {
            { NPCState.IDLE, idleState },
            { NPCState.WALK, walkState }
        };

        // initialize the NPCStateMachine
        stateMachine = new NPCStateMachine(NPCState.IDLE, possibleStates, gameObject);

        animationManager = GetComponent<NPCAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Step();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftBound, this.GetComponent<Transform>().position.y, 0), new Vector3(rightBound, this.GetComponent<Transform>().position.y, 0));
    }
}

/*

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
        else if (stateMachine.CurrentState == NPCState.SPEAK)        // SPEAK UPDATE
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
        else if (stateMachine.CurrentState == NPCState.HIDE)        // HIDE UPDATE
        {
            if (movingInHideState)
            {
                float currentHideDistance = closestHideableObject.transform.position.x - transform.position.x;
                int hideDirection = (currentHideDistance < 0) ? -1 : 1;

                float movement = hideDirection * hideSpeed;
                float targetX = transform.position.x + movement;

                // move the character
                transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);
                animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(-hideDirection, 0));
            }
        }
        else if (stateMachine.CurrentState == NPCState.CHASE)       // CHASE UPDATE
        {
            float currentChaseDistance = player.transform.position.x - transform.position.x;

            if (currentChaseDistance <= chaseSpeakDistance)
            {
                GoToState(NPCState.SPEAK);
                return;
            }

            int chaseDirection = (currentChaseDistance < 0) ? -1 : 1;

            float movement = chaseDirection * chaseSpeed;
            float targetX = transform.position.x + movement;

            // move the character
            transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);
            animationManager.FrameAnimationPlayer.FlipTransform(new Vector2(-chaseDirection, 0));
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
        else if (stateMachine.CurrentState == NPCState.HIDE)    // HIDE EXIT
        {
            StopCoroutine(HideCheck());
        }
        else if (stateMachine.CurrentState == NPCState.CHASE)   // CHASE EXIT
        {
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
        else if (stateMachine.CurrentState == NPCState.HIDE)    // HIDE ENTER
        {
            StartCoroutine(HideCheck());
        }
        else if (stateMachine.CurrentState == NPCState.CHASE)   // CHASE ENTER
        {
        }
    }

    #endregion

    #region ================== [ STATE HELPER FUNCTIONS ] ==================

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

    private IEnumerator FollowCheck()
    {
        for (; ; )
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
            }
            else if (movingInFollowState && !beyondFollowDistance)
            {
                movingInFollowState = false;
                animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.IDLE));
            }
        }
    }

    private IEnumerator HideCheck()
    {
        int hideCheckCount = 0;
        for (; ; )
        {
            hideableObjects = FindObjectsByType<Hideable_Object>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            // find the closest hideable object, only if there are any
            if (hideableObjects != null && hideableObjects.Length > 0)
            {
                areThereHideableObjects = true;

                GameObject currentClosest = hideableObjects[0].gameObject;
                float currentPos = this.transform.position.x;
                float currentClosestDistance = Mathf.Abs(currentPos - currentClosest.transform.position.x);

                for (int i = 1; i < hideableObjects.Length; i++)
                {
                    float newDistance = Mathf.Abs(currentPos - hideableObjects[i].transform.position.x);

                    if (newDistance < currentClosestDistance)
                    {
                        currentClosest = hideableObjects[i].gameObject;
                        currentClosestDistance = newDistance;
                    }
                }

                // set the closest hideAbleObject
                closestHideableObject = currentClosest;

                // Setup animation states
                bool beyondValidHideDistance = currentClosestDistance > validHideDistance;
                // we are out of range, need to start moving
                if (beyondValidHideDistance && !movingInHideState)
                {
                    movingInHideState = true;
                    animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.WALK));
                }
                // We just got in range, and need to stop moving
                else if (!beyondValidHideDistance && movingInHideState)
                {
                    movingInHideState = false;
                    animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.HIDE));
                }
            }
            else
            // if there are no hideable objects, then we need to just play the idle anim
            // but only if there used to be hideable objects or this is the first check
            {
                if (areThereHideableObjects || hideCheckCount == 0)
                {
                    animationManager.FrameAnimationPlayer.LoadSpriteSheet(animationManager.GetSpriteSheetWithState(NPCState.IDLE));
                    areThereHideableObjects = false;
                    movingInHideState = false;
                }
            }

            // time between checks
            yield return new WaitForSeconds(1 / 20);
            hideCheckCount++;
        }
    }

    #endregion
}

*/